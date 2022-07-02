using Backend.Core.Database;
using Backend.Models.Ammunation;
using Backend.Models.Ammunation.Data;
using Backend.Models.BankModel;
using Backend.Models.PlayerBankModel;
using Backend.Models.PlayerBankModel.Data;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Factories.CColShape.Data
{
    public class AmmunationShape : Script
    {
        private readonly CDBCLient _database;

        public AmmunationShape()
        {
            _database = new CDBCLient();

            CreateAmmunationShape();
            //CreateTestShape();
        }

        private void CreateTestShape()
        {
            var modelToAdd = new AmmunationModel
            {
                Position = new Vector3(22.495624542236328, -1105.433349609375, 29.796995162963867),
                Name = "Würfelpark Ammunation"
            };

            var weaponModel = new AmmunationItemModel
            {
                Category = 1,
                ItemId = 1,
                ItemImage = "gusenberg",
                ItemPrice = 15000
            };
            _database.AddOneToCollection<AmmunationItemModel>("Ammunation_Items", weaponModel);

            _database.AddOneToCollection<AmmunationModel>("Ammunations", modelToAdd);
        }

        private async Task CreateAmmunationShape()
        {
            var atms = await _database.GetAllFromCollection<AmmunationModel>("Ammunations");
            if (atms == null || atms.Count == 0) return;

            atms.ForEach(a =>
            {
                NAPI.Task.Run(() =>
                {
                    CColShape shape = (CColShape)NAPI.ColShape.CreateCylinderColShape(a.Position, 2f, 2f, 0);
                    shape.ShapeName = "Ammunation";
                    shape.ShapeFunction = RunFunction;

                    Blip blip = NAPI.Blip.CreateBlip(110, a.Position, 1f, 0, a.Name);
                    blip.ShortRange = true;
                });
            });
        }

        private async void RunFunction(CPlayer.CPlayer player)
        {
            if (player == null) return;
            if (player.CurrentShape == null) return;

            if (player.CurrentShape.ShapeName != "Ammunation") return;


            List<AmmunationItemModel> ammunationItems = await _database.GetAllFromCollection<AmmunationItemModel>("Ammunation_Items");
            if (ammunationItems == null) return;

            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "Client:Ammunation:OpenAmmunation", NAPI.Util.ToJson(ammunationItems));
            });
        }
    }
}
