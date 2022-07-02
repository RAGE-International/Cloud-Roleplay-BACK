using Backend.Core.Database;
using Backend.Core.Factories.Pools;
using GTANetworkAPI;
using System.Linq;

namespace Backend
{
    public class Resource : Script
    {
        private readonly CDBCLient _database;
        private readonly Pools _pools;

        public Resource()
        {
            _database = new CDBCLient();
            _pools = new Pools();
        }


        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            NAPI.Server.SetAutoRespawnAfterDeath(false);
            NAPI.Server.SetGlobalServerChat(false);
        }

        [ServerEvent(Event.ResourceStop)]
        private void OnResourceStop()
        {
            _pools.GetAllCPlayers()?.ToList().ForEach(p =>
            {
                if (p == null || p.DBModel == null) return;

                p.DBModel.Position = p.Position;
                p.Update().GetAwaiter();
            });
        }

    }
}