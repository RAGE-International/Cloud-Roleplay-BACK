using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Modules.Garage.Data
{
    public class GarageVehicleObject
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public int Health { get; set; }
        public double Fuel { get; set; }
        public string NumberPlate { get; set; }
        public double Km { get; set; }
        public int TrunkWeight { get; set; }
        public string Note { get; set; }
        public bool IsParked { get; set; }
    }
}
