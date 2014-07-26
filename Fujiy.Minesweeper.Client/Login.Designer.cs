namespace Fujiy.CampoMinado.Cliente
{
    partial class Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnConectar = new System.Windows.Forms.Button();
            this.txtIPServidor = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblConectando = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnConectar
            // 
            this.btnConectar.Location = new System.Drawing.Point(12, 32);
            this.btnConectar.Name = "btnConectar";
            this.btnConectar.Size = new System.Drawing.Size(75, 23);
            this.btnConectar.TabIndex = 1;
            this.btnConectar.Text = "Conectar";
            this.btnConectar.UseVisualStyleBackColor = true;
            this.btnConectar.Click += new System.EventHandler(this.btnConectar_Click);
            // 
            // txtIPServidor
            // 
            this.txtIPServidor.Location = new System.Drawing.Point(71, 6);
            this.txtIPServidor.Name = "txtIPServidor";
            this.txtIPServidor.Size = new System.Drawing.Size(100, 20);
            this.txtIPServidor.TabIndex = 0;
            this.txtIPServidor.Text = "localhost";
            this.txtIPServidor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtIPServidor_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Servidor:";
            // 
            // lblConectando
            // 
            this.lblConectando.AutoSize = true;
            this.lblConectando.Location = new System.Drawing.Point(93, 37);
            this.lblConectando.Name = "lblConectando";
            this.lblConectando.Size = new System.Drawing.Size(0, 13);
            this.lblConectando.TabIndex = 4;
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(183, 63);
            this.Controls.Add(this.lblConectando);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtIPServidor);
            this.Controls.Add(this.btnConectar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Conectar";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConectar;
        private System.Windows.Forms.TextBox txtIPServidor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblConectando;
    }
}