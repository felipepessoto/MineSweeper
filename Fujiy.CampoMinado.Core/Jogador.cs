using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Fujiy.CampoMinado.Core
{
    public class Jogador
    {
        private readonly Socket conexao;
        private readonly NetworkStream socketStream;
        private readonly ServidorJogo servidorRemoto;

        private readonly int meuNumero;
        private int pontos;
        bool terminou;

        public string MeuIp { get; set; }

        public Jogador(Socket socket, ServidorJogo servidor, int numeroJogador)
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
            await EnviarMsg(MensagemParaCliente.Addponto);
            await EnviarMsg(numJogador);
            if (numJogador == meuNumero)
                pontos++;
            if (pontos > 25)
            {
                terminou = true;
                await servidorRemoto.Venceu(numJogador);
            }
        }

        public async Task AbrirLocalMapa(int coordX, int coordY, int situacao)
        {
            await EnviarMsg(MensagemParaCliente.Abrir);
            await EnviarMsg(coordX);
            await EnviarMsg(coordY);
            await EnviarMsg(situacao);
        }

        public async Task AbrirBomba(int coordX, int coordY, int numeroJogador)
        {
            await EnviarMsg(MensagemParaCliente.AbrirBomba);
            await EnviarMsg(coordX);
            await EnviarMsg(coordY);
            await EnviarMsg(numeroJogador);
        }

        public async Task DesconectarJogador()
        {
            await EnviarMsg(MensagemParaCliente.Desconectar);
            socketStream.Close();
            conexao.Close();
            terminou = true;
            servidorRemoto.Desconectar();
        }

        public async Task ProcessarMensagem()
        {
            int mensagem = await ReceberMsg();

            if ((MensagemParaServidor)mensagem == MensagemParaServidor.Abrir)
            {
                int localX = await ReceberMsg();
                int localY = await ReceberMsg();

                if (localX < 16 && localY < 16)
                {
                    servidorRemoto.JogadaValida(localX, localY, meuNumero);
                }
                else
                {
                    await EnviarMsg(MensagemParaCliente.LocalInvalido);
                }
            }
            else if ((MensagemParaServidor)mensagem == MensagemParaServidor.Desconectar)
            {
                servidorRemoto.AmigoSaiu(meuNumero);
                DesconectarJogador();
            }
        }

        internal Task EnviarMsg(MensagemParaCliente mensagem)
        {
            return EnviarMsg((int) mensagem);
        }

        internal async Task EnviarMsg(int mensagem)
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
            await EnviarMsg(MensagemParaCliente.NumJogador);
            await EnviarMsg(meuNumero);

            if (conexao.Connected)
            {
                while (!terminou)
                {
                    await ProcessarMensagem();
                }
            }
        }
    }
}