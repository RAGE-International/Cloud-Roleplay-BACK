using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Backend.Modules.Inventory.Data
{
    public class InventoryModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public int ContainerID { get; set; }
        public int MaxSlots { get; set; }
        public int MaxWeight { get; set; }
        public int CurrentWeight 
        { 
            get
            {
                int weight = 0;
                Slots.ForEach(i =>
                {
                    if (i.Item != null)
                        weight += i.Item.Weight * i.ItemCount;
                });
                return weight;
            }
            set
            {
                TempInv.Weight = value;
            }
        }
        public List<ContainerClientSlotObject> Slots { get; set; }

        public InventoryModel(int containerId, int maxWeight, int currentWeight = 0, int maxSlots = 25, int externalContainerId = 0, int externalContainerType = 0)
        {
            this.ContainerID = containerId;
            this.MaxWeight = maxWeight;
            this.MaxSlots = maxSlots;
            this.ExternalContainerID = externalContainerId;
            this.ExternalContainerType = externalContainerType;

            var slots = new List<ContainerClientSlotObject>();
            for (int i = 0; i < maxSlots; i++)
            {
                slots.Add(new ContainerClientSlotObject(i, null, 0));
            }

            this.Slots = slots;
        }

        [NotMapped]
        public int ExternalContainerID { get; set; }

        [NotMapped]
        public int ExternalContainerType { get; set; }

    }

    public class TempInv
    {
        public static int Weight { get; set; }
    }
}
