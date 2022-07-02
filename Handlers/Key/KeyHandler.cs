using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Core.Factories.CVehicle;
using Backend.Core.Factories.Pools;
using Backend.Handlers.Inventory;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Backend.Modules.XMenu;

namespace Backend.Handlers.Key
{
    public class KeyHandler : Script
    {
        private readonly InventoryHandler _inventory;
        private readonly Pools _pools;
        private XMenuModule _xmenu;

        public KeyHandler()
        {
            _inventory = new InventoryHandler();
            _pools = new Pools();
            _xmenu = new XMenuModule();

            NAPI.ClientEvent.Register<CPlayer>("Server:KeyHandler:I", this, OnKeyPress_I);
            NAPI.ClientEvent.Register<CPlayer>("Server:KeyHandler:E", this, OnKeyPress_E);
            NAPI.ClientEvent.Register<CPlayer>("Server:KeyHandler:L", this, OnKeyPress_L);
            NAPI.ClientEvent.Register<CPlayer>("Server:KeyHandler:K", this, OnKeyPress_K);
        }

        private void OnKeyPress_K(CPlayer player)
        {
            if (player == null) return;
            if (!player.LoggedIn) return;

            if (player.Vehicle != null)
            {
                _xmenu.OnVehicleToggleTrunkState(player, (CVehicle)player.Vehicle);
                return;
            }

            CVehicle vehicle = _pools.GetAllCVehicles().FirstOrDefault(v => v.Position.DistanceTo(player.Position) <= 3f);
            if (vehicle == null) return;

            _xmenu.OnVehicleToggleTrunkState(player, vehicle);
        }

        private void OnKeyPress_L(CPlayer player)
        {
            if (player == null) return;
            if (!player.LoggedIn) return;

            if (player.Vehicle != null)
            {
                _xmenu.OnVehicleToggleLockState(player, (CVehicle)player.Vehicle);
                return;
            }

            if (player.CurrentShape != null)
            {
                switch (player.CurrentShape.ShapeName)
                {
                    case "House":

                        break;
                    case "StorageRoom":
                        break;

                    default:
                        break;
                }

                return;
            }
            
            CVehicle vehicle = _pools.GetAllCVehicles().FirstOrDefault(v => v.Position.DistanceTo(player.Position) <= 3f);
            if (vehicle == null) return;

            _xmenu.OnVehicleToggleLockState(player, vehicle);
        }

        private void OnKeyPress_I(CPlayer player)
        {
            if (player == null) return;
            if (!player.LoggedIn) return;

            var inventory = player.Inventory;
            if (inventory == null) return;

            _inventory.ShowInventory(player, !player.InInventory);
            player.InInventory = !player.InInventory;
        }

        private void OnKeyPress_E(CPlayer player)
        {
            if (player == null) return;
            if (player.InAction)
            {
                NAPI.Task.Run(() =>
                {
                    player.PlayAnimation("anim@heists@narcotics@funding@gang_idle", "brakedownanim", 1);
                    player.ResetActionAnimation();
                    player.StopProgressBar();
                    player.InAction = false;
                });
                return;
            }
            if (player.CurrentShape == null) return;

            player.CurrentShape?.ShapeFunction?.Invoke(player);

        }
    }
}
