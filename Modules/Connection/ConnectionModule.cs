using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Models.PlayerModel;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using BCrypt.Net;
using Backend.Modules.Inventory.Data;
using Backend.Models.SmartphoneModel.Apps;
using Backend.Models.NotificationModel;
using Backend.Models.ClothingModel;
using Backend.Models.ClothingModel.Data;
using Backend.Models.BanModel;
using Backend.Handlers.DeathHandler;

namespace Backend.Modules.Connection
{
    public class ConnectionModule : Script
    {
        private CDBCLient _database;

        public ConnectionModule()
        {
            _database = new CDBCLient();
            NAPI.ClientEvent.Register<CPlayer, string, string>("client:submitRegister", this, OnSubmitRegister);
        }
        
        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(CPlayer player, DisconnectionType type, string reason)
        {
            if (player == null) return;
            if (player.DBModel == null) return; 

            player.DBModel.Position = player.Position;
            player.DBModel.Health = player.Health;
            player.DBModel.Armor = player.Armor;

            if (DeathHandler.deadPlayers.Contains(player))
                DeathHandler.deadPlayers.Remove(player);

            NAPI.ClientEvent.TriggerClientEventInRange(player.Position, 20f, "Client:CreateNotification", "Anti Combatlog", $"Der Spieler {player.Name} hat das Spiel verlassen!", 2500, NotificationModel.WARNING, false);
            player?.Update().GetAwaiter();
        }

        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnect(CPlayer player)
        {
            if (player == null) return;

            player.SocialClubID = player.SocialClubId;
            player.HWID = player.Serial;
            player.IsCuffed = false;

            var dbPlayer = _database.GetOneFromCollection<PlayerModel>("Players", p => p.HWID == player.HWID || p.SocialID == player.SocialClubID);
            if (dbPlayer == null) return;
        }

        private async void OnSubmitRegister(CPlayer player, string username, string password)
        {
            if (player == null || username == "" || password == "") return;

            NAPI.Task.Run(() =>
            {
                var banModel = _database.GetOneFromCollection<BanModel>("Bans", b => b.SocialClubID == player.SocialClubID || b.HWID == player.Serial).Result;
                if (banModel != null)
                {
                    player.Kick($"Ban: {banModel.Reason}");
                    return;
                }
            });

            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hash = BCrypt.Net.BCrypt.HashPassword(password, salt);

            var query = await _database.GetOneFromCollection<PlayerModel>("Players", p => p.HWID == player.HWID || p.SocialID == player.SocialClubID);
            if (query != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(password, query.Password)) return;

                player.DBModel = query;

                NAPI.Task.Run(() =>
                {
                    NAPI.ClientEventThreadSafe.TriggerClientEvent(player.Handle, "destroyLogin");
                    NAPI.ClientEventThreadSafe.TriggerClientEvent(player.Handle, "Client:CreateHud", player.DBModel.PlayerId, player.DBModel.Money);

                    player.Position = query.Position;

                    player.SetHealthAC(player.DBModel.Health);
                    player.SetArmorAC(player.DBModel.Armor);

                    if (player.DBModel.InjuryModel.Injured)
                    {
                        player.Kill();
                        DeathHandler.deadPlayers.Add(player);
                    }
                    player.LoggedIn = true;

                    var clothings = player.DBModel.ClothingModel?.clothingComponents;
                    if (clothings == null) return;

                    clothings.ForEach(c =>
                    {
                        if (c == null) return;
                        if (!c.IsProp)
                            NAPI.Player.SetPlayerClothes(player, c.Id, c.Drawable, c.Texture);
                        else
                            NAPI.Player.SetPlayerAccessory(player, c.Id, c.Drawable, c.Texture);
                    });

                    player.DBModel.Weapons.ForEach(p =>
                    {
                        Console.WriteLine(p.ToString() + " found!");
                        player.GiveACWeapon(p);
                    });
                });
                return;
            }
            var id = _database.GetAllFromCollection<PlayerModel>("Players").Result.Count + 1;

            var dbModelToAdd = new PlayerModel
            {
                PlayerId = id,
                Name = username,
                Password = hash,
                HWID = player.HWID,
                SocialID = player.SocialClubID,
                AdminLevel = 0,
                Position = new Vector3(-1042.562255859375, -2745.984619140625, 21.359407424926758),
                InventoryId = _database.GetAllFromCollection<InventoryModel>("Inventories").Result.Count + 1,
                Faction = new PlayerFactionModel(0,"Zivilist", 0, false, false, false),
                PhoneNumber = id,
                SmartPhoneSettings = new SmartphoneSettingsModel()
            };

            var inventoryToAdd = new InventoryModel(1, 50000, 0, 25, dbModelToAdd.InventoryId, 1);


            await _database.AddOneToCollection<PlayerModel>("Players", dbModelToAdd);
            await _database.AddOneToCollection<InventoryModel>("Inventories", inventoryToAdd);

            player.DBModel = dbModelToAdd;
            player.LoggedIn = true;


            player.SetHealthAC(100);
            player.SetArmorAC(0);

            CreateClothings(player);

            NAPI.Task.Run(() =>
            {
                player.Position = dbModelToAdd.Position;
                NAPI.ClientEventThreadSafe.TriggerClientEvent(player.Handle, "destroyLogin");
                NAPI.ClientEventThreadSafe.TriggerClientEvent(player.Handle, "Client:CreateHud", dbModelToAdd.PlayerId, dbModelToAdd.Money);
            });
        }


        private void CreateClothings(CPlayer player)
        {
            if (player == null) return;

            List<ComponentModel> clothesToAdd = new List<ComponentModel>();

            NAPI.Task.Run(() =>
            {
                for (int i = 0; i <= 11; i++)
                {
                    clothesToAdd.Add(new ComponentModel
                    {
                        Id = i,
                        Texture = 0,
                        Drawable = 0,
                        IsProp = false
                    });
                }

                for (int i = 0; i <= 7; i++)
                {
                    if (i != 0 || i != 1 || i != 2 || i != 6 || i != 7) continue;

                    clothesToAdd.Add(new ComponentModel
                    {
                        Id = i,
                        Texture = 0,
                        Drawable = 0,
                        IsProp = true
                    });
                }

                clothesToAdd.ForEach(c => player.DBModel?.ClothingModel?.clothingComponents.Add(c));
                player.Update();
            });
            
        }
    }
}
