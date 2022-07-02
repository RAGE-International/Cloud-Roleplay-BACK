using Backend.Core.Database;
using Backend.Models.VehicleModel;
using Backend.Modules.Garage.Data;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Factories.CColShape.Data
{
    public class GarageShape : Script
    {
        private readonly CDBCLient _database;
        private readonly Pools.Pools _pools;

        public GarageShape()
        {
            _database = new CDBCLient();
            _pools = new Pools.Pools();
        }

        [ServerEvent(Event.ResourceStart)]
        public async Task CreateGarageShape()
        {
            var garageTest = new GarageModel
            {
                Name = "Würfelpark Garage",
                FactionGarage = false,
                Position = new Vector3(100.44974517822266, -1073.51318359375, 29.37415885925293),
                Rotation = new Vector3(),
                ParkOutPosition = new Vector3(107.52803039550781, -1080.51220703125, 29.1927490234375),
                ParkOutRotation = new Vector3(0, 0, 339.4)
            };

            //await _database.AddOneToCollection<GarageModel>("Garages", garageTest);

            _database.GetAllFromCollection<GarageModel>("Garages").Result.ForEach(g =>
            {
                NAPI.Task.Run(() =>
                {
                    CColShape shape = (CColShape)NAPI.ColShape.CreateCylinderColShape(g.Position, 6f, 6f, 0);
                    shape.ShapeName = g.Name;
                    shape.ShapeFunction = RunFunction;

                    Blip b = NAPI.Blip.CreateBlip(357, g.Position, 1f, 0, g.Name);
                    b.ShortRange = true; 
                });
            });
        }

        public async void RunFunction(CPlayer.CPlayer player)
        {
            if (player == null) return;

            var shape = player.CurrentShape;
            if (!shape.ShapeName.Contains("Garage")) return;

            string garageString = "[]";

            List<VehicleModel> ParkInVehicles = _database.GetAllFromCollection<VehicleModel>("Vehicles").Result.Where(v => v.OwnerId == player.DBModel.Id && v.IsParked || v.CoOwners.Contains(player.DBModel.Id) && v.IsParked).ToList();
            List<CVehicle.CVehicle> Vehicles = _pools.GetAllCVehicles().Where(v => v.DBModel.OwnerId == player.DBModel.Id && !v.DBModel.IsParked && v.Position.DistanceTo(player.Position) <= 25f || v.DBModel.CoOwners.Contains(player.DBModel.Id) && !v.DBModel.IsParked && v.Position.DistanceTo(player.Position) <= 25f).ToList();
            List<VehicleModel> ParkedOutVehicles = new List<VehicleModel>();

            Vehicles.ForEach(v =>
            {
                Console.WriteLine(v.DBModel.VehId + " " + v.Id);

                var model = new VehicleModel
                {
                    VehId = v.DBModel.VehId,
                    CoOwners = v.DBModel.CoOwners,
                    InventoryId = v.DBModel.InventoryId,
                    DisplayName = v.DBModel.DisplayName,
                    Health = v.DBModel.Health,
                    Fuel = v.DBModel.Fuel,
                    NumberPlate = v.DBModel.NumberPlate,
                    Km = v.DBModel.Km,
                    IsParked = false
                };

                ParkedOutVehicles.Add(model);
            });

            Console.WriteLine(player.DBModel.Id + NAPI.Util.ToJson(ParkedOutVehicles));

            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "Client:CreateGarage", "[" + NAPI.Util.ToJson(ParkedOutVehicles) + ", " + NAPI.Util.ToJson(ParkInVehicles) + "]", 0, shape.ShapeName);
            });
        }
    }
}
