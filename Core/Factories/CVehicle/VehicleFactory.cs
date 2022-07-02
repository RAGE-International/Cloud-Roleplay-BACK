using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Core.Factories.CVehicle
{
    public class VehicleFactory : Script
    {
        public VehicleFactory()
        {
            RAGE.Entities.Vehicles.CreateEntity = netHandle => Create(netHandle);
        }

        internal Vehicle Create(NetHandle netHandle)
        {
            Vehicle vehicle = (CVehicle)Activator.CreateInstance(typeof(CVehicle), netHandle);
            if (vehicle is null)
            {
                throw new Exception("Error at EntityFactory : player is null");
            }
            

            return vehicle!;
        }
    }
}
