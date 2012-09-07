using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Fujiy.CampoMinado.Cliente.Properties;
using Fujiy.CampoMinado.Core;

namespace Fujiy.CampoMinado.Cliente
{
    public partial class Cliente : Form
    {
        private TcpClient conexao;
        private NetworkStream stream;
        private readonly string ipServidor;
        private DateTime horario;
        private TimeSpan diferenca;

        private readonly PictureBox[,] mapa = new PictureBox[16, 16];

        private int meuNumero;
        private bool minhaVez;
        private bool terminou;

        private int pontosVermelho;
        private int pontosAzul;

        public Cliente(string ip)
        {
            ipServidor = ip;
            InitializeComponent();
        }

        public async Task<int> LerDados()
        {
            byte[] buffer = new byte[4];
            await stream.ReadAsync(buffer, 0, 4);
            int mensagem = BitConverter.ToInt32(buffer, 0);

#if DEBUG
            Height = 518;
            txtRecebidas.Text += "Recebendo: " + mensagem + "(" + (MensagemParaCliente)mensagem + ")" + Environment.NewLine;
#endif
            
            return mensagem;
        }

        internal Task EnviarMsg(MensagemParaServidor mensagem)
        {
#if DEBUG
            Height = 518;
            txtRecebidas.Text += "Enviando: " + mensagem + Environment.NewLine;
#endif
            return EnviarMsg((int) mensagem);
        }

        internal async Task EnviarMsg(int mensagem)
        {
#if DEBUG
            Height = 518;
            txtRecebidas.Text += "Enviando: " + mensagem + Environment.NewLine;
#endif

            byte[] buffer = BitConverter.GetBytes(mensagem);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }

        public async Task ProcessMessage()
        {
            while (true)
            {
                int mensagem = await LerDados();

                if (mensagem == (int) MensagemParaCliente.Desconectar)
                {
                    terminou = true;
                    stream.Close();
                    conexao.Close();
                    Application.Exit();
                }

                if (mensagem == (int) MensagemParaCliente.Venceu)
                {
                    MessageBox.Show("Voce Venceu!");
                }

                if (mensagem == (int) MensagemParaCliente.Perdeu)
                {
                    MessageBox.Show("Voce Perdeu!");
                }

                if (mensagem == (int) MensagemParaCliente.Amigosaiu)
                {
                    MessageBox.Show("Seu Amigo Saiu!");
                }

                if (mensagem == (int) MensagemParaCliente.Abrir)
                {
                    int localX = await LerDados();
                    int localY = await LerDados();
                    int situacao = await LerDados();

                    AbrirEspaco(localX, localY, situacao);
                }

                if (mensagem == (int) MensagemParaCliente.Vez)
                {
                    if (await LerDados() == meuNumero)
                    {
                        minhaVez = true;
                        lblVez.Text = "Sua Vez";
                    }
                    else
                    {
                        minhaVez = false;
                        lblVez.Text = "Vez do seu rival";
                    }
                }

                if (mensagem == (int) MensagemParaCliente.LocalInvalido)
                {
                    MessageBox.Show("Local Invalido");
                }

                if (mensagem == (int) MensagemParaCliente.Addponto)
                {
                    AdicionarPonto(await LerDados());
                }

                if (mensagem == (int) MensagemParaCliente.ComecaAbrirArea)
                {
                    List<int> localX = new List<int>();
                    List<int> localY = new List<int>();
                    List<int> situacao = new List<int>();

                    while (await LerDados() != (int) MensagemParaCliente.FimAbrirArea)
                    {
                        localX.Add(await LerDados());
                        localY.Add(await LerDados());
                        situacao.Add(await LerDados());
                    }
                    for (int x = 0; x < localX.Count; x++)
                    {
                        AbrirEspaco(localX[x], localY[x], situacao[x]);
                    }
                }
            }
        }

        private void AdicionarPonto(int idjogador)
        {
            if (idjogador == 0)
            {
                pontosVermelho++; 
                lblPontosJogVer.Text = pontosVermelho.ToString();
            }
            else
            {
                pontosAzul++;
                lblPontosJogAzul.Text = pontosAzul.ToString();
            }
        }

        private void AbrirEspaco(int localX, int localY, int situacao)
        {
            if (situacao == 0)
            {
                mapa[localX, localY].Image = Resources.Vazio;
            }
            else if(situacao == 1)
            {
                mapa[localX, localY].Image = Resources._1;
            }
            else if (situacao == 2)
            {
                mapa[localX, localY].Image = Resources._2;
            }
            else if(situacao == 3)
            {
                mapa[localX, localY].Image = Resources._3;
            }
            else if (situacao == 4)
            {
                mapa[localX, localY].Image = Resources._4;
            }
            else if (situacao == 5)
            {
                mapa[localX, localY].Image = Resources._5;
            }
            else if (situacao == 6)
            {
                mapa[localX, localY].Image = Resources._6;
            }
            else if (situacao == 7)
            {
                mapa[localX, localY].Image = Resources._7;
            }
            else if (situacao == 8)
            {
                mapa[localX, localY].Image = Resources._8;
            }
            else if (situacao == 9)
            {
                if (meuNumero == 0)
                    mapa[localX, localY].Image = Resources.Vermelho;
                else
                    mapa[localX, localY].Image = Resources.Azul;
            }
            else if (situacao == 10)
            {
                if (meuNumero == 1)
                    mapa[localX, localY].Image = Resources.Vermelho;
                else
                    mapa[localX, localY].Image = Resources.Azul;
            }
        }

        private void Preencher()
        {
            //Zera o mapa
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    mapa[x, y] = new PictureBox
                                     {
                                         Width = 20,
                                         Height = 20,
                                         Location = new Point(20*x, 20*y),
                                         Image = Resources.imagem,
                                         Enabled = true
                                     };

                    if (x < 10)
                        mapa[x, y].Name += "0";
                    mapa[x, y].Name += x.ToString();
                    if (y < 10)
                        mapa[x, y].Name += "0";
                    mapa[x, y].Name += y.ToString();

                    mapa[x, y].Click += mapa_Click;
                    PainelMapa.Controls.Add(mapa[x, y]);
                }
            }
        }

        private async void mapa_Click(object sender, EventArgs e)
        {
            if (minhaVez)
            {
                PictureBox clicada = (PictureBox)sender;

                await EnviarMsg(MensagemParaServidor.Abrir);
                await EnviarMsg(int.Parse(clicada.Name.Substring(0, 2)));
                await EnviarMsg(int.Parse(clicada.Name.Substring(2, 2)));
            }
        }

        private void Cliente_Load(object sender, EventArgs e)
        {
            Preencher();
        }

        public async Task Conectar()
        {
            conexao = new TcpClient(ipServidor, 2000);
            stream = conexao.GetStream();

            CheckForIllegalCrossThreadCalls = false;

            while (await LerDados() != (int)MensagemParaCliente.NumJogador && conexao.Connected)
            {
            }

            meuNumero = await LerDados();

            if (meuNumero == 0)
            {
                lblJogVer.Text = "Vermelho(Você): ";
                lblVez.Text = "Sua Vez";
            }
            else
            {
                lblJogAzul.Text = "Azul(Você): ";
                lblVez.Text = "Vez do seu rival";
            }

            horario = DateTime.Now;
            minhaVez = meuNumero == 0;

            Task.Run(() => { ProcessMessage(); });
        }

        private async void Cliente_FormClosing(object sender, FormClosingEventArgs e)
        {
            terminou = true;
            await EnviarMsg(MensagemParaServidor.Desconectar);
            stream.Close();
            conexao.Close();
            Application.Exit();
        }
    }
}