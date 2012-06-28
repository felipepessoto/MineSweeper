using System.IO;
using System.Net.Sockets;
using System.Threading;
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
            await EnviarMsg((int)MensagemParaCliente.Addponto);
            await EnviarMsg(numJogador);
            if (numJogador == meuNumero)
                pontos++;
            if (pontos > 25)
                servidorRemoto.Venceu(numJogador);
        }

        public async Task AbrirLocalMapa(int coordX, int coordY, int situacao)
        {
            await EnviarMsg((int)MensagemParaCliente.Abrir);
            await EnviarMsg(coordX);
            await EnviarMsg(coordY);
            await EnviarMsg(situacao);
        }

        public async Task DesconectarJogador()
        {
            await EnviarMsg((int)MensagemParaCliente.Desconectar);
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
                    await EnviarMsg((int)MensagemParaCliente.LocalInvalido);
                }
            }
            else if ((MensagemParaServidor)mensagem == MensagemParaServidor.Desconectar)
            {
                servidorRemoto.AmigoSaiu(meuNumero);
                DesconectarJogador();
            }
        }

        internal async Task EnviarMsg(int mensagem)
        {
            await new StreamWriter(socketStream).WriteLineAsync(mensagem.ToString());
        }

        private async Task<int> ReceberMsg()
        {
            string mensagem = await new StreamReader(socketStream).ReadLineAsync();
            return int.Parse(mensagem);
        }

        public async Task Ping()
        {
            while (conexao.Connected && !servidorRemoto.Desconectado)
            {
                await EnviarMsg((int)MensagemParaCliente.Ping);
                Thread.Sleep(1000);
            }
            DesconectarJogador();
        }        

        public async Task Executar()
        {
            await EnviarMsg((int)MensagemParaCliente.NumJogador);
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