using Backend.Core.Database;
using Backend.Models.ClothingShopModel;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Factories.CColShape.Data
{
    public class ClothingShopShape : Script
    {
        private readonly CDBCLient _database;

        public ClothingShopShape()
        {
            _database = new CDBCLient();

            CreateClothingShopShapes();
            //CreateTestShape();
        }

        private void CreateTestShape()
        {
            var modelToAdd = new ClothingShopModel
            {
                Position = new Vector3(33.27184295654297, -1348.1585693359375, 29.49704933166504),
                Name = "Davis Clothing Shop"
            };

            _database.AddOneToCollection<ClothingShopModel>("ClothingShops", modelToAdd);
        }

        private async Task CreateClothingShopShapes()
        {
            var atms = await _database.GetAllFromCollection<ClothingShopModel>("ClothingShops");
            if (atms == null || atms.Count == 0) return;

            atms.ForEach(a =>
            {
                NAPI.Task.Run(() =>
                {
                    Blip b = NAPI.Blip.CreateBlip(73, a.Position, 1f, 0, a.Name);
                    b.ShortRange = true;

                    CColShape shape = (CColShape)NAPI.ColShape.CreateCylinderColShape(a.Position, 2f, 2f, 0);
                    shape.ShapeName = "ClothingShop";
                    shape.ShapeFunction = RunFunction;
                });
            });
        }

        private async void RunFunction(CPlayer.CPlayer player)
        {
            if (player == null) return;
            if (player.CurrentShape == null) return;

            if (player.CurrentShape.ShapeName != "ClothingShop") return;


            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "Client:CreateClothingShop");
            });
        }
    }
}
