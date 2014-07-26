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

            Cliente Principal = new Cliente();
            await Principal.Conectar(txtIPServidor.Text);
            Principal.Show();
            this.Hide();
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