namespace Fujiy.CampoMinado.Servidor
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.btnCriarServidor = new System.Windows.Forms.Button();
            this.lblIP1 = new System.Windows.Forms.Label();
            this.lblIP2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnCriarServidor
            // 
            this.btnCriarServidor.Location = new System.Drawing.Point(12, 21);
            this.btnCriarServidor.Name = "btnCriarServidor";
            this.btnCriarServidor.Size = new System.Drawing.Size(91, 23);
            this.btnCriarServidor.TabIndex = 0;
            this.btnCriarServidor.Text = "Criar Servidor";
            this.btnCriarServidor.UseVisualStyleBackColor = true;
            this.btnCriarServidor.Click += new System.EventHandler(this.btnCriarServidor_Click);
            // 
            // lblIP1
            // 
            this.lblIP1.AutoSize = true;
            this.lblIP1.Location = new System.Drawing.Point(191, 12);
            this.lblIP1.Name = "lblIP1";
            this.lblIP1.Size = new System.Drawing.Size(33, 13);
            this.lblIP1.TabIndex = 1;
            this.lblIP1.Text = "lblIP1";
            // 
            // lblIP2
            // 
            this.lblIP2.AutoSize = true;
            this.lblIP2.Location = new System.Drawing.Point(191, 39);
            this.lblIP2.Name = "lblIP2";
            this.lblIP2.Size = new System.Drawing.Size(33, 13);
            this.lblIP2.TabIndex = 2;
            this.lblIP2.Text = "lblIP2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(128, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Jogador 1:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(128, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Jogador 2:";
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 71);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblIP2);
            this.Controls.Add(this.lblIP1);
            this.Controls.Add(this.btnCriarServidor);
            this.Name = "Form1";
            this.Text = "Campo Minado - Servidor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblIP1;
        private System.Windows.Forms.Label lblIP2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCriarServidor;
        private System.Windows.Forms.Timer timer1;


    }
}

