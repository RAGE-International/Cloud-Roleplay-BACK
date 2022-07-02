using Backend.Core.Database;
using Backend.Models.FactionModel;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Factories.CColShape.Data
{
    public class FactionShape : Script
    {
        private readonly CDBCLient _database;

        public FactionShape()
        {
            _database = new CDBCLient();
        }

        private async Task LoadFactionShapes()
        {
            var factions = await _database.GetAllFromCollection<FactionModel>("Factions");
            if (factions == null) return;

            factions.ForEach(f =>
            {
                
            }); 
        }

        private void RunStorage(CPlayer.CPlayer player)
        {
            return;
        }

        private void RunFunctionFactionMenu()
        {


        }

    }
}
