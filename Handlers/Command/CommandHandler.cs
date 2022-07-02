using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Core.Factories.CVehicle;
using Backend.Core.Factories.CVehicle.Data;
using Backend.Handlers.Inventory;
using Backend.Handlers.Inventory.Data;
using Backend.Models.FactionModel;
using Backend.Models.NotificationModel;
using Backend.Models.PlayerModel;
using Backend.Models.StorageRoom;
using Backend.Models.VehicleModel;
using Backend.Modules.Inventory;
using Backend.Modules.Inventory.Data;
using Backend.Modules.Vehicle;
using Backend.Modules.XMenu.Data;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Handlers.Commands
{
    public class CommandHandler : Script
    {
        private CDBCLient _database;
        private VehicleModule _inventory;

        public CommandHandler()
        {
            _database = new CDBCLient();
            _inventory = new VehicleModule();
        }

        [Command("invite")]
        private async void InviteMember(CPlayer player, string name)
        {
            if (player == null || player.DBModel == null) return;
            if (player.DBModel.Faction.Manager == false) return;

            CPlayer target = (CPlayer)NAPI.Player.GetPlayerFromName(name);
            if (target == null) return;


            DialogModel dialog = new DialogModel
            {
                title = "Fraktions Einladung",
                discription = $"Möchtest du der Fraktion {player.DBModel.Faction.name} beitreten?",
                buttons = new List<DialogButtonObject>
                {
                    new DialogButtonObject
                    {
                        name = "Abbrechen",
                        eventname = "CancleFactionInvite",
                        arguments = new object[]{}
                    },
                    new DialogButtonObject
                    {
                        name = "Bestätigen",
                        eventname = "AcceptFactionInvite",
                        arguments = new object[]{player.DBModel.Faction.id}
                    }
                }
            };

            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEvent(target, "Client:Dialog:Create", NAPI.Util.ToJson(dialog));
            });
        }

        
        [Command("addvehicles")]
        private async void AddVehicles(CPlayer player)
        {
            foreach (VehicleHash hash in Enum.GetValues(typeof(VehicleHash)))
            {
                var data = new VehicleData
                {
                    Hash = hash.ToString().ToLower(),
                    MaxWeight = 0,
                    Slots = 1
                };

                await _database.AddOneToCollection<VehicleData>("VehicleData", data);
            }
        }

        [Command("fveh")]
        private async void AddFVehicle(CPlayer player, string car, string faction)
        {

            var dbVeh = _database.GetOneFromCollection<VehicleData>("VehicleData", v => v.Hash == car.ToLower()).Result;
            if (dbVeh is null) return;

            Console.WriteLine(NAPI.Util.ToJson(dbVeh));

            VehicleHash hash = (VehicleHash)NAPI.Util.GetHashKey(car);
            CVehicle veh = (CVehicle)NAPI.Vehicle.CreateVehicle(hash, player.Position, player.Rotation.X, 0, 0, "ADMIN");

            var ownerId = _database.GetOneFromCollection<PlayerModel>("Players", p => p.SocialID == player.SocialClubID).Result.Id;
            if (ownerId == null) return;

            veh.MaxWeight = dbVeh.MaxWeight;
            veh.InventorySlots = dbVeh.Slots;
            veh.OwnerId = -1;
            veh.Locked = true;
            veh.TrunkLocked = true;
            veh.EngineStatus = false;

            await _inventory.CreateVehicle(veh, car, faction, true);
        }

        [Command("addstorage")]
        private void AddStorageRoom(CPlayer player)
        {
            var container = new InventoryModel(4, 500000, 0, 70, _database.GetAllFromCollection<InventoryModel>("Inventories").Result.Count + 1, 4);
            if (container == null) return;
            _database.AddOneToCollection<InventoryModel>("Inventories", container).GetAwaiter();

            var storage = new StorageRoomModel
            {
                Position = player.Position,
                Dimension = _database.GetAllFromCollection<StorageRoomModel>("StorageRooms").Result.Count + 1,
                IsFactionStorage = true,
                FactionName = player.DBModel.Faction.name,
                ExternalContainerId = container.ExternalContainerID,
            };

            _database.AddOneToCollection<StorageRoomModel>("StorageRooms", storage).GetAwaiter();

            player.SendCloudNotification("Storage-Room", $"Storage Room added for {storage.FactionName}!", 3500, NotificationModel.HOUSE, true);
        }

        [Command("additem")]
        private async void AddItem(CPlayer player, int slot, string itemName, int amount)
        {
            if (player == null || slot < 0 || itemName == "") return;

            var itemToAdd = new Item
            {
                Name = itemName,
                Image = itemName,
                Is_Food = false,
                Is_Weapon = false,
                Weight = 1000,
                MaxCount = 8,
                ItemScript = "Schutzweste"
            };

            var inventory = _database.GetOneFromCollection<InventoryModel>("Inventories", i => i.ExternalContainerID == player.DBModel.InventoryId).Result;
            if (inventory == null) return;

            inventory.Slots[slot].Item = itemToAdd;
            inventory.Slots[slot].ItemCount = amount;

            await _database.Update<InventoryModel>("Inventories", inventory, i => i.ExternalContainerID == player.DBModel.InventoryId);
        }

        [Command("health")]
        public void Health(CPlayer player, int health, int armor)
        {
            player.Health = health;
            player.Armor = armor;
        }

        [Command("weapon")]
        public void Weapon(CPlayer player, string weapon)
        {

            player.GiveWeapon((WeaponHash)NAPI.Util.GetHashKey(weapon), 9999);
        }

        [Command("veh")]
        private async void Vehicle(CPlayer player, string car)
        {
            var dbVeh = _database.GetOneFromCollection<VehicleData>("VehicleData", v => v.Hash == car.ToLower()).Result;
            if (dbVeh is null) return;

            Console.WriteLine(NAPI.Util.ToJson(dbVeh));

            VehicleHash hash = (VehicleHash)NAPI.Util.GetHashKey(car);
            CVehicle veh = (CVehicle)NAPI.Vehicle.CreateVehicle(hash, player.Position, player.Rotation.X, 0, 0, "ADMIN");

            var ownerId = _database.GetOneFromCollection<PlayerModel>("Players", p => p.SocialID == player.SocialClubID).Result.Id;
            if (ownerId == null) return;

            veh.MaxWeight = dbVeh.MaxWeight;
            veh.InventorySlots = dbVeh.Slots;
            veh.OwnerId = player.DBModel.PlayerId;
            veh.Locked = true;
            veh.TrunkLocked = true;
            veh.EngineStatus = false;
            

            await _inventory.CreateVehicle(veh, car, ownerId, false);
        }

        [Command("setfaction")]
        private async void Faction(CPlayer player, string target, string factionName)
        {
            if (player == null) return;
            CPlayer cTarget = (CPlayer)NAPI.Player.GetPlayerFromName(target);
            if (cTarget == null) return;

            var factionModel = _database.GetOneFromCollection<FactionModel>("Factions", f => f.name == factionName).Result;
            if (factionName == null) return;

            cTarget.DBModel.Faction = new PlayerFactionModel(factionModel.id, factionModel.name, 12, true, true, false);
            await cTarget.Update();
        }
    }   
}
