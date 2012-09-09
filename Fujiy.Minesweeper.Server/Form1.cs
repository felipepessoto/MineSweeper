using System;
using System.Windows.Forms;
using Fujiy.CampoMinado.Core;
using Fujiy.CampoMinado.Core.ServerSide;

namespace Fujiy.CampoMinado.Servidor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        ServerGameSession servidorJogo;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (servidorJogo != null && !servidorJogo.Disconnected)
                servidorJogo.Disconnect();
        }
        private void btnCriarServidor_Click(object sender, EventArgs e)
        {
            if (servidorJogo == null)
            {
                btnCriarServidor.Text = "Parar Servidor";
                servidorJogo = new ServerGameSession();
                timer1.Enabled = true;
            }
            else
            {
                btnCriarServidor.Text = "Criar Servidor";
                servidorJogo.Disconnect();
                servidorJogo = null;
                timer1.Enabled = false;
                lblIP1.Text = "Não Conectado";
                lblIP2.Text = "Não Conectado";
            }

        }

        public string TextoBotao
        {
            set { btnCriarServidor.Text = value; }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblIP1.Text = servidorJogo.EnderecoRemotoP0;
            lblIP2.Text = servidorJogo.EnderecoRemotoP1;
            if (servidorJogo.Disconnected)
                btnCriarServidor.PerformClick();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblIP1.Text = "Não Conectado";
            lblIP2.Text = "Não Conectado";
        }

    }
}