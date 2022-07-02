using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Factories.CPlayer
{
    public class CPlayerFactory : Script
    {
        public CPlayerFactory()
        {
            RAGE.Entities.Players.CreateEntity = netHandle => Create(netHandle);
        }

        internal Player Create(NetHandle netHandle)
        {
            CPlayer player = (CPlayer)Activator.CreateInstance(typeof(CPlayer), netHandle);
            if (player is null)
            {
                throw new Exception("Error at EntityFactory : player is null");
            }
            return player!;
        }
    }
}
