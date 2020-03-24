namespace SubDemo
{
    partial class SubDemoForm
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
            this.pbOcean = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbOcean)).BeginInit();
            this.SuspendLayout();
            // 
            // pbOcean
            // 
            this.pbOcean.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbOcean.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbOcean.Location = new System.Drawing.Point(12, 12);
            this.pbOcean.Name = "pbOcean";
            this.pbOcean.Size = new System.Drawing.Size(486, 381);
            this.pbOcean.TabIndex = 0;
            this.pbOcean.TabStop = false;
            // 
            // SubDemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 405);
            this.Controls.Add(this.pbOcean);
            this.MinimumSize = new System.Drawing.Size(300, 240);
            this.Name = "SubDemoForm";
            this.Text = "SubDemo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SubDemoForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pbOcean)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbOcean;
    }
}

