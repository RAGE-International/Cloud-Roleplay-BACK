using Backend.Core.Database;
using Backend.Core.Factories.CColShape;
using Backend.Core.Factories.CPlayer;
using Backend.Models.StorageRoom;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Modules.StorageRoom
{
    public class StorageRoomModule : Script
    {
        private readonly CDBCLient _database;
        public static List<StorageRoomModel> StorageRooms { get; set; } = new List<StorageRoomModel>();

        public StorageRoomModule()
        {
            _database = new CDBCLient();

        }

        [ServerEvent(Event.ResourceStart)]
        public void LoadStorageRooms()
        {
            var storagesFromDB = _database.GetAllFromCollection<StorageRoomModel>("StorageRooms").GetAwaiter().GetResult();
            if (storagesFromDB == null) return;

            NAPI.Task.Run(() =>
            {
                storagesFromDB.ForEach(s =>
                {
                    if (s != null)
                    {
                        StorageRooms.Add(s);
                        CColShape shape = (CColShape)NAPI.ColShape.CreateCylinderColShape(s.Position, 2f, 2f, 0);
                        shape.ShapeName = "StorageRoom";
                        shape.ShapeFunction = Return;
                    }
                });
            });

            Console.WriteLine(storagesFromDB.Count + " Storages loaded");
        }

        private void Return(CPlayer player)
        {
            return;
        }
    }
}
