using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Core.Factories.CVehicle.Data
{
    public class VehicleData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
        public int MaxWeight { get; set; } = 0;
        public int Slots { get; set; } = 0;
    }
}
