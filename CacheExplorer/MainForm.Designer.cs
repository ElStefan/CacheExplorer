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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.fastObjectListViewCacheFiles = new BrightIdeasSoftware.FastObjectListView();
      this.olvColumnCreateDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumnFileName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumnFileSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.contextMenuStripCacheFiles = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.browserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.checkBoxLastTenMinutes = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.fastObjectListViewCacheFiles)).BeginInit();
      this.contextMenuStripCacheFiles.SuspendLayout();
      this.menuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // fastObjectListViewCacheFiles
      // 
      this.fastObjectListViewCacheFiles.AllColumns.Add(this.olvColumnCreateDate);
      this.fastObjectListViewCacheFiles.AllColumns.Add(this.olvColumnFileName);
      this.fastObjectListViewCacheFiles.AllColumns.Add(this.olvColumnFileSize);
      this.fastObjectListViewCacheFiles.AllowDrop = true;
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
      this.fastObjectListViewCacheFiles.EmptyListMsg = "No files found";
      this.fastObjectListViewCacheFiles.EmptyListMsgFont = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.fastObjectListViewCacheFiles.FullRowSelect = true;
      this.fastObjectListViewCacheFiles.GridLines = true;
      this.fastObjectListViewCacheFiles.HideSelection = false;
      this.fastObjectListViewCacheFiles.Location = new System.Drawing.Point(12, 27);
      this.fastObjectListViewCacheFiles.Name = "fastObjectListViewCacheFiles";
      this.fastObjectListViewCacheFiles.ShowGroups = false;
      this.fastObjectListViewCacheFiles.Size = new System.Drawing.Size(855, 547);
      this.fastObjectListViewCacheFiles.TabIndex = 0;
      this.fastObjectListViewCacheFiles.UseCompatibleStateImageBehavior = false;
      this.fastObjectListViewCacheFiles.View = System.Windows.Forms.View.Details;
      this.fastObjectListViewCacheFiles.VirtualMode = true;
      this.fastObjectListViewCacheFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.fastObjectListViewCacheFiles_DragDrop);
      this.fastObjectListViewCacheFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.fastObjectListViewCacheFiles_DragEnter);
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
      // menuStrip1
      // 
      this.menuStrip1.AllowDrop = true;
      this.menuStrip1.BackColor = System.Drawing.SystemColors.ButtonFace;
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.browserToolStripMenuItem});
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
      // browserToolStripMenuItem
      // 
      this.browserToolStripMenuItem.Name = "browserToolStripMenuItem";
      this.browserToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
      this.browserToolStripMenuItem.Text = "Browser";
      this.browserToolStripMenuItem.Click += new System.EventHandler(this.browserToolStripMenuItem_Click);
      // 
      // checkBoxLastTenMinutes
      // 
      this.checkBoxLastTenMinutes.AutoSize = true;
      this.checkBoxLastTenMinutes.Location = new System.Drawing.Point(127, 4);
      this.checkBoxLastTenMinutes.Name = "checkBoxLastTenMinutes";
      this.checkBoxLastTenMinutes.Size = new System.Drawing.Size(143, 17);
      this.checkBoxLastTenMinutes.TabIndex = 5;
      this.checkBoxLastTenMinutes.Text = "Only from last 10 minutes";
      this.checkBoxLastTenMinutes.UseVisualStyleBackColor = true;
      this.checkBoxLastTenMinutes.CheckedChanged += new System.EventHandler(this.checkBoxLastTenMinutes_CheckedChanged);
      // 
      // MainForm
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(879, 586);
      this.Controls.Add(this.checkBoxLastTenMinutes);
      this.Controls.Add(this.fastObjectListViewCacheFiles);
      this.Controls.Add(this.menuStrip1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "MainForm";
      this.Text = "CacheExplorer";
      ((System.ComponentModel.ISupportInitialize)(this.fastObjectListViewCacheFiles)).EndInit();
      this.contextMenuStripCacheFiles.ResumeLayout(false);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
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
        private System.Windows.Forms.ContextMenuStrip contextMenuStripCacheFiles;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxLastTenMinutes;
    private System.Windows.Forms.ToolStripMenuItem browserToolStripMenuItem;
  }
}

