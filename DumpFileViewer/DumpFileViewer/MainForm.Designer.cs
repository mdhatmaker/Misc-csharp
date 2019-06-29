namespace DumpFileViewer
{
    partial class MainForm
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
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPageMain = new System.Windows.Forms.TabPage();
            this.btnOpenDumpFile = new System.Windows.Forms.Button();
            this.txtDump = new System.Windows.Forms.TextBox();
            this.tabPageInstruments = new System.Windows.Forms.TabPage();
            this.btnIidsFromClipboard = new System.Windows.Forms.Button();
            this.btnImportIidFile = new System.Windows.Forms.Button();
            this.listViewInstruments = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.openFileDlg = new System.Windows.Forms.OpenFileDialog();
            this.tabMain.SuspendLayout();
            this.tabPageMain.SuspendLayout();
            this.tabPageInstruments.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabPageMain);
            this.tabMain.Controls.Add(this.tabPageInstruments);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(708, 586);
            this.tabMain.TabIndex = 0;
            // 
            // tabPageMain
            // 
            this.tabPageMain.Controls.Add(this.btnOpenDumpFile);
            this.tabPageMain.Controls.Add(this.txtDump);
            this.tabPageMain.Location = new System.Drawing.Point(4, 22);
            this.tabPageMain.Name = "tabPageMain";
            this.tabPageMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMain.Size = new System.Drawing.Size(700, 560);
            this.tabPageMain.TabIndex = 0;
            this.tabPageMain.Text = "Main";
            this.tabPageMain.UseVisualStyleBackColor = true;
            // 
            // btnOpenDumpFile
            // 
            this.btnOpenDumpFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenDumpFile.Location = new System.Drawing.Point(8, 501);
            this.btnOpenDumpFile.Name = "btnOpenDumpFile";
            this.btnOpenDumpFile.Size = new System.Drawing.Size(135, 23);
            this.btnOpenDumpFile.TabIndex = 1;
            this.btnOpenDumpFile.Text = "Open Dump File...";
            this.btnOpenDumpFile.UseVisualStyleBackColor = true;
            this.btnOpenDumpFile.Click += new System.EventHandler(this.btnOpenDumpFile_Click);
            // 
            // txtDump
            // 
            this.txtDump.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDump.Location = new System.Drawing.Point(8, 6);
            this.txtDump.Multiline = true;
            this.txtDump.Name = "txtDump";
            this.txtDump.Size = new System.Drawing.Size(684, 489);
            this.txtDump.TabIndex = 0;
            // 
            // tabPageInstruments
            // 
            this.tabPageInstruments.Controls.Add(this.btnIidsFromClipboard);
            this.tabPageInstruments.Controls.Add(this.btnImportIidFile);
            this.tabPageInstruments.Controls.Add(this.listViewInstruments);
            this.tabPageInstruments.Location = new System.Drawing.Point(4, 22);
            this.tabPageInstruments.Name = "tabPageInstruments";
            this.tabPageInstruments.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageInstruments.Size = new System.Drawing.Size(700, 560);
            this.tabPageInstruments.TabIndex = 1;
            this.tabPageInstruments.Text = "Instruments";
            this.tabPageInstruments.UseVisualStyleBackColor = true;
            // 
            // btnIidsFromClipboard
            // 
            this.btnIidsFromClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnIidsFromClipboard.Location = new System.Drawing.Point(142, 529);
            this.btnIidsFromClipboard.Name = "btnIidsFromClipboard";
            this.btnIidsFromClipboard.Size = new System.Drawing.Size(110, 23);
            this.btnIidsFromClipboard.TabIndex = 3;
            this.btnIidsFromClipboard.Text = "Iids from clipboard";
            this.btnIidsFromClipboard.UseVisualStyleBackColor = true;
            this.btnIidsFromClipboard.Click += new System.EventHandler(this.btnIidsFromClipboard_Click);
            // 
            // btnImportIidFile
            // 
            this.btnImportIidFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnImportIidFile.Location = new System.Drawing.Point(6, 529);
            this.btnImportIidFile.Name = "btnImportIidFile";
            this.btnImportIidFile.Size = new System.Drawing.Size(110, 23);
            this.btnImportIidFile.TabIndex = 2;
            this.btnImportIidFile.Text = "Iids from text File...";
            this.btnImportIidFile.UseVisualStyleBackColor = true;
            this.btnImportIidFile.Click += new System.EventHandler(this.btnImportIidFile_Click);
            // 
            // listViewInstruments
            // 
            this.listViewInstruments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewInstruments.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewInstruments.Location = new System.Drawing.Point(8, 6);
            this.listViewInstruments.Name = "listViewInstruments";
            this.listViewInstruments.Size = new System.Drawing.Size(684, 518);
            this.listViewInstruments.TabIndex = 1;
            this.listViewInstruments.UseCompatibleStateImageBehavior = false;
            this.listViewInstruments.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "InstrumentId";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Symbol";
            this.columnHeader2.Width = 400;
            // 
            // openFileDlg
            // 
            this.openFileDlg.DefaultExt = "txt";
            this.openFileDlg.FileName = "openFileDialog1";
            this.openFileDlg.Filter = "\"Dump files|*.txt|All files|*.*\"";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 586);
            this.Controls.Add(this.tabMain);
            this.Name = "MainForm";
            this.Text = "Dump File Viewer";
            this.tabMain.ResumeLayout(false);
            this.tabPageMain.ResumeLayout(false);
            this.tabPageMain.PerformLayout();
            this.tabPageInstruments.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPageMain;
        private System.Windows.Forms.TabPage tabPageInstruments;
        private System.Windows.Forms.Button btnIidsFromClipboard;
        private System.Windows.Forms.Button btnImportIidFile;
        private System.Windows.Forms.ListView listViewInstruments;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnOpenDumpFile;
        private System.Windows.Forms.TextBox txtDump;
        private System.Windows.Forms.OpenFileDialog openFileDlg;
    }
}

