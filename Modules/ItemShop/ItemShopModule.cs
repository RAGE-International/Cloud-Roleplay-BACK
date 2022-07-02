using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Handlers.Inventory.Data;
using Backend.Models.NotificationModel;
using Backend.Models.ShopItemModel;
using Backend.Modules.Inventory;
using Backend.Modules.ItemShop.Data;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Modules.ItemShop
{
    public class ItemShopModule : Script
    {

        private readonly CDBCLient _database;
        public List<ShopItemModel> shopItems;
        private readonly InventoryModule _inventory;
        private readonly ItemData _item;


        public ItemShopModule()
        {
            _database = new CDBCLient();
            shopItems = LoadShopItems().Result;
            _inventory = new InventoryModule();
            _item = new ItemData();
            //AddItemsToDb();

            NAPI.ClientEvent.Register<CPlayer, string>("Server:ItemShop:BuyItems", this, OnShopBuyItems);
        }

        private async void OnShopBuyItems(CPlayer player, string json)
        {
            if (player == null) return;
            if (!player.LoggedIn) return;

            Console.WriteLine(json);

            List<ClientItemShopModel> cart = NAPI.Util.FromJson<List<ClientItemShopModel>>(json);
            if (cart == null) return;

            var totalCartPrice = 0;
            cart.ForEach(x =>
            {
                if (x != null)
                    totalCartPrice += x.Price * x.Quantity;
            }); 

            if (player.DBModel.Money < totalCartPrice)
            {
                player.SendCloudNotification("Item Shop", "Dafür hast du zu wenig Geld!", 2500, NotificationModel.ALERT, false);
                return;
            }

            cart.ForEach(async x =>
            {
                var itemToAdd = _item.Items.FirstOrDefault(i => i.Name == x.Name);
                if (itemToAdd == null) return;

                Console.WriteLine(itemToAdd.Name + " found!");

                if (!_inventory.AddItem(player, itemToAdd, x.Quantity)) return;
            });

            player.DBModel.Money -= totalCartPrice;
            await player.Update();
            player.SendCloudNotification("Item Shop", $"Du hast erfolgreich für {totalCartPrice}$ eingekauft!", 2500, NotificationModel.SUCCESS, false);

        }

        /*public async void AddItemsToDb()
        {
            var shopItemModel = new ShopItemModel
            {
                Id = 1,
                Name = "Verbandskoffer",
                Price = 250
            };
            shopItems.Add(shopItemModel);

            shopItemModel = new ShopItemModel
            {
                Id = 2,
                Name = "Smartphone",
                Price = 500
            };

            shopItems.Add(shopItemModel);

            shopItems.ForEach(s =>
            {
                _database.AddOneToCollection<ShopItemModel>("Shop_Items", s);
            });
        }*/

        public async Task<List<ShopItemModel>> LoadShopItems()
        {
            List<ShopItemModel> query = await _database.GetAllFromCollection<ShopItemModel>("Shop_Items");
            if (query != null)
                return query;

            return new List<ShopItemModel>();
        }
    }

}
