using System;
using System.Collections.Generic;
using System.Drawing;
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
        private Thread processoConexao;
        private TcpClient conexao;
        private NetworkStream stream;
        private BinaryWriter writer;
        private BinaryReader reader;
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

        public void Executar()
        {
            //FALSE, PARA PERMITIR QUE ESSE PROCESSO TENHA ACESSO AO FORM(EX.: MUDAR O TEXTO DO LABEL)
            CheckForIllegalCrossThreadCalls = false;
            // Primeiro pega o numero de identificacao
            do
            {
                Thread.Sleep(500);
                //Espera O Aviso Que o Servidor Vai Enviar o Numero do Jogador
            } while (LerDados() != (int)MensagemParaCliente.NumJogador && conexao.Connected);

            meuNumero = LerDados();

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

            //Se for 0, voce comeca
            minhaVez = meuNumero == 0;

            // Recebe mensagens enviadas ao cliente
            while (!terminou && conexao.Connected)
            {
                Thread.Sleep(50);
                ProcessMessage(LerDados());
            }
        }

        public int LerDados()
        {
            int dados = 0;
            try
            {
                if (conexao.Connected && conexao.Available != 0)
                    dados = reader.ReadInt32();
            }
            catch
            {
                MessageBox.Show("Servidor caiu, game over", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            return dados;
        }

        public void ProcessMessage(int mensagem)
        {
            #if DEBUG
            this.Height = 518;
            if (mensagem > 0 && mensagem != (int)MensagemParaCliente.Ping)
                txtRecebidas.Text += (MensagemParaCliente)mensagem + Environment.NewLine;
            #endif
            if (mensagem == (int)MensagemParaCliente.Desconectar)
            {
                terminou = true;
                try
                {
                    stream.Close();
                    conexao.Close();
                    Application.Exit();
                }
                catch { };
            }

            if (mensagem == (int)MensagemParaCliente.Ping)
            {
                diferenca = DateTime.Now.Subtract(horario);
                horario = DateTime.Now;
                lblPing.Text = diferenca.Milliseconds.ToString();           
            }

            if (mensagem == (int)MensagemParaCliente.Venceu)
            {
                MessageBox.Show("Voce Venceu!");
            }

            if (mensagem == (int)MensagemParaCliente.Perdeu)
            {
                MessageBox.Show("Voce Perdeu!");
            }

            if (mensagem == (int)MensagemParaCliente.Amigosaiu)
            {
                MessageBox.Show("Seu Amigo Saiu!");
            }

            if (mensagem == (int)MensagemParaCliente.Abrir)
            {
                int localX = LerDados();
                int localY = LerDados();
                int situacao = LerDados();

                AbrirEspaco(localX, localY, situacao);
            }

            if (mensagem == (int)MensagemParaCliente.Vez)
            {
                if (LerDados() == meuNumero)
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

            if (mensagem == (int)MensagemParaCliente.LocalInvalido)
            {
                MessageBox.Show("Local Invalido");
            }

            if (mensagem == (int)MensagemParaCliente.Addponto)
            {
                AdicionarPonto(LerDados());
            }

            if (mensagem == (int)MensagemParaCliente.ComecaAbrirArea)
            {
                List<int> localX = new List<int>();
                List<int> localY = new List<int>();
                List<int> situacao = new List<int>();

                while (LerDados() != (int)MensagemParaCliente.FimAbrirArea)
                {
                    localX.Add(LerDados());
                    localY.Add(LerDados());
                    situacao.Add(LerDados());
                }
                for (int x = 0; x < localX.Count; x++)
                {
                    AbrirEspaco(localX[x], localY[x], situacao[x]);
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

        private void mapa_Click(object sender, EventArgs e)
        {
            if (minhaVez)
            {
                PictureBox clicada = (PictureBox)sender;
                try
                {
                    writer.Write((int)MensagemParaServidor.Abrir);
                    writer.Write(int.Parse(clicada.Name.Substring(0, 2)));
                    writer.Write(int.Parse(clicada.Name.Substring(2, 2)));
                }
                catch { }
            }
        }

        private void Cliente_Load(object sender, EventArgs e)
        {
            Preencher();
        }

        public bool Conectar()
        {
            try
            {
                conexao = new TcpClient(ipServidor, 2000);
            }
            catch
            {
                return false;
            }
            stream = conexao.GetStream();
            writer = new BinaryWriter(stream);
            reader = new BinaryReader(stream);

            //Processo para enviar e receber mensagens
            processoConexao = new Thread(Executar);
            processoConexao.Start();

            return true;
        }

        private void Cliente_FormClosing(object sender, FormClosingEventArgs e)
        {
            terminou = true;
            try
            {
                writer.Write((int)MensagemParaServidor.Desconectar);
                reader.Close();
                writer.Close();
                stream.Close();
                conexao.Close();
                Application.Exit();
            }
            catch { };
        }
    }
}