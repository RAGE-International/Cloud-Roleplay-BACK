using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Core.Factories.CVehicle;
using Backend.Handlers.Inventory;
using Backend.Models.NotificationModel;
using Backend.Modules.Garage.Data;
using Backend.Modules.Inventory;
using Backend.Modules.XMenu.Data;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Modules.XMenu
{
    public class XMenuModule : Script
    {
        private readonly CDBCLient _database;
        private readonly InventoryHandler _inventory;

        public XMenuModule()
        {
            _database = new CDBCLient();
            _inventory = new InventoryHandler();

            NAPI.ClientEvent.Register<CPlayer, CVehicle>("ClientToggleVehicleLockState", this, OnVehicleToggleLockState);
            NAPI.ClientEvent.Register<CPlayer, CVehicle>("ClientToggleVehicleTrunkState", this, OnVehicleToggleTrunkState);
            NAPI.ClientEvent.Register<CPlayer, CVehicle>("ClientVehiclePark", this, OnVehicleParkIn);
            NAPI.ClientEvent.Register<CPlayer>("ClientToggleVehicleEngine", this, OnVehicleToggleEngine);
            NAPI.ClientEvent.Register<CPlayer, CPlayer>("ClientGiveMoney", this, OnPlayerGiveMoney);
            NAPI.ClientEvent.Register<CPlayer, int>("CompleteGiveMoney", this, OnCompleteGiveMoney);
            NAPI.ClientEvent.Register<CPlayer>("CancelGiveMoney", this, OnCancelGiveMoney);
            NAPI.ClientEvent.Register<CPlayer, CPlayer>("ClientGiveItem", this, OnPlayerStartGiveItem);
        }

        private void OnPlayerStartGiveItem(CPlayer player, CPlayer target)
        {
            if (player == null) return;
            if (target == null) return;

            if (!player.LoggedIn) return;
            if (!target.LoggedIn) return;

            if (player.InteractingWith != null) return;

            player.InteractingWith = target;
            _inventory.ShowInventory(player, true);
        }

        private void OnCancelGiveMoney(CPlayer player)
        {
            if (player == null) return;
            player.InteractingWith = null;

            NAPI.ClientEvent.TriggerClientEvent(player, "Client:Dialog:Destory");
        }

        private void OnCompleteGiveMoney(CPlayer player, int moneyAmount)
        {
            if (player == null || moneyAmount <= 0) return;
            if (player.InteractingWith == null) return;

            if (player.DBModel.Money - moneyAmount < 0)
            {
                player.SendCloudNotification("Geldübergabe", "Du hast zu wenig Geld dabei!", 2500, NotificationModel.ALERT, false);
                return;
            }

            var target = player.InteractingWith;
            if (target == null) return;

            player.DBModel.Money -= moneyAmount;
            target.DBModel.Money += moneyAmount;

            player.Update();
            target.Update();

            player.SendCloudNotification("Geldübergabe", $"Du hast einem Spieler {moneyAmount}$ übergeben!", 2500, NotificationModel.SUCCESS, false);
            target.SendCloudNotification("Geldübergabe", $"Du hast {moneyAmount}$ erhalten!", 2500, NotificationModel.SUCCESS, false);

            NAPI.ClientEvent.TriggerClientEvent(player, "Client:SetMoney", player.DBModel.Money);
            NAPI.ClientEvent.TriggerClientEvent(target, "Client:SetMoney", target.DBModel.Money);

            NAPI.ClientEvent.TriggerClientEvent(player, "Client:Dialog:Destory");
            player.InteractingWith = null;
        }

        private void OnPlayerGiveMoney(CPlayer player, CPlayer target)
        {
            if (player == null || target == null) return;
            if (player.InteractingWith != null) return;
            player.InteractingWith = target;

            DialogModel dialog = new DialogModel
            {
                title = "Geldübergabe",
                discription = "Gebe einem anderen Spieler Geld!",
                buttons = new List<DialogButtonObject>
                {
                    new DialogButtonObject
                    {
                        name = "Abbrechen",
                        eventname = "CancleGiveMoney",
                        arguments = new object[]{}
                    },
                    new DialogButtonObject
                    {
                        name = "Bestätigen",
                        eventname = "CompleteGiveMoney",
                        arguments = new object[]{}
                    }
                }
            };

            NAPI.ClientEvent.TriggerClientEvent(player, "Client:DialogInput:Create", NAPI.Util.ToJson(dialog));
        }

        private void OnVehicleToggleEngine(CPlayer player)
        {
            if (player == null) return;

            var vehicle = (CVehicle)player.Vehicle;
            if (vehicle == null) return;

            if (vehicle.DBModel.OwnerId == player.DBModel.Id || vehicle.DBModel.CoOwners.Contains(player.DBModel.Id) || vehicle.DBModel.OwnerFaction == player.DBModel.Faction.name)
            {
                vehicle.EngineStatus = !vehicle.EngineStatus;
                player.SendCloudNotification("FAHRZEUG", vehicle.EngineStatus ? "Du hast den Motor angemacht." : "Du hast den Motor ausgemacht.", 2500, NotificationModel.SUCCESS, false);
            }
        }

        public void OnVehicleToggleLockState(CPlayer player, CVehicle vehicle)
        {
            if (player == null) return;
            if (player.Vehicle != null)
                vehicle = (CVehicle)player.Vehicle;
            if (vehicle == null) return;

            Console.WriteLine(vehicle.DBModel.OwnerId + " " + player.DBModel.Id);

            //TODO: ADD FACTION
            if (vehicle.DBModel.OwnerId == player.DBModel.Id || vehicle.DBModel.CoOwners.Contains(player.DBModel.Id) || vehicle.DBModel.OwnerFaction == player.DBModel.Faction.name)
            {
                vehicle.Locked = !vehicle.Locked;
                player.SendCloudNotification("Fahrzeug", vehicle.Locked ? "Du hast dein Fahrzeug abgesperrt" : "Du hast dein Fahrzeug aufgesperrt", 2500, NotificationModel.SUCCESS, false);
            }
            if (vehicle.Locked)
                vehicle.TrunkLocked = true;

        }

        public void OnVehicleToggleTrunkState(CPlayer player, CVehicle vehicle)
        {
            if (player == null || vehicle == null) return;
            if (vehicle.Locked) return;

            vehicle.TrunkLocked = !vehicle.TrunkLocked;
            player.SendCloudNotification("FAHRZEUG", vehicle.TrunkLocked ? "Du hast den Kofferraum abgeschlossen." : "Du hast den Kofferraum aufgeschlossen", 2500, NotificationModel.SUCCESS, false);
        }

        private void OnVehicleParkIn(CPlayer player, CVehicle vehicle)
        {
            if (player == null || vehicle == null) return;
            if (vehicle.DBModel.OwnerId == player.DBModel.Id || vehicle.DBModel.CoOwners.Contains(player.DBModel.Id) || vehicle.DBModel.OwnerFaction == player.DBModel.Faction.name)
            {
                GarageModel garageToParkIn = null;
                if (vehicle.DBModel.OwnerFaction == "")
                    garageToParkIn = _database.GetAllFromCollection<GarageModel>("Garages").Result.FirstOrDefault(g => g.Position.DistanceTo(player.Position) <= 20f);
                else
                    garageToParkIn = _database.GetAllFromCollection<GarageModel>("Faction_Garages").Result.FirstOrDefault(g => g.Position.DistanceTo(player.Position) <= 20f);


                if (garageToParkIn == null || garageToParkIn == default)
                {
                    player.SendCloudNotification("GARAGE", "Keine Garage in deiner Nähe gefunden!", 2500, NotificationModel.ERROR, false);
                    return;
                }

                vehicle.DBModel.IsParked = true;
                vehicle.Update();

                player.SendCloudNotification("GARAGE", "Du hast dein Fahrzeug erfolgreich eingepark!", 2500, NotificationModel.SUCCESS, false);
                vehicle.Delete();
            }
        }
    }
}
