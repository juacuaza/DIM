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
            this.LBL_Texto = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LBL_Texto
            // 
            this.LBL_Texto.AutoSize = true;
            this.LBL_Texto.Location = new System.Drawing.Point(12, 32);
            this.LBL_Texto.Name = "LBL_Texto";
            this.LBL_Texto.Size = new System.Drawing.Size(16, 13);
            this.LBL_Texto.TabIndex = 0;
            this.LBL_Texto.Text = "---";
            // 
            // App
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 82);
            this.Controls.Add(this.LBL_Texto);
            this.Name = "App";
            this.Text = "App";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LBL_Texto;
    }
}