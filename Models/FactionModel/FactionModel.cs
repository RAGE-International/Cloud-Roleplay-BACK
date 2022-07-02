using Backend.Core.Factories.CPlayer;
using Backend.Models.StorageRoom;
using Backend.Modules.Inventory.Data;
using GTANetworkAPI;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models.FactionModel
{
    public class FactionModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DbId { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string ColorHex { get; set; }
        public Vector3 SpawnPos { get; set; } = new Vector3();
        public Vector3 GaragePos { get; set; } = new Vector3();
        public Vector3 ParkOutPos { get; set; } = new Vector3();
        public Vector3 ParkOutRot { get; set; } = new Vector3();
        
    }
}
