using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Core.Factories.CVehicle;
using Backend.Core.Factories.Pools;
using Backend.Models.NotificationModel;
using Backend.Models.VehicleModel;
using Backend.Modules.Garage.Data;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backend.Modules.Garage
{
    public class GarageModule : Script
    {
        private readonly CDBCLient _database;
        private readonly Pools _pools;

        public GarageModule()
        {
            _database = new CDBCLient();
            _pools = new Pools();

            NAPI.ClientEvent.Register<CPlayer, string>("Server:Garage:ParkOutVehicle", this, ParkOutVehicle);
            NAPI.ClientEvent.Register<CPlayer, string>("Server:Garage:ParkInVehicle", this, ParkInVehicle);
        }

        private async void ParkOutVehicle(CPlayer player, string vehicleId) 
        {
            if (player == null) return;
            if (!player.CurrentShape.ShapeName.Contains("Garage")) return;

            Console.WriteLine(vehicleId);

            GarageModel garage = null;
            
            VehicleModel vehicle = _database.GetOneFromCollection<VehicleModel>("Vehicles", v => v.VehId == vehicleId && v.OwnerId == player.DBModel.Id || v.CoOwners.Contains(player.DBModel.Id)).Result;
            if (vehicle == null)
            {
                vehicle = _database.GetOneFromCollection<VehicleModel>("Faction_Vehicles", v => v.VehId == vehicleId && v.OwnerFaction == player.DBModel.Faction.name).Result; ;
            }
            if (vehicle == null) return;

            if (vehicle.OwnerFaction != "")
                garage = await _database.GetOneFromCollection<GarageModel>("Faction_Garages", g => g.Name == player.CurrentShape.ShapeName);
            else
                garage = await _database.GetOneFromCollection<GarageModel>("Garages", g => g.Name == player.CurrentShape.ShapeName);

            if (garage == null) return;

            NAPI.Task.Run(() =>
            {
                CVehicle veh = (CVehicle)NAPI.Vehicle.CreateVehicle(NAPI.Util.GetHashKey(vehicle.DisplayName), garage.ParkOutPosition, garage.ParkOutRotation.Z, 0, 0, vehicle.NumberPlate);
                veh.DBModel = vehicle;
                veh.DBModel.IsParked = false;
                veh.Locked = true;
                veh.TrunkLocked = true;
                veh.EngineStatus = false;
                veh.Update();

                player.SendCloudNotification("GARAGE", "Fahrzeug: " + veh?.DisplayName + " ausgeparkt!", 2500, NotificationModel.SUCCESS, false);
            });
            
        }

        private async void ParkInVehicle(CPlayer player, string vehicleId)
        { 
            if (player == null) return;
            if (!player.CurrentShape.ShapeName.Contains("Garage")) return;

            Console.WriteLine(vehicleId);

            CVehicle vehicle = _pools.GetAllCVehicles().FirstOrDefault(v => v.Id == vehicleId && v.Position.DistanceTo(player.Position) <= 15f);

            GarageModel garage = null;
            if(vehicle.DBModel.OwnerFaction == "")
                garage = _database.GetOneFromCollection<GarageModel>("Garages", g => g.Name == player.CurrentShape.ShapeName).Result;
            else
                garage = _database.GetOneFromCollection<GarageModel>("Faction_Garages", g => g.Name == player.CurrentShape.ShapeName).Result;

            if (garage == null) return;

            if (vehicle == null)
            {
                player.SendCloudNotification("GARAGE", "Einparken leider nicht möglich! Versuche es erneut!", 3500, NotificationModel.ERROR, false);
                return;
            }

            vehicle.DBModel.IsParked = true;
            Console.WriteLine(vehicle.Id);
            vehicle.Update();
            vehicle.Delete();

            player.SendCloudNotification("GARAGE", "Fahrzeug: " + vehicle.DisplayName + " eingeparkt!", 3500, NotificationModel.SUCCESS, false);
        }
    }
}
