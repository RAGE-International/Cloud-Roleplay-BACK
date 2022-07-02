using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Core.Factories.Pools;
using Backend.Models.BanModel;
using Backend.Models.NotificationModel;
using Backend.Modules.Connection;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backend.Modules.AntiCheat
{
    public class AntiCheatModule : Script
    {
        private readonly CDBCLient _database;
        private readonly Pools _pools;

        public AntiCheatModule()
        {
            _database = new CDBCLient();
            _pools = new Pools();

            NAPI.ClientEvent.Register<CPlayer, int, int>("server:anticheat:callHealKey", this, HealKeyDetected);
            NAPI.ClientEvent.Register<CPlayer>("server:anticheat:callGodMode", this, GodModeDetected);
            NAPI.ClientEvent.Register<CPlayer>("Server:AntiCheat:GiveWeapon", this, GiveWeaponDetected);

        }

        private async void GiveWeaponDetected(CPlayer player)
        {
            if (player == null) return;

            var onlineAdmins = _pools.GetAllCPlayers().ToList().Where(p => p.DBModel.AdminLevel > 0).ToList();
            if (onlineAdmins != null || onlineAdmins.Count > 0)
            {
                onlineAdmins.ForEach(p =>
                {
                    p.SendCloudNotification("Anti-Cheat", $"GiveWeapon Detected : {player.Name}", 7500, NotificationModel.ALERT, true);
                });
            }

            var banModel = new BanModel
            {
                Reason = "Anti-Cheat GiveWeapon",
                BannedBy = "Anti-Cheat",
                HWID = player.DBModel.HWID,
                SocialClubID = player.DBModel.SocialID,
                DbId = player.DBModel.Id
            };

            await _database.AddOneToCollection<BanModel>("Bans", banModel);
            NAPI.Task.Run(() =>
            {
                player.Kick("Banned: Anti-Cheat: GiveWeapon");
            });
        }

        private async void GodModeDetected(CPlayer player)
        {
            if (player == null) return;

            var onlineAdmins = _pools.GetAllCPlayers().ToList().Where(p => p.DBModel.AdminLevel > 0).ToList();
            if (onlineAdmins != null || onlineAdmins.Count > 0)
            {
                onlineAdmins.ForEach(p =>
                {
                    p.SendCloudNotification("Anti-Cheat", $"GodMode Detected : {player.Name}", 7500, NotificationModel.ALERT, true);
                });
            }

            var banModel = new BanModel
            {
                Reason = "Anti-Cheat Godmode",
                BannedBy = "Anti-Cheat",
                HWID = player.DBModel.HWID,
                SocialClubID = player.DBModel.SocialID,
                DbId = player.DBModel.Id
            };

            await _database.AddOneToCollection<BanModel>("Bans", banModel);
            NAPI.Task.Run(() =>
            {

                player.Kick("Banned: Anti-Cheat: GodMode");
            });
        }
        private async void HealKeyDetected(CPlayer player, int allowedHealth, int currentHealth)
        {
            if (player == null) return;

            var onlineAdmins = _pools.GetAllCPlayers().Where(p => p.DBModel.AdminLevel > 0).ToList();
            if (onlineAdmins != null || onlineAdmins.Count > 0)
            {
                onlineAdmins.ForEach(p =>
                {
                    p.SendCloudNotification("Anti-Cheat", $"HealKey Detected : {player.Name}", 7500, NotificationModel.ALERT, true);
                });
            }

            var banModel = new BanModel
            {
                Reason = "Anti-Cheat HealKey",
                BannedBy = "Anti-Cheat",
                HWID = player.DBModel.HWID,
                SocialClubID = player.DBModel.SocialID,
                DbId = player.DBModel.Id
            };

            await _database.AddOneToCollection<BanModel>("Bans", banModel);
            NAPI.Task.Run(() =>
            {

                player.Kick("Banned: Anti-Cheat: HealKey");
            });
        }
    }
}
