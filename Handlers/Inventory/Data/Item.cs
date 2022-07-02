using Backend.Core.Database;
using Backend.Handlers.Inventory.Data.Items;
using GTANetworkAPI;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Handlers.Inventory.Data
{
    public class Item
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; } // In Gramm
        public int MaxCount { get; set; }
        public string Image { get; set; }
        public bool Is_Weapon { get; set; }
        public bool Is_Food { get; set; }
        public string ItemScript { set; get; }
    }

    public class ItemData : Script
    {
        private readonly CDBCLient _database;
        public List<Item> Items;
        public static List<IScriptItems> ScriptItems = new List<IScriptItems>();

        public ItemData()
        {
            _database = new CDBCLient();
            Items = new List<Item>();

            //AddExampleItem();
            this.LoadItems();
        }

        private async void AddExampleItem()
        {
            var item = new Item
            {
                Name = "Schutzweste",
                Weight = 1000,
                MaxCount = 8,
                Image = "Schutzweste",
                Is_Weapon = false,
                Is_Food = false,
                ItemScript = "Schutzweste"
            };

            await _database.AddOneToCollection<Item>("Items", item);
        }

        private async void LoadItems()
        {
            var items = _database.GetAllFromCollection<Item>("Items").Result;
            if (items == null) return;

            items.ForEach(i =>
            {
                Items.Add(i);
            });

            Console.WriteLine(items.Count);
        }
    }
}
