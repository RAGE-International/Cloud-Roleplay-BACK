using Backend.Core.Database;
using Backend.Models.ShopItemModel;
using Backend.Models.ShopModel;
using Backend.Modules.ItemShop;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Factories.CColShape.Data
{
    public class ItemShopShape : Script
    {
        private readonly CDBCLient _database;
        private readonly ItemShopModule _items;
        public readonly List<ShopModel> shops;


        public ItemShopShape()
        {
            _database = new CDBCLient();
            _items = new ItemShopModule();
            shops = LoadShopShapes().Result;
            //AddExampleShop();
        }

        private void AddExampleShop()
        {
            var shopToAdd = new ShopModel
            {
                ShopName = "Davis Shop",
                ShopPos = new Vector3(24.405349731445312, -1345.55908203125, 29.49701690673828)
            };
            _database.AddOneToCollection<ShopModel>("Shops", shopToAdd);
        }

        private async Task<List<ShopModel>> LoadShopShapes()
        {
            var shops = _database.GetAllFromCollection<ShopModel>("Shops").Result;
            if (shops == null) return await Task.FromResult<List<ShopModel>>(new List<ShopModel> { });

            NAPI.Task.Run(() =>
            {
                shops.ForEach(s =>
                {
                    Blip b = NAPI.Blip.CreateBlip(52, s.ShopPos, 1f, 25, s.ShopName);
                    b.ShortRange = true;

                    CColShape shape = (CColShape)NAPI.ColShape.CreateCylinderColShape(s.ShopPos, 3f, 3f, 0);
                    shape.ShapeName = "ItemShop";
                    shape.ShapeFunction = RunFunction;
                });
            });
            return await Task.FromResult<List<ShopModel>>(shops);
        }

        private void RunFunction(CPlayer.CPlayer player)
        {
            if (player == null) return;
            if (!player.LoggedIn) return;

            if (player.CurrentShape == null) return;
            if (player.CurrentShape.ShapeName != "ItemShop") return;

            Console.WriteLine(NAPI.Util.ToJson(_items.shopItems));

            NAPI.ClientEvent.TriggerClientEvent(player, "Client:CreateItemShop", NAPI.Util.ToJson(_items.shopItems));
        }
    }
}
