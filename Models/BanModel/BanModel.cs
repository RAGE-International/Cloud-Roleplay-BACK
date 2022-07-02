using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models.BanModel
{
    public class BanModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string DbId { get; set; }
        public string Reason { get; set; }
        public string BannedBy { get; set; }
        public DateTime BanTime { get; set; } = DateTime.UtcNow;
        public string HWID { get; set; }
        public ulong SocialClubID { get; set; }
    }
}
