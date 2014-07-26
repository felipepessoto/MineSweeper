namespace Fujiy.CampoMinado.Core
{
    public enum MessageToClient
    {
        OpenPosition = 1,
        Disconnect=2,
        AddScore=3,
        Won=4,
        Lost=5,
        ChangeTurn=6,
        FriendLeaved=7,
        InvalidPosition=8,
        PlayerColor=9,
        StartOpenPositionRange=10,
        EndOpenPositionRange=11,
        OpenBomb = 12,
    }
}
