using System;

namespace Fujiy.CampoMinado.Core.ClientSide
{
    public class OpenedPositionEventArgs : EventArgs
    {
        public int LocationX { get; private set; }
        public int LocationY { get; private set; }
        public int BombsAround { get; private set; }

        public OpenedPositionEventArgs(int locationX, int locationY, int bombsAround)
        {
            LocationX = locationX;
            LocationY = locationY;
            BombsAround = bombsAround;
        }
    }
}