using System;
using System.Windows.Forms;

namespace Fujiy.CampoMinado.Cliente
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void btnConectar_Click(object sender, EventArgs e)
        {
            lblConectando.Text = "Conectando...";
            Application.DoEvents();

            Cliente Principal = new Cliente(txtIPServidor.Text);

            //Caso Conecte com Sucesso Abre o Jogo
            if (await Principal.Conectar())
            {
                Principal.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Não foi possivel estabelecer a conexão");
                lblConectando.Text = "";
            }
        }

        private void txtIPServidor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                btnConectar.PerformClick();
            }
        }
    }
}