using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Handlers.Inventory.Data;
using Backend.Models.Ammunation.Data;
using Backend.Models.NotificationModel;
using Backend.Modules.Inventory;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Modules.Ammunation
{
    public class AmmunationModule : Script
    {
        private readonly CDBCLient _database;
        private readonly InventoryModule _inventory;

        public AmmunationModule()
        {
            _database = new CDBCLient();
            _inventory = new InventoryModule();

            NAPI.ClientEvent.Register<CPlayer, int, int, int>("Server:Ammunation:BuyItem", this, BuyAmmunationItem);
        }

        private async void BuyAmmunationItem(CPlayer player, int itemId, int amount, int price)
        {
            if (player == null) return;
            if (itemId == 0) return;
            if (amount == 0) return;

            var item = await _database.GetOneFromCollection<AmmunationItemModel>("Ammunation_Items", i => i.ItemId == itemId);
            if (item == null) return;

            price = item.ItemPrice * amount;
            if (price > player.DBModel.Money) {
                NAPI.Task.Run(() =>
                {
                    player.SendCloudNotification("Ammunation", "Dafür fehlt dir leider das Geld!", 3500, NotificationModel.ALERT, false);
                   
                    player.TriggerEvent("Client:Ammunation:Destroy");
                });

                return;
            }

            var itemToAdd = await _database.GetOneFromCollection<Item>("Items", i => i.Name.ToLower() == item.ItemImage);
            if (itemToAdd == null) return;

            player.DBModel.Money -= price;
            player.DBModel.Weapons.Add(itemToAdd.Name);

            _inventory.AddItem(player, itemToAdd, amount);
            NAPI.Task.Run(() =>
            {
                player.SendCloudNotification("Ammunation", "Du hast erfolgreich etwas eingekauft!", 3500, NotificationModel.SUCCESS, false);
                player.TriggerEvent("Client:Ammunation:Destroy");
                player.TriggerEvent("Client:SetMoney", player.DBModel.Money - price);
                return;
            });


            await player.Update();
        }
    }
}
