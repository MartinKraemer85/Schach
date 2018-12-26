namespace Schach
{
    partial class frmBauernumwandlung
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
            this.btnTurm = new System.Windows.Forms.Button();
            this.btnDame = new System.Windows.Forms.Button();
            this.btnLaeufer = new System.Windows.Forms.Button();
            this.btnPferd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTurm
            // 
            this.btnTurm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnTurm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnTurm.Location = new System.Drawing.Point(159, 26);
            this.btnTurm.Name = "btnTurm";
            this.btnTurm.Size = new System.Drawing.Size(98, 97);
            this.btnTurm.TabIndex = 1;
            this.btnTurm.UseVisualStyleBackColor = true;
            this.btnTurm.Click += new System.EventHandler(this.btn_Click);
            // 
            // btnDame
            // 
            this.btnDame.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnDame.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnDame.Location = new System.Drawing.Point(43, 143);
            this.btnDame.Name = "btnDame";
            this.btnDame.Size = new System.Drawing.Size(98, 97);
            this.btnDame.TabIndex = 2;
            this.btnDame.UseVisualStyleBackColor = true;
            this.btnDame.Click += new System.EventHandler(this.btn_Click);
            // 
            // btnLaeufer
            // 
            this.btnLaeufer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLaeufer.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnLaeufer.Location = new System.Drawing.Point(159, 143);
            this.btnLaeufer.Name = "btnLaeufer";
            this.btnLaeufer.Size = new System.Drawing.Size(98, 97);
            this.btnLaeufer.TabIndex = 3;
            this.btnLaeufer.UseVisualStyleBackColor = true;
            this.btnLaeufer.Click += new System.EventHandler(this.btn_Click);
            // 
            // btnPferd
            // 
            this.btnPferd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPferd.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnPferd.Location = new System.Drawing.Point(43, 27);
            this.btnPferd.Name = "btnPferd";
            this.btnPferd.Size = new System.Drawing.Size(98, 97);
            this.btnPferd.TabIndex = 4;
            this.btnPferd.UseVisualStyleBackColor = true;
            this.btnPferd.Click += new System.EventHandler(this.btn_Click);
            // 
            // frmBauernumwandlung
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 279);
            this.Controls.Add(this.btnPferd);
            this.Controls.Add(this.btnLaeufer);
            this.Controls.Add(this.btnDame);
            this.Controls.Add(this.btnTurm);
            this.Name = "frmBauernumwandlung";
            this.Text = "Bauernumwandlung";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTurm;
        private System.Windows.Forms.Button btnDame;
        private System.Windows.Forms.Button btnLaeufer;
        private System.Windows.Forms.Button btnPferd;
    }
}