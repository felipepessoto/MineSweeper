using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Fujiy.CampoMinado.Core.ClientSide
{
    public class GameSession
    {
        private TcpClient tcpConnection;
        
        private NetworkStream stream;
        
        private bool myTurn;

        public bool Ended { get; private set; }

        public PlayerColor MyPlayerColor { get; private set; }

        public int RedPlayerScore { get; private set; }

        public int BluePlayerScore { get; private set; }

        public event EventHandler<OpenedPositionEventArgs> PositionOpened;

        public event EventHandler<string> ShowMessage;

        public event EventHandler<PlayerColor> ChangeTurn;

        public event EventHandler<OpenedBombEventArgs> OpenBomb;

        public event EventHandler ScoredRefreshed;

        public async Task Connect(string serverHostName)
        {
            tcpConnection = new TcpClient(serverHostName, 2000);
            stream = tcpConnection.GetStream();

            MessageToClient response = await ReadDataEnum();

            if (response != MessageToClient.PlayerColor)
            {
                throw new Exception("Resposta inesperada");
            }

            if (!tcpConnection.Connected)
            {
//TODO acontece?
                throw new Exception("VERIFICAR");
            }

            MyPlayerColor = (PlayerColor)await ReadData();
            myTurn = MyPlayerColor == PlayerColor.Red;

            Task.Run(() => { ProcessMessage(); });
        }

        public async Task OpenPosition(int x, int y)
        {
            if (myTurn)
            {
                await SendData(MessageToServer.OpenPosition);
                await SendData(x);
                await SendData(y);
            }
        }

        public async Task Disconnect()
        {
            await SendData(MessageToServer.Disconnect);
            Finish();
        }

        private void Finish()
        {
            Ended = true;
            stream.Close();
            tcpConnection.Close();
        }

        public async Task ProcessMessage()
        {
            while (!Ended)
            {
                MessageToClient message = await ReadDataEnum();

                if (message == MessageToClient.Disconnect)
                {
                    Finish();
                }
                else if (message == MessageToClient.Won)
                {
                    Finish();
                    ShowMessage(this, "Você Venceu!");
                }
                else if (message == MessageToClient.Lost)
                {
                    Finish();
                    ShowMessage(this, "Você Perdeu!");
                }
                else if (message == MessageToClient.FriendLeaved)
                {
                    Finish();
                    ShowMessage(this, "Seu Amigo Saiu!");
                }
                else if (message == MessageToClient.OpenPosition)
                {
                    int localX = await ReadData();
                    int localY = await ReadData();
                    int bombsAround = await ReadData();

                    PositionOpened(this, new OpenedPositionEventArgs(localX, localY, bombsAround));
                }
                else if (message == MessageToClient.ChangeTurn)
                {
                    PlayerColor playerNumber = (PlayerColor)await ReadData();
                    myTurn = playerNumber == MyPlayerColor;
                    ChangeTurn(this, playerNumber);
                }
                else if (message == MessageToClient.InvalidPosition)
                {
                    ShowMessage(this, "Local Invalido");
                }
                else if (message == MessageToClient.AddScore)
                {
                    AddScore(await ReadData());
                }

                else if (message == MessageToClient.StartOpenPositionRange)
                {
                    while (await ReadData() != (int)MessageToClient.EndOpenPositionRange)
                    {
                        PositionOpened(this, new OpenedPositionEventArgs(await ReadData(), await ReadData(), await ReadData()));
                    }
                }
                else if(message ==MessageToClient.OpenBomb)
                {
                    int localX = await ReadData();
                    int localY = await ReadData();
                    int playerNumber = await ReadData();

                    OpenBomb(this, new OpenedBombEventArgs(localX, localY, playerNumber));
                }
                else
                {
                    throw new Exception("Mensagem desconhecida");
                }
            }
        }

        private void AddScore(int idjogador)
        {
            if (idjogador == 0)
            {
                RedPlayerScore++;
            }
            else
            {
                BluePlayerScore++;
            }
            ScoredRefreshed(this, new EventArgs());
        }

        private async Task<MessageToClient> ReadDataEnum()
        {
            int result = await ReadData();
            return (MessageToClient) result;
        }

        private async Task<int> ReadData()
        {
            var buffer = new byte[4];
            await stream.ReadAsync(buffer, 0, 4);
            int message = BitConverter.ToInt32(buffer, 0);

            Console.WriteLine("Recebendo: " + message + "(" + (MessageToClient) message + ")");
            return message;
        }

        private Task SendData(MessageToServer message)
        {
            Console.Write("Enviando: " + message);
            return SendData((int)message);
        }

        private async Task SendData(int message)
        {
            Console.Write("Enviando: " + message);
            byte[] buffer = BitConverter.GetBytes(message);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
