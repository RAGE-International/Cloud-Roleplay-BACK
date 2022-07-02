using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Models.SmartphoneModel.Apps;
using Backend.Modules.Inventory.Data;
using GTANetworkAPI;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Models.PlayerModel
{
    public class PlayerModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int PlayerId { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string HWID { get; set; } = string.Empty;
        public ulong SocialID { get; set; } = 0;
        public bool IsBanned { get; set; } = false;
        public int AdminLevel { get; set; } = 0;
        public int Money { get; set; } = 250000;
        public int BankNumber { get; set; } = new Random().Next(10000000, 19999999);
        public int BankMoney { get; set; } = 150000;
        public Vector3 Position { get; set; } = new Vector3();
        public int InventoryId { get; set; } = 0;
        public int PhoneNumber { get; set; } = 0;
        public string DiscordId { get; set; } = string.Empty;
        public string AvatarURL { get; set; } = string.Empty;
        public bool IsLinked { get; set; } = false;
        public int Health { get; set; } = 100;
        public int Armor { get; set; } = 0;
        public InjuryModel.InjuryModel InjuryModel { get; set; } = new InjuryModel.InjuryModel();
        public List<string> Weapons { get; set; } = new List<string>();
        public ClothingModel.ClothingModel ClothingModel { get; set; } = new ClothingModel.ClothingModel();
        public PlayerFactionModel Faction { get; set; } = new PlayerFactionModel(0, "Zivilist", 0, false, false, false);
        public SmartphoneSettingsModel SmartPhoneSettings { get; set; } = new SmartphoneSettingsModel();

    }

    public class PlayerFactionModel
    {
        public PlayerFactionModel(int Id, string Name, int rank, bool leader, bool manager, bool onDuty)
        {
            id = Id;
            name = Name;
            Rank = rank;
            Leader = leader;
            Manager = manager;
            OnDuty = onDuty;
        }

        public int id { get; set; } = 0;
        public string name { get; set; } = "Zivilist";
        public int Rank { get; set; } = 0;
        public bool Leader { get; set; } = false;
        public bool Manager { get; set; } = false;
        public bool OnDuty { get; set; } = false;
    }

}
