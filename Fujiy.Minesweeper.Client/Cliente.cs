using Fujiy.CampoMinado.Cliente.Properties;
using Fujiy.CampoMinado.Core;
using Fujiy.CampoMinado.Core.ClientSide;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fujiy.CampoMinado.Cliente
{
    public partial class Cliente : Form
    {
        private readonly GameSession player = new GameSession();
        private readonly PictureBox[,] mapa = new PictureBox[16, 16];

        public Cliente()
        {
            InitializeComponent();
            player.PositionOpened += PlayerPositionOpened;
            player.ShowMessage += player_ShowMessage;
            player.ChangeTurn += player_ChangeTurn;
            player.OpenBomb += player_OpenBomb;
            player.ScoredRefreshed += player_ScoredRefreshed;
        }

        private void player_ScoredRefreshed(object sender, EventArgs e)
        {
            lblPontosJogVer.Text = player.RedPlayerScore.ToString();
            lblPontosJogAzul.Text = player.BluePlayerScore.ToString();
        }

        private void PlayerPositionOpened(object sender, OpenedPositionEventArgs e)
        {
            int localX = e.LocationX;
            int localY = e.LocationY;
            int bombsAround = e.BombsAround;

            if (bombsAround == 0)
            {
                mapa[localX, localY].Image = Resources.Vazio;
            }
            else if (bombsAround == 1)
            {
                mapa[localX, localY].Image = Resources._1;
            }
            else if (bombsAround == 2)
            {
                mapa[localX, localY].Image = Resources._2;
            }
            else if (bombsAround == 3)
            {
                mapa[localX, localY].Image = Resources._3;
            }
            else if (bombsAround == 4)
            {
                mapa[localX, localY].Image = Resources._4;
            }
            else if (bombsAround == 5)
            {
                mapa[localX, localY].Image = Resources._5;
            }
            else if (bombsAround == 6)
            {
                mapa[localX, localY].Image = Resources._6;
            }
            else if (bombsAround == 7)
            {
                mapa[localX, localY].Image = Resources._7;
            }
            else if (bombsAround == 8)
            {
                mapa[localX, localY].Image = Resources._8;
            }
            else
            {
                throw new Exception();
            }
        }

        private void player_OpenBomb(object sender, OpenedBombEventArgs e)
        {
            mapa[e.LocationX, e.LocationY].Image = e.PlayerNumber == 0 ? Resources.Vermelho : Resources.Azul;
        }

        private void player_ShowMessage(object sender, string e)
        {
            MessageBox.Show(e);
        }

        private void player_ChangeTurn(object sender, PlayerColor e)
        {
            lblVez.Text = e == player.MyPlayerColor ? "Sua Vez" : "Vez do seu rival";
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
                                         Enabled = true,
                                         Name =  x.ToString("00") + y.ToString("00"),
                                     };
                    mapa[x, y].Click += mapa_Click;
                    PainelMapa.Controls.Add(mapa[x, y]);
                }
            }
        }

        private async void mapa_Click(object sender, EventArgs e)
        {
            PictureBox clicada = (PictureBox)sender;
            await player.OpenPosition(int.Parse(clicada.Name.Substring(0, 2)), int.Parse(clicada.Name.Substring(2, 2)));
        }

        private void Cliente_Load(object sender, EventArgs e)
        {
            Preencher();
        }

        public async Task Conectar(string hostName)
        {
            await player.Connect(hostName);

            if (player.MyPlayerColor == 0)
            {
                lblJogVer.Text = "Vermelho(Você): ";
                lblVez.Text = "Sua Vez";
            }
            else
            {
                lblJogAzul.Text = "Azul(Você): ";
                lblVez.Text = "Vez do seu rival";
            }
        }

        private async void Cliente_FormClosing(object sender, FormClosingEventArgs e)
        {
            await player.Disconnect();
            Application.Exit();
        }
    }
}