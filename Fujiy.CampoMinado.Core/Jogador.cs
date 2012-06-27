using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Fujiy.CampoMinado.Core
{
    public class Jogador
    {
        private readonly Socket conexao;
        private readonly NetworkStream socketStream;
        private readonly ServidorJogo servidorRemoto;
        private readonly BinaryWriter writer;
        private readonly BinaryReader reader;

        private readonly int meuNumero;
        private int pontos;
        internal bool ProcessoSuspenso = true;
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
            writer = new BinaryWriter(socketStream);
            reader = new BinaryReader(socketStream);
        }

        public void AdicionarPonto(int numJogador)
        {
            EnviarMsg((int)MensagemParaCliente.Addponto);
            EnviarMsg(numJogador);
            if (numJogador == meuNumero)
                pontos++;
            if (pontos > 25)
                servidorRemoto.Venceu(numJogador);
        }

        public void AbrirLocalMapa(int coordX, int coordY, int situacao)
        {
            EnviarMsg((int)MensagemParaCliente.Abrir);
            EnviarMsg(coordX);
            EnviarMsg(coordY);
            EnviarMsg(situacao);
        }

        public void DesconectarJogador()
        {
            EnviarMsg((int)MensagemParaCliente.Desconectar);
            writer.Close();
            reader.Close();
            socketStream.Close();
            conexao.Close();
            terminou = true;
            servidorRemoto.Desconectar();
        }

        public void ProcessarMensagem()
        {
            int mensagem = ReceberMsg();

            if ((MensagemParaServidor)mensagem == MensagemParaServidor.Abrir)
            {
                int localX = ReceberMsg();
                int localY = ReceberMsg();

                if (localX < 16 && localY < 16)
                {
                    servidorRemoto.JogadaValida(localX, localY, meuNumero);
                }
                else
                {
                    EnviarMsg((int)MensagemParaCliente.LocalInvalido);
                }
            }
            else if ((MensagemParaServidor)mensagem == MensagemParaServidor.Desconectar)
            {
                servidorRemoto.AmigoSaiu(meuNumero);
                DesconectarJogador();
            }


        }

        internal void EnviarMsg(int mensagem)
        {
            try
            {
                writer.Write(mensagem);
            }
            catch
            {
            }
        }

        private int ReceberMsg()
        {
            try
            {
                return reader.ReadInt32();
            }
            catch
            {
                return 0;
            }
        }

        public void Ping()
        {
            while (conexao.Connected && !servidorRemoto.Desconectado)
            {
                EnviarMsg((int)MensagemParaCliente.Ping);//(Ping)Verifica se o cliente ainda esta conectado
                Thread.Sleep(1000);
            }
            DesconectarJogador();
        }        

        public void Executar()
        {
            //Avisa Que vai Enviar o Numero do Jogador e Envia( 0 ou 1)
            EnviarMsg((int)MensagemParaCliente.NumJogador);
            EnviarMsg(meuNumero);

            //Se for o primeiro a conectar
            if (meuNumero == 0)
            {
                //Suspende o processo
                lock (this)
                {
                    while (ProcessoSuspenso && conexao.Connected)
                    {
                        Monitor.Wait(this, 1000);
                    }
                }
            }
            if (conexao.Connected)
            {
                while (!terminou)
                {
                    //Espera mensagem
                    while (conexao.Connected && conexao.Available == 0)
                    {
                        Thread.Sleep(100);//Dorme 0.1 seg pra nao usar 100% da CPU
                    }
                    ProcessarMensagem();
                }
            }
        }
    }
}