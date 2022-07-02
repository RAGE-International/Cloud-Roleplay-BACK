using Backend.Core.Database;
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
    public class BankShape : Script
    {
        private readonly CDBCLient _database;

        public BankShape()
        {
            _database = new CDBCLient();

            CreateBankShape();
            //CreateTestShape();
        }

        private void CreateTestShape()
        {
            var modelToAdd = new BankModel
            {
                Position = new Vector3(33.27184295654297, -1348.1585693359375, 29.49704933166504),
                Name = "Davis Shop Atm"
            };

            _database.AddOneToCollection<BankModel>("ATMS", modelToAdd);
        }

        private async Task CreateBankShape()
        {
            var atms = await _database.GetAllFromCollection<BankModel>("ATMS");
            if (atms == null || atms.Count == 0) return;

            atms.ForEach(a =>
            {
                NAPI.Task.Run(() =>
                {
                    CColShape shape = (CColShape)NAPI.ColShape.CreateCylinderColShape(a.Position, 2f, 2f, 0);
                    shape.ShapeName = "ATM";
                    shape.ShapeFunction = RunFunction;
                });
            });
        }

        private async void RunFunction(CPlayer.CPlayer player)
        {
            if (player == null) return;
            if (player.CurrentShape == null) return;

            if (player.CurrentShape.ShapeName != "ATM") return;


            var bankModel = new PlayerBankModel(player.DBModel.BankMoney, player.DBModel.Money, player.DBModel.BankNumber, new BankHistoryModel());
            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "Client:CreateBank", NAPI.Util.ToJson(bankModel));
            });
        }
    }
}
