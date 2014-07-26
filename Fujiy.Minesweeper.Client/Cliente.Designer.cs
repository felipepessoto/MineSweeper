namespace Fujiy.CampoMinado.Cliente
{
    partial class Cliente
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
            this.PainelMapa = new System.Windows.Forms.Panel();
            this.lblPontosJogVer = new System.Windows.Forms.Label();
            this.lblPontosJogAzul = new System.Windows.Forms.Label();
            this.lblJogVer = new System.Windows.Forms.Label();
            this.lblJogAzul = new System.Windows.Forms.Label();
            this.txtRecebidas = new System.Windows.Forms.TextBox();
            this.lblVez = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPing = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PainelMapa
            // 
            this.PainelMapa.Location = new System.Drawing.Point(15, 12);
            this.PainelMapa.Name = "PainelMapa";
            this.PainelMapa.Size = new System.Drawing.Size(320, 320);
            this.PainelMapa.TabIndex = 4;
            // 
            // lblPontosJogVer
            // 
            this.lblPontosJogVer.AutoSize = true;
            this.lblPontosJogVer.Location = new System.Drawing.Point(100, 335);
            this.lblPontosJogVer.Name = "lblPontosJogVer";
            this.lblPontosJogVer.Size = new System.Drawing.Size(13, 13);
            this.lblPontosJogVer.TabIndex = 5;
            this.lblPontosJogVer.Text = "0";
            // 
            // lblPontosJogAzul
            // 
            this.lblPontosJogAzul.AutoSize = true;
            this.lblPontosJogAzul.Location = new System.Drawing.Point(100, 353);
            this.lblPontosJogAzul.Name = "lblPontosJogAzul";
            this.lblPontosJogAzul.Size = new System.Drawing.Size(13, 13);
            this.lblPontosJogAzul.TabIndex = 6;
            this.lblPontosJogAzul.Text = "0";
            // 
            // lblJogVer
            // 
            this.lblJogVer.AutoSize = true;
            this.lblJogVer.Location = new System.Drawing.Point(12, 335);
            this.lblJogVer.Name = "lblJogVer";
            this.lblJogVer.Size = new System.Drawing.Size(54, 13);
            this.lblJogVer.TabIndex = 7;
            this.lblJogVer.Text = "Vermelho:";
            // 
            // lblJogAzul
            // 
            this.lblJogAzul.AutoSize = true;
            this.lblJogAzul.Location = new System.Drawing.Point(12, 353);
            this.lblJogAzul.Name = "lblJogAzul";
            this.lblJogAzul.Size = new System.Drawing.Size(30, 13);
            this.lblJogAzul.TabIndex = 8;
            this.lblJogAzul.Text = "Azul:";
            // 
            // txtRecebidas
            // 
            this.txtRecebidas.Location = new System.Drawing.Point(9, 369);
            this.txtRecebidas.Multiline = true;
            this.txtRecebidas.Name = "txtRecebidas";
            this.txtRecebidas.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRecebidas.Size = new System.Drawing.Size(323, 110);
            this.txtRecebidas.TabIndex = 2;
            // 
            // lblVez
            // 
            this.lblVez.AutoSize = true;
            this.lblVez.Location = new System.Drawing.Point(352, 84);
            this.lblVez.Name = "lblVez";
            this.lblVez.Size = new System.Drawing.Size(0, 13);
            this.lblVez.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(232, 351);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ping:";
            // 
            // lblPing
            // 
            this.lblPing.AutoSize = true;
            this.lblPing.Location = new System.Drawing.Point(320, 351);
            this.lblPing.Name = "lblPing";
            this.lblPing.Size = new System.Drawing.Size(13, 13);
            this.lblPing.TabIndex = 10;
            this.lblPing.Text = "0";
            // 
            // Cliente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 369);
            this.Controls.Add(this.lblPing);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblVez);
            this.Controls.Add(this.lblJogAzul);
            this.Controls.Add(this.lblJogVer);
            this.Controls.Add(this.lblPontosJogAzul);
            this.Controls.Add(this.lblPontosJogVer);
            this.Controls.Add(this.PainelMapa);
            this.Controls.Add(this.txtRecebidas);
            this.Name = "Cliente";
            this.Text = "Campo Minado Online - Por Felipe Fujiy Pessoto";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Cliente_FormClosing);
            this.Load += new System.EventHandler(this.Cliente_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel PainelMapa;
        private System.Windows.Forms.Label lblPontosJogVer;
        private System.Windows.Forms.Label lblPontosJogAzul;
        private System.Windows.Forms.Label lblJogVer;
        private System.Windows.Forms.Label lblJogAzul;
        private System.Windows.Forms.TextBox txtRecebidas;
        private System.Windows.Forms.Label lblVez;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblPing;
    }
}

