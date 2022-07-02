using Backend.Core.Database;
using Backend.Core.Factories.CColShape;
using Backend.Core.Factories.CPlayer;
using Backend.Core.Factories.CVehicle;
using Backend.Core.Factories.Pools;
using Backend.Models.FactionModel;
using Backend.Models.NotificationModel;
using Backend.Models.PlayerModel;
using Backend.Models.VehicleModel;
using Backend.Modules.Garage.Data;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Modules.Faction
{
    public class FactionModule : Script
    {
        private readonly CDBCLient _database;
        public List<FactionModel> Factions_;
        private readonly Pools _pools;

        public FactionModule()
        {
            _database = new CDBCLient();
            _pools = new Pools();   

            NAPI.ClientEvent.Register<CPlayer, int>("AcceptFactionInvite", this, AcceptInvite);
            //AddExampleFaction();
            LoadFactions().GetAwaiter();
        }

        private async void AcceptInvite(CPlayer player, int id)
        {
            if (player == null) return;
            if (id <= 0) return;

            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "Client:Dialog:Destory");
            });

            var factionToAdd = _database.GetOneFromCollection<FactionModel>("Factions", f => f.id == id).Result;
            if (factionToAdd == null) return;

            player.DBModel.Faction = new PlayerFactionModel(factionToAdd.id, factionToAdd.name, 0, false, false, false);
            await player.Update();

            var factionMembers = _pools.GetAllCPlayers().ToList().Where(p => p.DBModel.Faction.id == id).ToList();
            NAPI.Task.Run(() =>
            {
                factionMembers.ForEach(p =>
                {
                    if (p == null) return;
                    p.SendCloudNotification("Fraktion", $"Der Spieler {player.Name} ist der Fraktion beigetreten!", 5000, NotificationModel.ALERT, false);
                });
            });
        }

        private void AddExampleFaction()
        {
            FactionModel model = new FactionModel
            {
                id = 1,
                name = "Ballas",
                SpawnPos = new Vector3(84.89752197265625, -1958.7786865234375, 21.121654510498047),
                GaragePos = new Vector3(84.33366394042969, -1966.87353515625, 20.939176559448242),
                ParkOutPos = new Vector3(88.47847747802734, -1967.932373046875, 20.755760192871094),
                ParkOutRot = new Vector3(0, 0, 319.27),
                ColorHex = "##990e8d"

            };
            _database.AddOneToCollection<FactionModel>("Factions", model);
        }

        public async Task<List<FactionModel>> LoadFactions()
        {
            var factions = await _database.GetAllFromCollection<FactionModel>("Factions");
            if (factions == null) return await Task.FromResult<List<FactionModel>>(new List<FactionModel> { });

            factions.ForEach(f =>
            {
                NAPI.Task.Run(() =>
                {
                    Blip b = NAPI.Blip.CreateBlip(84, f.SpawnPos, 1f, 0, f.name);
                    b.ShortRange = true;

                    GarageModel garage = new GarageModel
                    {
                        FactionGarage = true,
                        Position = f.GaragePos,
                        ParkOutPosition = f.ParkOutPos,
                        ParkOutRotation = f.ParkOutRot,
                        Name = f.name + " Garage",
                        Rotation = new Vector3()
                    };

                    CColShape shape = (CColShape)NAPI.ColShape.CreateCylinderColShape(f.GaragePos, 2f, 2f, 0);
                    shape.ShapeName = garage.Name;
                    shape.ShapeFunction = LoadFactionVehicles;  
                });
            });

            return await Task.FromResult<List<FactionModel>>(factions);
        }

        private async void LoadFactionVehicles(CPlayer player)
        {
            if (player == null) return;
            if (player.CurrentShape == null) return;
            if (!player.CurrentShape.ShapeName.Contains("Garage")) return;

            var factionName = player.CurrentShape.ShapeName.Split(" ")[0];
            if (factionName != player.DBModel.Faction.name) return;

            List<VehicleModel> ParkInVehicles = _database.GetAllFromCollection<VehicleModel>("Faction_Vehicles")
                                                            .Result
                                                            .Where(f => f.OwnerFaction == player.DBModel.Faction.name && f.IsParked)
                                                            .ToList();

            List<CVehicle> Vehicles = _pools.GetAllCVehicles()
                                            .Where(v => v.DBModel.OwnerFaction == factionName && !v.DBModel.IsParked && v.Position.DistanceTo(player.Position) <= 25f)
                                            .ToList();

            List<VehicleModel> ParkedOutVehicles = new List<VehicleModel>();

            if (Vehicles != null)
            {
                Vehicles?.ForEach(v =>
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
                        IsParked = false,
                        OwnerFaction = v.DBModel.OwnerFaction
                    };

                    ParkedOutVehicles.Add(model);
                });
            }
            

            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "Client:CreateGarage", "[" + NAPI.Util.ToJson(ParkedOutVehicles) + ", " + NAPI.Util.ToJson(ParkInVehicles) + "]", 0, factionName);
            });

        }
    }
}
