using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Core.Factories.CColShape
{
    public class ColShapeFactory : Script
    {
        public ColShapeFactory()
        {
            RAGE.Entities.Colshapes.CreateEntity = netHandle => Create(netHandle);
        }

        internal ColShape Create(NetHandle netHandle)
        {
            CColShape shape = (CColShape)Activator.CreateInstance(typeof(CColShape), netHandle);
            if (shape is null)
            {
                throw new Exception("Error at EntityFactory : player is null");
            }
            return shape!;
        }
    }
}
