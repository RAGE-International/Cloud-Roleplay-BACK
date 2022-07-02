using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backend.Core.Factories.Pools
{
    public sealed class Pools
    {
        public IEnumerable<CVehicle.CVehicle> GetAllCVehicles()
        {
            return NAPI.Pools.GetAllVehicles().Cast<CVehicle.CVehicle>().ToList();
        }

        public IEnumerable<CPlayer.CPlayer> GetAllCPlayers()
        {
            return NAPI.Pools.GetAllPlayers().Cast<CPlayer.CPlayer>().ToList();
        }
        public IEnumerable<CColShape.CColShape> GetAllCColShapes()
        {
            return NAPI.Pools.GetAllColShapes().Cast<CColShape.CColShape>().ToList();
        }
    }
}
