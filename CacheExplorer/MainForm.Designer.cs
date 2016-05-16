namespace CacheExplorer
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
            this.components = new System.ComponentModel.Container();
            this.fastObjectListViewCacheFiles = new BrightIdeasSoftware.FastObjectListView();
            this.olvColumnCreateDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnFileName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnFileSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxMp3 = new System.Windows.Forms.CheckBox();
            this.contextMenuStripCacheFiles = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.olvColumnMediaLength = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            ((System.ComponentModel.ISupportInitialize)(this.fastObjectListViewCacheFiles)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStripCacheFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // fastObjectListViewCacheFiles
            // 
            this.fastObjectListViewCacheFiles.AllColumns.Add(this.olvColumnCreateDate);
            this.fastObjectListViewCacheFiles.AllColumns.Add(this.olvColumnFileName);
            this.fastObjectListViewCacheFiles.AllColumns.Add(this.olvColumnMediaLength);
            this.fastObjectListViewCacheFiles.AllColumns.Add(this.olvColumnFileSize);
            this.fastObjectListViewCacheFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fastObjectListViewCacheFiles.CellEditUseWholeCell = false;
            this.fastObjectListViewCacheFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnCreateDate,
            this.olvColumnFileName,
            this.olvColumnFileSize});
            this.fastObjectListViewCacheFiles.ContextMenuStrip = this.contextMenuStripCacheFiles;
            this.fastObjectListViewCacheFiles.Cursor = System.Windows.Forms.Cursors.Default;
            this.fastObjectListViewCacheFiles.FullRowSelect = true;
            this.fastObjectListViewCacheFiles.GridLines = true;
            this.fastObjectListViewCacheFiles.Location = new System.Drawing.Point(12, 27);
            this.fastObjectListViewCacheFiles.Name = "fastObjectListViewCacheFiles";
            this.fastObjectListViewCacheFiles.ShowGroups = false;
            this.fastObjectListViewCacheFiles.Size = new System.Drawing.Size(855, 266);
            this.fastObjectListViewCacheFiles.TabIndex = 0;
            this.fastObjectListViewCacheFiles.UseCompatibleStateImageBehavior = false;
            this.fastObjectListViewCacheFiles.View = System.Windows.Forms.View.Details;
            this.fastObjectListViewCacheFiles.VirtualMode = true;
            // 
            // olvColumnCreateDate
            // 
            this.olvColumnCreateDate.AspectName = "CreateDate";
            this.olvColumnCreateDate.AspectToStringFormat = "{0:dd.MM.yyyy HH:mm:ss}";
            this.olvColumnCreateDate.Text = "CreateDate";
            this.olvColumnCreateDate.Width = 110;
            // 
            // olvColumnFileName
            // 
            this.olvColumnFileName.AspectName = "FileName";
            this.olvColumnFileName.FillsFreeSpace = true;
            this.olvColumnFileName.Text = "Name";
            // 
            // olvColumnFileSize
            // 
            this.olvColumnFileSize.AspectName = "FileSize";
            this.olvColumnFileSize.Text = "Size (KB)";
            this.olvColumnFileSize.Width = 109;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(879, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // checkBoxMp3
            // 
            this.checkBoxMp3.AutoSize = true;
            this.checkBoxMp3.Location = new System.Drawing.Point(72, 4);
            this.checkBoxMp3.Name = "checkBoxMp3";
            this.checkBoxMp3.Size = new System.Drawing.Size(70, 17);
            this.checkBoxMp3.TabIndex = 4;
            this.checkBoxMp3.Text = "Only mp3";
            this.checkBoxMp3.UseVisualStyleBackColor = true;
            this.checkBoxMp3.CheckedChanged += new System.EventHandler(this.checkBoxMp3_CheckedChanged);
            // 
            // contextMenuStripCacheFiles
            // 
            this.contextMenuStripCacheFiles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAsToolStripMenuItem});
            this.contextMenuStripCacheFiles.Name = "contextMenuStripCacheFiles";
            this.contextMenuStripCacheFiles.Size = new System.Drawing.Size(122, 26);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.saveAsToolStripMenuItem.Text = "Save as...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // olvColumnMediaLength
            // 
            this.olvColumnMediaLength.AspectName = "MediaLength";
            this.olvColumnMediaLength.AspectToStringFormat = "";
            this.olvColumnMediaLength.DisplayIndex = 2;
            this.olvColumnMediaLength.IsVisible = false;
            this.olvColumnMediaLength.Text = "Media length";
            this.olvColumnMediaLength.Width = 80;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 305);
            this.Controls.Add(this.checkBoxMp3);
            this.Controls.Add(this.fastObjectListViewCacheFiles);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "CacheExplorer";
            ((System.ComponentModel.ISupportInitialize)(this.fastObjectListViewCacheFiles)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStripCacheFiles.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.FastObjectListView fastObjectListViewCacheFiles;
        private BrightIdeasSoftware.OLVColumn olvColumnFileName;
        private BrightIdeasSoftware.OLVColumn olvColumnFileSize;
        private BrightIdeasSoftware.OLVColumn olvColumnCreateDate;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxMp3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripCacheFiles;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private BrightIdeasSoftware.OLVColumn olvColumnMediaLength;
    }
}

