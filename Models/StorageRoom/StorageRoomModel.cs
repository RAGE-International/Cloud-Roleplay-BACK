using Backend.Modules.Inventory.Data;
using GTANetworkAPI;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models.StorageRoom
{
    public class StorageRoomModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public Vector3 Position { get; set; } = new Vector3();
        public Vector3 EnterPosition { get; set; } = new Vector3();
        public bool IsFactionStorage { get; set; } = false;
        public string FactionName { get; set; } = "";
        public int Dimension { get; set; } = 0;
        public int ExternalContainerId { get; set; } = -1;
        public string OwnerId { get; set; } = "";
    }
}
