using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Core.Factories.CVehicle;
using Backend.Models.VehicleModel;
using Backend.Modules.Inventory.Data;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Modules.Vehicle
{
    public class VehicleModule : Script
    {
        private readonly CDBCLient _database;

        public VehicleModule()
        {
            _database = new CDBCLient();
        }


        [ServerEvent(Event.PlayerEnterVehicle)]
        public void OnPlayerEnteredVehicle(CPlayer player, CVehicle vehicle, sbyte seatId)
        {
            var vehicleStats = new VehicleStatsModel(1, (float)vehicle.DBModel.Km, (int)vehicle.DBModel.Fuel, 80, 1);

            vehicle.SetSharedData("VEHICLE_STATS", NAPI.Util.ToJson(vehicleStats));
            player.TriggerEvent("playerUpdateCurrentVehicleStats", NAPI.Util.ToJson(vehicleStats));
        }
        public async Task CreateVehicle(CVehicle vehicle, string hash, string ownerId, bool faction)
        {
            var containerId = _database.GetAllFromCollection<InventoryModel>("Inventories").GetAwaiter().GetResult().Count;
            var maxWeight = vehicle.MaxWeight;
            var maxSlots = vehicle.InventorySlots;

            var inventoryModel = new InventoryModel(3, maxWeight, 0, maxSlots, containerId, 3);
            await _database.AddOneToCollection<InventoryModel>("Inventories", inventoryModel);

            if (faction)
            {
                var dbModelToAdd = new VehicleModel
                {
                    InventoryId = containerId,
                    OwnerId = "",
                    CoOwners = new string[4],
                    DisplayName = hash.ToUpper(),
                    Health = 1000,
                    Fuel = 40,
                    NumberPlate = "CLOUD",
                    Km = 0,
                    Note = "ADMIN-ABUSE",
                    IsParked = false,
                    OwnerFaction = ownerId
                };

                vehicle.DBModel = dbModelToAdd;

                await _database.AddOneToCollection<VehicleModel>("Faction_Vehicles", dbModelToAdd);
            }
            else
            {
                var dbModelToAdd = new VehicleModel
                {
                    InventoryId = containerId,
                    OwnerId = ownerId,
                    CoOwners = new string[4],
                    DisplayName = hash.ToUpper(),
                    Health = 1000,
                    Fuel = 40,
                    NumberPlate = "CLOUD",
                    Km = 0,
                    Note = "ADMIN-ABUSE",
                    IsParked = false
                };

                vehicle.DBModel = dbModelToAdd;

                await _database.AddOneToCollection<VehicleModel>("Vehicles", dbModelToAdd);
            }

            
        }
    }
    class VehicleStatsModel
    {
        public int VehicleId { get; set; }
        public float Kilometers { get; set; }
        public int Fuel { get; set; }
        public int MaxFuel { get; set; }
        public int FuelMultiplikator { get; set; }

        public VehicleStatsModel(int VehicleId, float Kilometers, int Fuel, int MaxFuel, int FuelMultiplikator)
        {
            this.VehicleId = VehicleId;
            this.Kilometers = Kilometers;
            this.Fuel = Fuel;
            this.MaxFuel = MaxFuel;
            this.FuelMultiplikator = FuelMultiplikator;
        }
    }
}
