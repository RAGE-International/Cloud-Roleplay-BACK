using Backend.Core.Factories.CPlayer;
using Backend.Modules.Inventory.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Handlers.Inventory.Data
{
    public class IScriptItems
    {
        public string Name { get; set; }
        public Action<CPlayer, InventoryModel,int,int> Function { get; set; }
    }
}
