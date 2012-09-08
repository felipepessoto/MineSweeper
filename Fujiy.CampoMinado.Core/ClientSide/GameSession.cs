using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fujiy.CampoMinado.Core.ClientSide
{
    public class GameSession
    {
        private TcpClient conexao;
        private NetworkStream stream;
        private bool minhaVez;
        public bool Terminou { get; private set; }

        public int MeuNumero { get; private set; }

        public int PontosVermelho { get; private set; }
        public int PontosAzul { get; private set; }

        public event EventHandler<OpenedPositionEventArgs> OpenPosition;

        public event EventHandler<string> ShowMessage;

        public event EventHandler<int> ChangeTurn;

        public event EventHandler<OpenedBombEventArgs> OpenBomb;

        public event EventHandler ScoredRefreshed;

        public async Task Connect(string serverHostName)
        {
            conexao = new TcpClient(serverHostName, 2000);
            stream = conexao.GetStream();

            MensagemParaCliente response = await ReadDataEnum();

            if (response != MensagemParaCliente.NumJogador)
            {
                throw new Exception("Resposta inesperada");
            }

            if (!conexao.Connected)
            {
//TODO acontece?
                throw new Exception("VERIFICAR");
            }

            MeuNumero = await ReadData();
            minhaVez = MeuNumero == 0;

            Task.Run(() => { ProcessMessage(); });
        }

        public async Task TryToOpen(int x, int y)
        {
            if (minhaVez)
            {
                await SendData(MensagemParaServidor.Abrir);
                await SendData(x);
                await SendData(y);
            }
        }

        public async Task Disconnect()
        {
            await SendData(MensagemParaServidor.Desconectar);
            Finish();
        }

        private void Finish()
        {
            Terminou = true;
            stream.Close();
            conexao.Close();
        }

        public async Task ProcessMessage()
        {
            while (!Terminou)
            {
                MensagemParaCliente mensagem = await ReadDataEnum();

                if (mensagem == MensagemParaCliente.Desconectar)
                {
                    Finish();
                }
                else if (mensagem == MensagemParaCliente.Venceu)
                {
                    Finish();
                    ShowMessage(this, "Você Venceu!");
                }
                else if (mensagem == MensagemParaCliente.Perdeu)
                {
                    Finish();
                    ShowMessage(this, "Você Perdeu!");
                }
                else if (mensagem == MensagemParaCliente.Amigosaiu)
                {
                    Finish();
                    ShowMessage(this, "Seu Amigo Saiu!");
                }
                else if (mensagem == MensagemParaCliente.Abrir)
                {
                    int localX = await ReadData();
                    int localY = await ReadData();
                    int bombsAround = await ReadData();

                    OpenPosition(this, new OpenedPositionEventArgs(localX, localY, bombsAround));
                }
                else if (mensagem == MensagemParaCliente.Vez)
                {
                    int playerNumber = await ReadData();
                    minhaVez = playerNumber == MeuNumero;
                    ChangeTurn(this, playerNumber);
                }
                else if (mensagem == MensagemParaCliente.LocalInvalido)
                {
                    ShowMessage(this, "Local Invalido");
                }
                else if (mensagem == MensagemParaCliente.Addponto)
                {
                    AddScore(await ReadData());
                }

                else if (mensagem == MensagemParaCliente.ComecaAbrirArea)
                {
                    while (await ReadData() != (int)MensagemParaCliente.FimAbrirArea)
                    {
                        OpenPosition(this, new OpenedPositionEventArgs(await ReadData(), await ReadData(), await ReadData()));
                    }
                }
                else if(mensagem ==MensagemParaCliente.AbrirBomba)
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
                PontosVermelho++;
            }
            else
            {
                PontosAzul++;
            }
            ScoredRefreshed(this, new EventArgs());
        }

        private async Task<MensagemParaCliente> ReadDataEnum()
        {
            int result = await ReadData();
            return (MensagemParaCliente) result;
        }

        private async Task<int> ReadData()
        {
            byte[] buffer = new byte[4];
            await stream.ReadAsync(buffer, 0, 4);
            int mensagem = BitConverter.ToInt32(buffer, 0);

            Console.WriteLine("Recebendo: " + mensagem + "(" + (MensagemParaCliente) mensagem + ")");
            return mensagem;
        }

        private Task SendData(MensagemParaServidor mensagem)
        {
            Console.Write("Enviando: " + mensagem);
            return SendData((int)mensagem);
        }

        private async Task SendData(int mensagem)
        {
            Console.Write("Enviando: " + mensagem);
            byte[] buffer = BitConverter.GetBytes(mensagem);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }

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
