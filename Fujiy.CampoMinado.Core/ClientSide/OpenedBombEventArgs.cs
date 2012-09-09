using System;

namespace Fujiy.CampoMinado.Core.ClientSide
{
    public class OpenedBombEventArgs : EventArgs
    {
        public int LocationX { get; private set; }
        public int LocationY { get; private set; }
        public int PlayerNumber { get; private set; }

        public OpenedBombEventArgs(int locationX, int locationY, int playerNumber)
        {
            LocationX = locationX;
            LocationY = locationY;
            PlayerNumber = playerNumber;
        }
    }
}