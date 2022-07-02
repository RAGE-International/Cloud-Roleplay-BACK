using Backend.Modules.Inventory.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models.VehicleModel
{
    public class VehicleModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string VehId { get; set; }
        public string DisplayName { get; set; } = "Revolter";
        public int Health { get; set; } = 100;
        public double Fuel { get; set; } = 0;
        public string NumberPlate { get; set; } = "CLOUD";
        public double Km { get; set; } = 0;
        public int TrunkWeight { get; set; } = 0;
        public string Note { get; set; } = "";
        public bool IsParked { get; set; } = true;
        public string OwnerFaction { get; set; } = null;
        public string OwnerId { get; set; }
        public string[] CoOwners { get; set; } = new string[4];
        public int InventoryId { get; set; }
    }
}
