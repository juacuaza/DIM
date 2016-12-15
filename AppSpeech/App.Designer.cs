namespace AppSpeech
{
    partial class App
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(App));
            this.LBL_Texto = new System.Windows.Forms.Label();
            this.PB_Alarma = new System.Windows.Forms.PictureBox();
            this.LBL_Hora = new System.Windows.Forms.Label();
            this.WMP = new AxWMPLib.AxWindowsMediaPlayer();
            this.Browser = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Alarma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WMP)).BeginInit();
            this.SuspendLayout();
            // 
            // LBL_Texto
            // 
            this.LBL_Texto.AutoSize = true;
            this.LBL_Texto.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBL_Texto.Location = new System.Drawing.Point(8, 444);
            this.LBL_Texto.Name = "LBL_Texto";
            this.LBL_Texto.Size = new System.Drawing.Size(24, 20);
            this.LBL_Texto.TabIndex = 0;
            this.LBL_Texto.Text = "---";
            // 
            // PB_Alarma
            // 
            this.PB_Alarma.Image = ((System.Drawing.Image)(resources.GetObject("PB_Alarma.Image")));
            this.PB_Alarma.Location = new System.Drawing.Point(26, 210);
            this.PB_Alarma.Name = "PB_Alarma";
            this.PB_Alarma.Size = new System.Drawing.Size(270, 205);
            this.PB_Alarma.TabIndex = 2;
            this.PB_Alarma.TabStop = false;
            // 
            // LBL_Hora
            // 
            this.LBL_Hora.AutoSize = true;
            this.LBL_Hora.Font = new System.Drawing.Font("Microsoft Sans Serif", 38F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBL_Hora.Location = new System.Drawing.Point(160, 80);
            this.LBL_Hora.Name = "LBL_Hora";
            this.LBL_Hora.Size = new System.Drawing.Size(151, 59);
            this.LBL_Hora.TabIndex = 3;
            this.LBL_Hora.Text = "00:00";
            // 
            // WMP
            // 
            this.WMP.Enabled = true;
            this.WMP.Location = new System.Drawing.Point(12, 477);
            this.WMP.Name = "WMP";
            this.WMP.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("WMP.OcxState")));
            this.WMP.Size = new System.Drawing.Size(296, 429);
            this.WMP.TabIndex = 1;
            this.WMP.Visible = false;
            // 
            // Browser
            // 
            this.Browser.Location = new System.Drawing.Point(12, 24);
            this.Browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.Browser.Name = "Browser";
            this.Browser.Size = new System.Drawing.Size(299, 417);
            this.Browser.TabIndex = 4;
            // 
            // App
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::AppSpeech.Properties.Resources.principal;
            this.ClientSize = new System.Drawing.Size(320, 567);
            this.Controls.Add(this.Browser);
            this.Controls.Add(this.LBL_Hora);
            this.Controls.Add(this.PB_Alarma);
            this.Controls.Add(this.LBL_Texto);
            this.Controls.Add(this.WMP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "App";
            this.Text = "App";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Alarma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WMP)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LBL_Texto;
        private AxWMPLib.AxWindowsMediaPlayer WMP;
        private System.Windows.Forms.PictureBox PB_Alarma;
        private System.Windows.Forms.Label LBL_Hora;
        private System.Windows.Forms.WebBrowser Browser;
    }
}