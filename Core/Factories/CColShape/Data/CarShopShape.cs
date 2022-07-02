using Backend.Core.Database;
using Backend.Models.CarShopModel;
using Backend.Models.CarShopModel.Data;
using Backend.Models.ClothingShopModel;
using Backend.Models.NativeMenuModel;
using Backend.Models.NativeMenuModel.Data;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Factories.CColShape.Data
{
    public class CarShopShape : Script
    {
        private readonly CDBCLient _database;

        public CarShopShape()
        {
            _database = new CDBCLient();

            CreateShopShape().GetAwaiter();
            //CreateTestShape();
        }

        private void CreateTestShape()
        {
            var modelToAdd = new CarShopModel
            {
                Position = new Vector3(-1007.2628173828125, -2767.68505859375, 14.39925479888916),
                Name = "Flughafen Fahrzeughandel",
                carShopVehicles = new List<CarShopVehicles> { new CarShopVehicles { Name = "Faggio", Price = 2500 } }
            };

            _database.AddOneToCollection<CarShopModel>("CarShops", modelToAdd);
        }

        private async Task CreateShopShape()
        {
            var carShop = await _database.GetAllFromCollection<CarShopModel>("CarShops");
            if (carShop == null || carShop.Count == 0) return;

            carShop.ForEach(a =>
            {
                NAPI.Task.Run(() =>
                {
                    CColShape shape = (CColShape)NAPI.ColShape.CreateCylinderColShape(a.Position, 2f, 2f, 0);
                    shape.ShapeName = "CarShop";
                    shape.ShapeFunction = RunFunction;
                });
            });
        }

        private async void RunFunction(CPlayer.CPlayer player)
        {
            if (player == null) return;
            if (player.CurrentShape == null) return;

            if (player.CurrentShape.ShapeName != "CarShop") return;


            var shop = _database.GetAllFromCollection<CarShopModel>("CarShops").Result.FirstOrDefault(c => c.Position.DistanceTo(player.Position) <= 3f);
            if (shop == null) return;

            List<NativeItem> items = new List<NativeItem>();
            shop.carShopVehicles.ForEach(a =>
            {
                items.Add(new NativeItem(a.Name, "CarShop:BuyVehicle", new string[] {shop.Name.Replace(" ", "_"),a.Name}));
            });

            NativeMenu menu = new NativeMenu("Fahrzeughandel", "Kaufe ein Fahrzeug deiner Wahl!", items, true);
            if (menu == null) return;

            Console.WriteLine(NAPI.Util.ToJson(menu));

            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "Client:NativeMenu:Create", NAPI.Util.ToJson(menu));
            });
        }
    }
}
