using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Core.Factories.CVehicle;
using Backend.Core.Factories.CVehicle.Data;
using Backend.Models.CarShopModel;
using Backend.Models.NotificationModel;
using Backend.Models.VehicleModel;
using Backend.Modules.Inventory.Data;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backend.Handlers.CarShop
{
    public class CarShopHandler : Script
    {
        private readonly CDBCLient _database;

        public CarShopHandler()
        {
            _database = new CDBCLient();
            NAPI.ClientEvent.Register<CPlayer, string, string>("CarShop:BuyVehicle", this, BuyShopVehicle);
        }

        private async void BuyShopVehicle(CPlayer player, string shopName, string vehicleName)
        {
            if (player == null) return;
            Console.WriteLine("SS");
            if (string.IsNullOrEmpty(shopName)) return;
            if (string.IsNullOrEmpty(vehicleName)) return;

            var shop = await _database.GetOneFromCollection<CarShopModel>("CarShops", c => c.Name == shopName.Replace("_", " "));
            if (shop == null) return;

            var vehicleToBuy = shop.carShopVehicles.FirstOrDefault(v => v.Name == vehicleName);
            if (vehicleToBuy == null) return;

            if (vehicleToBuy.Price > player.DBModel.Money) return;

            player.DBModel.Money -= vehicleToBuy.Price;
            await player.Update();

            VehicleModel vehicle = new VehicleModel
            {
                OwnerId = player.DBModel.Id,
                DisplayName = vehicleToBuy.Name,
                NumberPlate = "CGL " + new Random().Next(1, 999999),
                IsParked = true,
                InventoryId = _database.GetAllFromCollection<InventoryModel>("Inventories").Result.Count + 1,
                TrunkWeight = _database.GetOneFromCollection<VehicleData>("VehicleData", v => v.Hash == vehicleName.ToLower()).Result.MaxWeight
            };

            await _database.AddOneToCollection<VehicleModel>("Vehicles", vehicle);

            NAPI.Task.Run(() =>
            {
                player.TriggerEvent("Client:SetMoney", player.DBModel.Money);
                player.TriggerEvent("Client:NativeMenu:Destory");
                player.SendCloudNotification("Fahrzeughandel", $"Du hast erfolgreich einen {vehicle.DisplayName} erworben!", 5000, NotificationModel.CAR, false);
            });
        }
    }
}
