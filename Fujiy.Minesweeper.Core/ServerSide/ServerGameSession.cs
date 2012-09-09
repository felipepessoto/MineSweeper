using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Fujiy.CampoMinado.Core.ServerSide
{
    public class ServerGameSession
    {
        private const int DefaultPort = 2000;
        private readonly PositionType[,] boardMap;
        private int currentPlayer;
        public bool Disconnected { get; set; }

        private TcpListener listener;

        private readonly Player[] jogadores;

        public ServerGameSession()
        {
            boardMap = new PositionType[16, 16];
            jogadores = new Player[2];
            Start();
        }

        private async Task Start()
        {
            listener = new TcpListener(IPAddress.Any, DefaultPort);
            listener.Start();

            ShuffleMap();

            Task<Socket> jog1Socket = listener.AcceptSocketAsync();
            Task<Socket> jog2Socket = listener.AcceptSocketAsync();

            jogadores[0] = new Player(await jog1Socket, this, 0);
            jogadores[1] = new Player(await jog2Socket, this, 1);

            var taskJogador1 = jogadores[0].Executar();
            var taskJogador2 = jogadores[1].Executar();

            await Task.WhenAll(taskJogador1, taskJogador2);

            listener.Stop();
        }

        private void ShuffleMap()
        {
            //Zera o mapa
            for (int x = 0; x < boardMap.GetLength(0); x++)
            {
                for (int y = 0; y < boardMap.GetLength(1); y++)
                {
                    boardMap[x, y] = PositionType.Closed;
                }
            }

            //Coloca as bombas
            Random random = new Random();
            for (int x = 0; x < 52; x++)
            {
                int positionX;
                int positionY;
                do
                {
                    positionX = random.Next(0, 15);
                    positionY = random.Next(0, 15);
                } while (boardMap[positionX, positionY] == PositionType.ClosedBomb);
                boardMap[positionX, positionY] = PositionType.ClosedBomb;
            }


        }

        private bool IsClosed(int localX, int localY)
        {
            PositionType local = boardMap[localX, localY];
            return local == PositionType.Closed || local == PositionType.ClosedBomb;
        }

        private bool IsBomb(int localX, int localY)
        {
            return boardMap[localX, localY] == PositionType.ClosedBomb;
        }

        private async Task OpenAround(int localX, int localY)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (localX + x >= 0 && localX + x <= 15 && localY + y >= 0 && localY + y <= 15)
                    {
                        if (boardMap[localX + x, localY + y] == PositionType.Closed)
                        {
                            //Abre o Local
                            boardMap[localX + x, localY + y] = PositionType.Open;

                            //Calcula o numero do quadrado
                            int nQuadrado = EvaluatePositionNumber(localX + x, localY + y);

                            if (nQuadrado == 0)
                                await OpenAround(localX + x, localY + y);

                            await jogadores[0].AbrirLocalMapa(localX + x, localY + y, nQuadrado);
                            await jogadores[1].AbrirLocalMapa(localX + x, localY + y, nQuadrado);
                        }
                    }
                }
            }
        }

        private int EvaluatePositionNumber(int localX, int localY)
        {
            int total = 0;

            for(int x = -1; x<2;x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (localX + x >= 0 && localX + x <= 15 && localY + y >= 0 && localY + y <= 15)
                    {
                        if (boardMap[localX + x, localY + y] == PositionType.ClosedBomb || boardMap[localX + x, localY + y] == PositionType.OpenBomb)
                            total++;
                    }
                }
            }
            return total;

        }

        public async Task OpenPosition(int localX, int localY, int jogador)
        {
            if (jogador != currentPlayer)
            {
                return;
            }

            if (IsClosed(localX, localY))
            {
                if (IsBomb(localX, localY))
                {
                    await jogadores[0].AbrirBomba(localX, localY, jogador);
                    await jogadores[1].AbrirBomba(localX, localY, jogador);

                    //Atualiza o mapa
                    boardMap[localX, localY] = PositionType.OpenBomb;

                    await jogadores[0].AdicionarPonto(currentPlayer);
                    await jogadores[1].AdicionarPonto(currentPlayer);
                }
                else
                {
                    //Calcula o numero do quadrado
                    int nQuadrado = EvaluatePositionNumber(localX, localY);

                    if (nQuadrado == 0)
                    {
                        await jogadores[0].SendData(MessageToClient.StartOpenPositionRange);
                        await jogadores[1].SendData(MessageToClient.StartOpenPositionRange);
                        await OpenAround(localX, localY);
                        await jogadores[0].SendData(MessageToClient.EndOpenPositionRange);
                        await jogadores[1].SendData(MessageToClient.EndOpenPositionRange);
                    }

                    //Envia a mensagem para o jogador atual
                    await jogadores[0].AbrirLocalMapa(localX, localY, nQuadrado);
                    await jogadores[1].AbrirLocalMapa(localX, localY, nQuadrado == 9 ? 10 : nQuadrado);

                    //Atualiza o mapa
                    boardMap[localX, localY] = PositionType.Open;

                    //Passa a vez
                    currentPlayer = (currentPlayer + 1) % 2;
                    await jogadores[0].SendData(MessageToClient.ChangeTurn);
                    await jogadores[0].SendData(currentPlayer);
                    await jogadores[1].SendData(MessageToClient.ChangeTurn);
                    await jogadores[1].SendData(currentPlayer);
                }
            }

        }

        public void Disconnect()
        {
            Disconnected = true;
            listener.Stop();
        }

        public void FriendLeaved(int meuNumero)
        {
            jogadores[(meuNumero + 1) % 2].SendData(MessageToClient.FriendLeaved);
        }

        public async Task Won(int numJogador)
        {
            await jogadores[numJogador].SendData(MessageToClient.Won);
            await jogadores[(numJogador + 1) % 2].SendData(MessageToClient.Lost);

            Disconnect();
        }

        public string EnderecoRemotoP0
        {
            get
            {
                if (jogadores[0] != null)
                    return jogadores[0].MeuIp;
                return "Não Conectado";
            }
        }

        public string EnderecoRemotoP1
        {
            get
            {
                if (jogadores[1] != null)
                    return jogadores[1].MeuIp;
                return "Não Conectado";
            }
        }
    }
}
