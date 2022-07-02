using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Core.Factories.Pools;
using Backend.Models.NotificationModel;
using Backend.Models.PlayerModel;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backend.Modules.Bank
{
    public class BankModule : Script
    {
        private readonly CDBCLient _database;
        private readonly Pools _pools;

        public BankModule()
        {
            _database = new CDBCLient();
            _pools = new Pools();

            NAPI.ClientEvent.Register<CPlayer, int>("Server:BankWithdrawMoney", this, WithDrawFromBank);
            NAPI.ClientEvent.Register<CPlayer, int>("Server:BankDepositMoney", this, DepositMoney);
            NAPI.ClientEvent.Register<CPlayer, int, int>("Server:BankTransfareMoney", this, TransfareMoney);
        }

        private async void TransfareMoney(CPlayer player, int bank, int amount)
        {
            if (player == null) return;
            if (amount <= 0) return;

            var bankAccountToAdd = await _database.GetOneFromCollection<PlayerModel>("Players", p => p.BankNumber == bank);
            if (bankAccountToAdd == null)
            {
                NAPI.Task.Run(() =>
                {
                    player.SendCloudNotification("ATM", "Diese Kontonummer ist nicht vergeben!", 2500, NotificationModel.ALERT, false);
                    NAPI.ClientEvent.TriggerClientEvent(player, "Client:DestroyBank");
                    return;
                });
            }

            var target = _pools.GetAllCPlayers().FirstOrDefault(p => p.DBModel.Id == bankAccountToAdd?.Id);
            if (target == null) return;

            target.DBModel.BankMoney += amount;
            player.DBModel.BankMoney -= amount;

            await player.Update();
            await target.Update();

            NAPI.Task.Run(() =>
            {
                player.SendCloudNotification("ATM", $"Du hast erfolgreich {amount}$ überwiesen!", 2500, NotificationModel.SUCCESS, false);
                NAPI.ClientEvent.TriggerClientEvent(player, "Client:DestroyBank");

                target.SendCloudNotification("ATM", $"Du hast {amount}$ per Überweisung erhalten!", 2500, NotificationModel.SUCCESS, false);
            });
        }

        private async void DepositMoney(CPlayer player, int amount)
        {
            if (player == null) return;
            if (amount <= 0) return;

            if (player.DBModel.Money < amount)
            {
                NAPI.Task.Run(() =>
                {
                    player.SendCloudNotification("ATM", "Dafür hast du zu wenig Geld!", 2500, NotificationModel.ALERT, false);
                    NAPI.ClientEvent.TriggerClientEvent(player, "Client:DestroyBank");
                    return;
                });
            }

            player.DBModel.Money -= amount;
            player.DBModel.BankMoney += amount;

            await player.Update();

            NAPI.Task.Run(() =>
            {
                player.SendCloudNotification("ATM", $"Du hast erfolgreich {amount}$ eingezahlt!", 2500, NotificationModel.SUCCESS, false);
                NAPI.ClientEvent.TriggerClientEvent(player, "Client:DestroyBank");
                NAPI.ClientEvent.TriggerClientEvent(player, "Client:SetMoney", player.DBModel.Money);
            });
        }

        private async void WithDrawFromBank(CPlayer player, int amount) 
        {
            if (player == null) return;
            if (amount <= 0) return;

            if (player.DBModel.BankMoney < amount)
            {
                NAPI.Task.Run(() =>
                {
                    player.SendCloudNotification("ATM", "Dafür hast du zu wenig Geld!", 2500, NotificationModel.ALERT, false);
                    NAPI.ClientEvent.TriggerClientEvent(player, "Client:DestroyBank");
                    return;
                });
            }

            player.DBModel.Money += amount;
            player.DBModel.BankMoney -= amount;

            await player.Update();

            NAPI.Task.Run(() =>
            {
                player.SendCloudNotification("ATM", $"Du hast erfolgreich {amount}$ ausgezahlt!", 2500, NotificationModel.SUCCESS, false);
                NAPI.ClientEvent.TriggerClientEvent(player, "Client:DestroyBank");
                NAPI.ClientEvent.TriggerClientEvent(player, "Client:SetMoney", player.DBModel.Money);
            });
        }
    }
}
