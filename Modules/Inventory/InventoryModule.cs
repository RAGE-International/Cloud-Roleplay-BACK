using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Core.Factories.CVehicle;
using Backend.Handlers.Inventory.Data;
using Backend.Models.NotificationModel;
using Backend.Models.PlayerModel;
using Backend.Modules.Inventory.Data;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Modules.Inventory
{
    public class InventoryModule : Script
    {
        private readonly CDBCLient _database;
        public List<Item> _items;
        public InventoryModule()
        {
            _database = new CDBCLient();
            _items = new List<Item>();

            //1: Player 2: Rucksack 3: Autokofferraum 4: Lagerhalle

            NAPI.ClientEvent.Register<CPlayer, int, int, int, int, string, string, int, int, int, int>("Server:Inventory:MoveItem", this, MoveItem);
            NAPI.ClientEvent.Register<CPlayer, int, int, int>("Server:Inventory:GiveItem", this, GiveItem);
            NAPI.ClientEvent.Register<CPlayer>("Server:CloseInventory", this, OnInventoryClose);
            NAPI.ClientEvent.Register<CPlayer, int, int, int>("Server:Inventory:ThrowItem", this, ThrowItem);
            NAPI.ClientEvent.Register<CPlayer, int, int, int>("Server:Inventory:UseItem", this, UseItem);
            NAPI.ClientEvent.Register<CPlayer, int>("Server:Inventory:UseItemOnSlot", this, UseItemOnSlot);
            NAPI.ClientEvent.Register<CPlayer, int, bool>("Server:Clothing:SetPlayerClothing", this, SetPlayerClothes);
        }

        private void OnInventoryClose(CPlayer player)
        {
            if (player == null) return;
            if (player.InteractingWith != null)
                player.InteractingWith = null;
        }

        public async Task CreateInventory(System.Object obj)
        {
            if (obj == null) return;

            var type = 0;

            if (obj.GetType() == typeof(CPlayer))
                type = 1;
            else if (obj.GetType() == typeof(CVehicle))
                type = 3;

            var inventoryToAdd = new InventoryModel(type, 50000, 25, _database.GetAllFromCollection<InventoryModel>("Inventories").Result.Count + 1, type);
            if (inventoryToAdd == null) return;

            await _database.AddOneToCollection<InventoryModel>("Inventories", inventoryToAdd);

            if (obj.GetType() == typeof(CPlayer))
            {
                CPlayer player = (CPlayer)obj;
                player.DBModel.InventoryId = inventoryToAdd.ExternalContainerID;

                await player.Update();
            }
            
            else if (obj.GetType() == typeof(CVehicle))
            {
                CVehicle vehicle = (CVehicle)obj;
                vehicle.DBModel.InventoryId = inventoryToAdd.ExternalContainerID;
                vehicle.Update();
            }
        }

        private void SetPlayerClothes(CPlayer player, int componentId, bool isAccessories)
        {
            var clothing = player.DBModel.ClothingModel?.clothingComponents?.FirstOrDefault(i => i.Id == componentId && i.IsProp == isAccessories);
            if (clothing == null) return;

            bool isMale = true;
            if (player.Model == ((uint)PedHash.FreemodeFemale01))
                isMale = false;

            if (isAccessories)
            {
                switch (componentId)
                {
                    case 0:
                        if (isMale)
                        {
                            if (player.GetAccessoryDrawable(componentId) == 8)
                                player.SetAccessories(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetAccessories(componentId, 8, 0);
                        }
                        else
                        {
                            if (player.GetAccessoryDrawable(componentId) == 57)
                                player.SetAccessories(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetAccessories(componentId, 57, 0);
                        }
                        break;
                    case 1:
                        if (isMale)
                        {
                            if (player.GetAccessoryDrawable(componentId) == 0)
                                player.SetAccessories(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetAccessories(componentId, 0, 0);
                        }
                        else
                        {
                            if (player.GetAccessoryDrawable(componentId) == 5)
                                player.SetAccessories(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetAccessories(componentId, 5, 0);
                        }
                        break;
                    case 2:
                        if (isMale)
                        {
                            if (player.GetAccessoryDrawable(componentId) == 65535)
                                player.SetAccessories(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetAccessories(componentId, -1, -1);
                        }
                        else
                        {
                            if (player.GetAccessoryDrawable(componentId) == 65535)
                                player.SetAccessories(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetAccessories(componentId, -1, -1);
                        }
                        break;
                    case 6:
                        if (isMale)
                        {
                            if (player.GetAccessoryDrawable(componentId) == 65535)
                                player.SetAccessories(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetAccessories(componentId, -1, -1);
                        }
                        else
                        {
                            if (player.GetAccessoryDrawable(componentId) == 1)
                                player.SetAccessories(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetAccessories(componentId, 1, 0);
                        }
                        break;
                    case 7:
                        if (isMale)
                        {
                            if (player.GetAccessoryDrawable(componentId) == 0)
                                player.SetAccessories(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetAccessories(componentId, 0, 10);
                        }
                        else
                        {
                            if (player.GetAccessoryDrawable(componentId) == 0)
                                player.SetAccessories(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetAccessories(componentId, 0, 10);
                        }
                        break;
                }
            }
            else
            {
                switch (componentId)
                {
                    case 1:
                        if (player.GetClothesDrawable(componentId) == 0)
                            player.SetClothes(componentId, clothing.Drawable, clothing.Texture);
                        else
                            player.SetClothes(componentId, 0, 0);
                        break;
                    case 3:
                        if (isMale)
                        {
                            if (player.GetClothesDrawable(componentId) == 15)
                                player.SetClothes(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetClothes(componentId, 15, 0);
                        }
                        else
                        {
                            if (player.GetClothesDrawable(componentId) == 15)
                                player.SetClothes(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetClothes(componentId, 15, 0);
                        }
                        break;
                    case 4:
                        if (isMale)
                        {
                            if (player.GetClothesDrawable(componentId) == 21)
                                player.SetClothes(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetClothes(componentId, 21, 0);
                        }
                        else
                        {
                            if (player.GetClothesDrawable(componentId) == 15)
                                player.SetClothes(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetClothes(componentId, 15, 0);
                        }
                        break;
                    case 5:
                        if (player.GetClothesDrawable(componentId) == 0)
                            player.SetClothes(componentId, clothing.Drawable, clothing.Texture);
                        else
                            player.SetClothes(componentId, 0, 0);

                        break;
                    case 6:
                        if (isMale)
                        {
                            if (player.GetClothesDrawable(componentId) == 34)
                                player.SetClothes(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetClothes(componentId, 34, 0);
                        }
                        else
                        {
                            if (player.GetClothesDrawable(componentId) == 35)
                                player.SetClothes(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetClothes(componentId, 35, 0);
                        }
                        break;
                    case 7:
                        if (isMale)
                        {
                            if (player.GetClothesDrawable(componentId) == 0)
                                player.SetClothes(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetClothes(componentId, 0, 0);
                        }
                        else
                        {
                            if (player.GetClothesDrawable(componentId) == 0)
                                player.SetClothes(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetClothes(componentId, 0, 0);
                        }
                        break;
                    case 8:
                        if (isMale)
                        {
                            if (player.GetClothesDrawable(componentId) == 15)
                                player.SetClothes(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetClothes(componentId, 15, 0);
                        }
                        else
                        {
                            if (player.GetClothesDrawable(componentId) == 2)
                                player.SetClothes(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetClothes(componentId, 2, 0);
                        }
                        break;
                    case 9:
                        if (isMale)
                        {
                            if (player.GetClothesDrawable(componentId) == 0)
                                player.SetClothes(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetClothes(componentId, 0, 0);
                        }
                        else
                        {
                            if (player.GetClothesDrawable(componentId) == 0)
                                player.SetClothes(componentId, clothing.Drawable, clothing.Texture);
                            else
                                player.SetClothes(componentId, 0, 0);
                        }
                        break;
                    case 11:
                        if (isMale)
                        {
                            if (player.GetClothesDrawable(componentId) == 15)
                            {
                                player.SetClothes(componentId, clothing.Drawable, clothing.Texture);
                                player.SetClothes(3, clothing.Drawable, clothing.Texture);
                            }
                            else
                            {
                                player.SetClothes(componentId, 15, 0);
                                player.SetClothes(3, 15, 0);
                            }
                        }
                        else
                        {
                            if (player.GetClothesDrawable(componentId) == 15)
                            {
                                player.SetClothes(componentId, clothing.Drawable, clothing.Texture);
                                player.SetClothes(3, clothing.Drawable, clothing.Texture);
                            }
                            else
                            {
                                player.SetClothes(componentId, 15, 0);
                                player.SetClothes(3, 15, 0);
                            }
                        }
                        break;
                }
            }
        }

        private void UseItemOnSlot(CPlayer player, int slot)
        {
            if (player == null) return;
            if (slot > 1) return;

            UseItem(player, 1, slot, 1);
        }
        private void UseItem(CPlayer player, int container, int slot, int amount)
        {
            Console.WriteLine("1");
            if (player == null) return;
            if (container != 1) return;

            amount = 1;
            
            var inventory = _database.GetOneFromCollection<InventoryModel>("Inventories", i => i.ExternalContainerID == player.DBModel.InventoryId).Result;
            if (inventory == null) return;
            Console.WriteLine("2");

            var item = inventory.Slots[slot].Item;
            if (item == null) return;

            if (item.Is_Weapon)
            {
                WeaponHash weapon = new WeaponHash();
                foreach (WeaponHash w in Enum.GetValues(typeof(WeaponHash)))
                {
                    if (w.ToString().ToLower() == item.Name.ToLower())
                    {
                        weapon = w;
                        break;
                    }
                }
                if (weapon == new WeaponHash()) return;

                Console.WriteLine(item.Name);
                player.GiveACWeapon(item.Name);
                RemoveItem(slot, inventory, 1);
                NAPI.ClientEvent.TriggerClientEvent(player, "Client:Inventory:HideInventory");
                return;

            }
            
            Console.WriteLine(item.ItemScript);

            var itemFunction = ItemData.ScriptItems.FirstOrDefault(i => i.Name == item.ItemScript);
            if (itemFunction == null) return;

            NAPI.ClientEvent.TriggerClientEvent(player, "Client:Inventory:HideInventory");

            Console.WriteLine("4");

            itemFunction.Function.Invoke(player, inventory, slot, amount);
        }

        private async void ThrowItem(CPlayer player, int container, int slot, int amount)
        {
            var inventory = await _database.GetOneFromCollection<InventoryModel>("Inventories", i => i.ExternalContainerID == player.DBModel.InventoryId);
            if (inventory == null || container != 1) return;

            var itemToThrow = inventory.Slots[slot];
            if (itemToThrow.Item == null) return;
            if (itemToThrow.ItemCount - amount <= 0)
            {
                itemToThrow.Item = null;
                itemToThrow.ItemCount = 0;
            }
            else
            {
                itemToThrow.ItemCount -= amount;
            }

            await _database.Update<InventoryModel>("Inventories", inventory, i => i.ExternalContainerID == player.DBModel.InventoryId);

            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "Client:Inventory:HideInventory");
            });
            player.SendCloudNotification("Inventar", "Du hast etwas weggeworfen!", 2500, NotificationModel.SUCCESS, false);
        }

        private async void GiveItem(CPlayer player, int container, int slot, int amount)
        {
            if (player == null) return;

            Console.WriteLine(container + " " + slot + " " + amount);

            if (container < 0) return;
            if (slot < 0) return;  
            if (amount < 0) return;
            if (player.InteractingWith == null) return;

            var inventory = await _database.GetOneFromCollection<InventoryModel>("Inventories", i => i.ExternalContainerID == player.DBModel.InventoryId);
            if (inventory == null) return;

            var itemToGive = inventory.Slots[slot].Item;
            if (itemToGive == null) return;

            if (amount > inventory.Slots[slot].ItemCount) return;

            var inventoryToAdd = await _database.GetOneFromCollection<InventoryModel>("Inventories", i => i.ExternalContainerID == player.InteractingWith.DBModel.InventoryId);
            if (inventoryToAdd == null) return;

            Console.WriteLine("4");

            if (inventoryToAdd.Slots.TrueForAll(i => i.Item != null))
            {
                NAPI.Task.Run(() =>
                {
                    NAPI.ClientEvent.TriggerClientEvent(player, "Client:Inventory:HideInventory");
                });
                player.SendCloudNotification("Inventar", "Das Inventar des anderen Spielers ist voll!", 2500, NotificationModel.ERROR, false);
                return;
            }

            if ((inventoryToAdd.CurrentWeight + (itemToGive.Weight * amount)) > inventoryToAdd.MaxWeight)
            {
                NAPI.Task.Run(() =>
                {
                    NAPI.ClientEvent.TriggerClientEvent(player, "Client:Inventory:HideInventory");
                });
                player.SendCloudNotification("Inventar", "Das wäre zu schwer!", 2500, NotificationModel.ERROR, false);
                return;
            }

            var slotToAdd = -1;
            
            

            var isAlreadyInInventory = inventoryToAdd.Slots.FirstOrDefault(i => i.Item?.Name == itemToGive?.Name && i.Item != null && i.ItemCount != i.Item?.MaxCount);
            Console.WriteLine(isAlreadyInInventory?.SlotID);
            if (isAlreadyInInventory != null && isAlreadyInInventory.ItemCount != isAlreadyInInventory.Item.MaxCount)
            {
                if (isAlreadyInInventory.ItemCount + amount > isAlreadyInInventory.Item.MaxCount)
                {
                    var amountToAdd = isAlreadyInInventory.Item.MaxCount - isAlreadyInInventory.ItemCount;

                    isAlreadyInInventory.ItemCount += amountToAdd;
                    var nextFreeSlot = inventoryToAdd.Slots.FirstOrDefault(i => i.Item == null);
                    if (nextFreeSlot == null)
                    {
                        player.SendCloudNotification("Inventar", "Dafür ist zu wenig Platz", 2500, NotificationModel.ALERT, false);
                        NAPI.Task.Run(() =>
                        {
                            NAPI.ClientEvent.TriggerClientEvent(player, "Client:Inventory:HideInventory");
                        });
                        return;
                    }

                    amountToAdd = amount - amountToAdd;
                    inventoryToAdd.Slots[nextFreeSlot.SlotID].Item = itemToGive;
                    inventoryToAdd.Slots[nextFreeSlot.SlotID].ItemCount = amountToAdd;
                }
                else
                {
                    isAlreadyInInventory.ItemCount += amount;
                }
            }
            else
            {
                var firstFreeSlot = inventoryToAdd.Slots.FirstOrDefault(i => i.Item == null).SlotID;
                if (firstFreeSlot == null) return;

                inventoryToAdd.Slots[firstFreeSlot].Item = itemToGive;
                inventoryToAdd.Slots[firstFreeSlot].ItemCount = amount;
            }


            if (inventory.Slots[slot].ItemCount - amount <= 0)
            {
                inventory.Slots[slot].Item = null;
                inventory.Slots[slot].ItemCount = 0;
            }
            else
                inventory.Slots[slot].ItemCount -= amount;

            await _database.Update<InventoryModel>("Inventories", inventory, i => i.ExternalContainerID == player.DBModel.InventoryId);
            await _database.Update<InventoryModel>("Inventories", inventoryToAdd, i => i.ExternalContainerID == player.InteractingWith.DBModel.InventoryId);

            player.SendCloudNotification("Inventar", "Du hast erfolgreich ein Item übergeben!", 2500, NotificationModel.SUCCESS, false);
            player.InteractingWith.SendCloudNotification("Inventar", "Du hast etwas erhalten!", 2500, NotificationModel.SUCCESS, false);
            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "Client:Inventory:HideInventory");
            });
        }


        private async void MoveItem(CPlayer player, int currentSlot, int newSlot, int currentContainer, int newContainer, string oldItemID, string newItemID, int externalOldID, int externalOldType, int externalNewID, int externalNewType)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            InventoryModel oldInventory = await _database.GetOneFromCollection<InventoryModel>("Inventories", i => i.ExternalContainerID == externalOldID);
            if (oldInventory == null) return;

            var newInventory = oldInventory;

            if(currentContainer != newContainer)
            {
                newInventory = await _database.GetOneFromCollection<InventoryModel>("Inventories", i => i.ExternalContainerID == externalNewID);
                if (newInventory == null) return;
            }

            var itemOutOfInventory = oldInventory.Slots.FirstOrDefault(i => i.SlotID == currentSlot);
            if (itemOutOfInventory == null) return;

            if (currentContainer != newContainer) 
            {
                if (newInventory.CurrentWeight + itemOutOfInventory.Item.Weight > newInventory.MaxWeight)
                {
                    player.SendCloudNotification("INVENTORY", "Scheint als ob das zu viel wiegt!", 2500, NotificationModel.ERROR, false);
                    return;
                }

                if (newInventory.Slots.TrueForAll(i => i.Item != null))
                {
                    player.SendCloudNotification("INVENTORY", "Dafür ist kein Platz mehr!", 2500, NotificationModel.ERROR, false);
                    return;
                }
            }
            

            if (newInventory.Slots[newSlot].Item != null && newInventory.Slots[newSlot].Item?.Name == itemOutOfInventory.Item?.Name && newInventory.Slots[newSlot].ItemCount < newInventory.Slots[newSlot]?.Item.MaxCount)
            {
                if (newInventory.Slots[newSlot].ItemCount + itemOutOfInventory.ItemCount > newInventory.Slots[newSlot].Item.MaxCount)
                {
                    var amountToAdd = itemOutOfInventory.Item.MaxCount - newInventory.Slots[newSlot].ItemCount;
                    if (amountToAdd > 0)
                    {
                        newInventory.Slots[newSlot].ItemCount += amountToAdd;
                        amountToAdd = itemOutOfInventory.ItemCount - amountToAdd;
                        if (amountToAdd == 0)
                        {
                            itemOutOfInventory.Item = null;
                            itemOutOfInventory.ItemCount = 0;
                        }
                        else
                        {
                            itemOutOfInventory.ItemCount -= amountToAdd;
                        }
                    }
                }
                else
                {
                    newInventory.Slots[newSlot].ItemCount += itemOutOfInventory.ItemCount;
                    itemOutOfInventory.Item = null;
                    itemOutOfInventory.ItemCount = 0;
                }
            }
            else
            {
                newInventory.Slots[newSlot].Item = itemOutOfInventory.Item!;
                newInventory.Slots[newSlot].ItemCount = itemOutOfInventory.ItemCount;

                oldInventory.Slots[currentSlot].Item = null;
                oldInventory.Slots[currentSlot].ItemCount = 0;
            }

            await _database.Update<InventoryModel>("Inventories", oldInventory, i => i.ExternalContainerID == oldInventory.ExternalContainerID);
            if (currentContainer != newContainer)
                await _database.Update<InventoryModel>("Inventories", newInventory, i => i.ExternalContainerID == newInventory.ExternalContainerID);

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        public async void RemoveItem(int slot, InventoryModel inventory, int amount) 
        {
            if (inventory == null) return;
            if (inventory.Slots[slot].Item == null) return;

            var itemToThrow = inventory.Slots[slot];
            if (itemToThrow.Item == null) return;
            if (itemToThrow.ItemCount - amount <= 0)
            {
                itemToThrow.Item = null;
                itemToThrow.ItemCount = 0;
            }
            else
            {
                itemToThrow.ItemCount -= amount;
            }

            await _database.Update<InventoryModel>("Inventories", inventory, i => i.ExternalContainerID == inventory.ExternalContainerID);
        }

        public bool AddItem(System.Object obj, Item item, int amount)
        {
            if (obj == null) return false;

            if (obj.GetType() == typeof(CPlayer))
            {
                var player = (CPlayer)obj;
                if (player == null) return false;

                var inventory = _database.GetOneFromCollection<InventoryModel>("Inventories", i => i.ExternalContainerID == player.DBModel.InventoryId).Result;
                if (inventory == null) return false;

                if (item.Weight * amount + inventory.CurrentWeight > inventory.MaxWeight)
                {
                    player.SendCloudNotification("INVENTORY", "Das wäre zu schwer!", 2500, NotificationModel.ERROR, false);
                    return false;
                }

                var firstFreeSlot = inventory.Slots.FirstOrDefault(i => i.Item == null).SlotID;
                if (firstFreeSlot == null) 
                {
                    player.SendCloudNotification("INVENTORY", "Inventar voll!", 2500, NotificationModel.ERROR, false);
                    return false;
                }

                inventory.Slots[firstFreeSlot].Item = item;
                inventory.Slots[firstFreeSlot].ItemCount = amount;

                _database.Update<InventoryModel>("Inventories", inventory, i => i.ExternalContainerID == inventory.ExternalContainerID).GetAwaiter();

                return true;
            }

            if (obj.GetType() == typeof(CVehicle))
            {
                var vehicle = (CVehicle)obj;
                if (vehicle == null) return false;

                var vehicleInventory = vehicle.Inventory;
                if (vehicleInventory == null) return false;

                var firstFreeSlot = vehicleInventory.Slots.FirstOrDefault(i => i.Item == null).SlotID;
                if (firstFreeSlot == -1) return false;

                vehicleInventory.Slots[firstFreeSlot].Item = item;
                vehicleInventory.Slots[firstFreeSlot].ItemCount = amount;

                _database.Update<InventoryModel>("Inventories", vehicleInventory, i => i.ContainerID == vehicleInventory.ContainerID).GetAwaiter();

                return true;
            }

            return true;
            //TODO: LAGERHALLE UND SO BLA BLA
        }
    }
}
