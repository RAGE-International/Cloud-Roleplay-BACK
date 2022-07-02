using Backend.Core.Database;
using Backend.Core.Factories.CVehicle.Data;
using Backend.Models.VehicleModel;
using Backend.Modules.Inventory.Data;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Core.Factories.CVehicle
{
    public class CVehicle : Vehicle
    {
        private readonly CDBCLient _database = new CDBCLient();

        public int one { get; set; }
        public string Id 
        { 
            get 
            {
                if (DBModel.OwnerFaction == "")
                    return _database.GetOneFromCollection<VehicleModel>("Vehicles", v => v.VehId == DBModel.VehId)!.Result!.VehId;
                else
                    return _database.GetOneFromCollection<VehicleModel>("Faction_Vehicles", v => v.VehId == DBModel.VehId)!.Result!.VehId;
            } 
        }
        public int OwnerId { get; set; }
        public int[] Keys { get; set; }
        public int MaxWeight { get; set; }
        public int InventorySlots { get; set; }
        public bool TrunkLocked
        {
            get
            {
                return GetSharedData<bool>("IS_TRUNK_OPEN");
            }
            set
            {
                SetSharedData("IS_TRUNK_OPEN", value);
            }
        }
        public InventoryModel Inventory
        {
            get
            {
                return _database.GetOneFromCollection<InventoryModel>("Inventories", i => i.ExternalContainerID == DBModel.InventoryId).Result;
            }
        }
        public VehicleModel DBModel { get; set; } = new VehicleModel();
        public CVehicle(NetHandle handle) : base(handle)
        {
            OwnerId = 0;
            Keys = new int[4];
            MaxWeight = 0;
            InventorySlots = 0;
            DBModel = null;
        }

        public async void Update()
        {
            if (DBModel.OwnerFaction != "")
                await _database.Update<VehicleModel>("Vehicles", this.DBModel, v => v.VehId == Id);
            else
                await _database.Update<VehicleModel>("Faction_Vehicles", DBModel, v => v.VehId == Id);
        }
    }
}
