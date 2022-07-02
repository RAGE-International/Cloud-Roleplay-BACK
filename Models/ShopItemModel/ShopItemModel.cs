using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models.ShopItemModel
{
    public class ShopItemModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DbId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
    }
}
