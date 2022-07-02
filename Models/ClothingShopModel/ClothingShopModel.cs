using GTANetworkAPI;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models.ClothingShopModel
{
    public class ClothingShopModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public Vector3 Position { get; set; }
        public string Name { get; set; }
    }
}