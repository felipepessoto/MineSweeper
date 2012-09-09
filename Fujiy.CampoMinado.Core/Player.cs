using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Fujiy.CampoMinado.Core.ServerSide;

namespace Fujiy.CampoMinado.Core
{
    public class Player
    {
        private readonly Socket conexao;
        private readonly NetworkStream socketStream;
        private readonly ServerGameSession servidorRemoto;

        private readonly int meuNumero;
        private int score;
        bool ended;

        public string MeuIp { get; set; }

        public Player(Socket socket, ServerGameSession servidor, int numeroJogador)
        {
            conexao = socket;
            MeuIp = conexao.RemoteEndPoint.ToString();

            servidorRemoto = servidor;
            meuNumero = numeroJogador;

            // Cria objeto NetworkStream para o Socket
            socketStream = new NetworkStream(conexao);

            // Cria Streams para leitura/escrita dos bytes
            //writer = new BinaryWriter(socketStream);
            //reader = new BinaryReader(socketStream);
        }

        public async Task AdicionarPonto(int numJogador)
        {
            await SendData(MessageToClient.AddScore);
            await SendData(numJogador);
            if (numJogador == meuNumero)
                score++;
            if (score > 25)
            {
                ended = true;
                await servidorRemoto.Won(numJogador);
            }
        }

        public async Task AbrirLocalMapa(int coordX, int coordY, int situacao)
        {
            await SendData(MessageToClient.OpenPosition);
            await SendData(coordX);
            await SendData(coordY);
            await SendData(situacao);
        }

        public async Task AbrirBomba(int coordX, int coordY, int numeroJogador)
        {
            await SendData(MessageToClient.OpenBomb);
            await SendData(coordX);
            await SendData(coordY);
            await SendData(numeroJogador);
        }

        public async Task DesconectarJogador()
        {
            await SendData(MessageToClient.Disconnect);
            socketStream.Close();
            conexao.Close();
            ended = true;
            servidorRemoto.Disconnect();
        }

        public async Task ProcessarMensagem()
        {
            int mensagem = await ReceberMsg();

            if ((MessageToServer)mensagem == MessageToServer.OpenPosition)
            {
                int localX = await ReceberMsg();
                int localY = await ReceberMsg();

                if (localX < 16 && localY < 16)
                {
                    servidorRemoto.OpenPosition(localX, localY, meuNumero);
                }
                else
                {
                    await SendData(MessageToClient.InvalidPosition);
                }
            }
            else if ((MessageToServer)mensagem == MessageToServer.Disconnect)
            {
                servidorRemoto.FriendLeaved(meuNumero);
                DesconectarJogador();
            }
        }

        internal Task SendData(MessageToClient mensagem)
        {
            return SendData((int) mensagem);
        }

        internal async Task SendData(int mensagem)
        {
            byte[] buffer = BitConverter.GetBytes(mensagem);
            await socketStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private async Task<int> ReceberMsg()
        {
            byte[] buffer = new byte[4];
            await socketStream.ReadAsync(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }       

        public async Task Executar()
        {
            await SendData(MessageToClient.PlayerColor);
            await SendData(meuNumero);

            if (conexao.Connected)
            {
                while (!ended)
                {
                    await ProcessarMensagem();
                }
            }
        }
    }
}