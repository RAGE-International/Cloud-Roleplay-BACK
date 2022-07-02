using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Core.Factories.CColShape
{
    public class CColShape : ColShape
    {
        public Action<CPlayer.CPlayer> ShapeFunction { get; set; }
        public string ShapeName { get; set; }
        public CColShape(NetHandle handle) : base(handle)
        {
            ShapeFunction = null;
            ShapeName = "";
        }
    }

    public class ShapeHandling : Script
    {
        [ServerEvent(Event.PlayerEnterColshape)]
        public void OnShapeEntered(CColShape shape,CPlayer.CPlayer player)
        {
            if (shape == null || player == null) return;

            player.CurrentShape = shape;
        }

        [ServerEvent(Event.PlayerExitColshape)]
        public void OnShapeExited(CColShape shape, CPlayer.CPlayer player)
        {
            if (shape == null || player == null) return;

            player.CurrentShape = null;
        }
    }
}
