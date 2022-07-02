using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models.Ammunation.Data
{
    public class AmmunationItemModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int Category { get; set; }
        public int ItemPrice { get; set; }
        public int ItemId { get; set; }
        public string ItemImage { get; set; } = "gusenberg";
    }
}
