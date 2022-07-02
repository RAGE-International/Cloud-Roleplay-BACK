using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Core.Factories.CVehicle;
using Backend.Core.Factories.Pools;
using Backend.Models.StorageRoom;
using Backend.Modules.Inventory.Data;
using Backend.Modules.StorageRoom;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backend.Handlers.Inventory
{
    public class InventoryHandler : Script
    {
        private readonly CDBCLient _database;
        private readonly Pools _pools = new Pools();
        private readonly StorageRoomModule _storage;

        public InventoryHandler()
        {
            _database = new CDBCLient();
            _pools = new Pools();
            _storage = new StorageRoomModule();
        }

        public void ShowInventory(CPlayer player, bool state)
        {
            if (player == null) return;

            if (!state)
            {
                player.TriggerEvent("Client:Inventory:HideInventory");
                return;
            }

            List<InventoryModel> inventories = new List<InventoryModel>();
            inventories.Add(player.Inventory);

            CVehicle veh = _pools.GetAllCVehicles().FirstOrDefault(v => v.Position.DistanceTo(player.Position) <= 3f);
            
            if (veh != default && !veh.TrunkLocked)
            {
                inventories.Add(veh.Inventory);
            }

            if (player.CurrentShape != null && player.CurrentShape?.ShapeName == "StorageRoom")
            {

                Console.WriteLine(StorageRoomModule.StorageRooms.Count);

                StorageRoomModel room = StorageRoomModule.StorageRooms.FirstOrDefault(s => s.Position.DistanceTo(player.Position) <= 2f);
                if (room != null && room.FactionName == player.DBModel.Faction.name || room != null && room.OwnerId == player.DBModel.Id)
                {

                    Console.WriteLine(StorageRoomModule.StorageRooms.Count + " s");

                    var inv = _database.GetOneFromCollection<InventoryModel>("Inventories", i => i.ExternalContainerID == room.ExternalContainerId).GetAwaiter().GetResult();
                    if (inv != null)
                        inventories.Add(inv);
                }
            }

            Console.WriteLine(NAPI.Util.ToJson(inventories));

            player.TriggerEvent("Client:Inventory:TriggerInventory", player.Name, player.DBModel.Money, NAPI.Util.ToJson(inventories));
            
                
        }

        public bool HasItem(CPlayer player, string itemName)
        {
            if (player == null) return false;
            if (itemName == "") return false;

            var inventory = player.Inventory;
            if (inventory == null) return false;

            var foundItem = inventory.Slots.FirstOrDefault(i => i.Item?.Name == itemName);
            if (foundItem == null) return false;

            return true;
        }
    }
}
