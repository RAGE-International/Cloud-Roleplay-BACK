using GTANetworkAPI;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Modules.Garage.Data
{
    public class GarageModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string VehId { get; set; }
        public string Name { get; set; }
        public bool FactionGarage { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 ParkOutPosition { get; set; }
        public Vector3 ParkOutRotation { get; set; }
    }
}
