using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Models.NotificationModel;
using Backend.Modules.Inventory;
using Backend.Modules.Inventory.Data;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Handlers.Inventory.Data.Items
{
    public class SchutzwesteData : Script
    {
        private readonly CDBCLient _database;
        private readonly InventoryModule _inventory;

        public SchutzwesteData()
        {
            _database = new CDBCLient();
            _inventory = new InventoryModule();
            AddVestToList();
        }

        public void AddVestToList()
        {
            var vest = new IScriptItems
            {
                Name = "Schutzweste",
                Function = RunFunction
            };

            ItemData.ScriptItems.Add(vest);

        }

        private async void RunFunction(CPlayer player, InventoryModel inventory, int slot, int amount)
        {
            if (player == null || !player.LoggedIn || player.InAction || inventory == null) return;


            var item = inventory.Slots[slot];
            if (item == null || item.Item == null) return;

            player.InAction = true;

            NAPI.Task.Run(() =>
            {
                player.StartProgressBar(4);
                player.PlayAnimation("anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", 1);
                player.SetActionAnimation("anim@heists@narcotics@funding@gang_idle", 1);
            });

            await Task.Delay(4 * 1000);

            if (player == null) return;
            if (!player.InAction) return;

            NAPI.Task.Run(() =>
            {
                player.PlayAnimation("anim@heists@narcotics@funding@gang_idle", "brakedownanim", 1);
                player.ResetActionAnimation();
                player.SetArmorAC(100);
            });

            _inventory.RemoveItem(slot, inventory, 1);

            player.SendCloudNotification($"Information", $"Du hast eine Schutzweste angezogen.", 2500, NotificationModel.SUCCESS, false);
            player.InAction = false;
        }
    }
}
