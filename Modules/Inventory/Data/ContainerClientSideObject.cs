using Backend.Handlers.Inventory.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Backend.Modules.Inventory.Data
{
    public class ContainerClientSlotObject
    {
        public int SlotID { get; set; }
        public Item Item { get; set; } = new Item();
        public int ItemCount { get; set; }

        public ContainerClientSlotObject(int slotID, Item item, int itemCount) {
            this.SlotID = slotID;
            this.Item = item;
            this.ItemCount = itemCount;
        }
    }
}
