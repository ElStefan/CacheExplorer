namespace CacheExplorer
{
    partial class TagSelectionDialog
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
            this.fastObjectListViewMatches = new BrightIdeasSoftware.FastObjectListView();
            this.olvColumnTrackName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnArtist = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnAlbum = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnReleaseDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.textBoxArtist = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxAlbum = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.labelSource = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.fastObjectListViewMatches)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // fastObjectListViewMatches
            // 
            this.fastObjectListViewMatches.AllColumns.Add(this.olvColumnTrackName);
            this.fastObjectListViewMatches.AllColumns.Add(this.olvColumnArtist);
            this.fastObjectListViewMatches.AllColumns.Add(this.olvColumnAlbum);
            this.fastObjectListViewMatches.AllColumns.Add(this.olvColumnReleaseDate);
            this.fastObjectListViewMatches.CellEditUseWholeCell = false;
            this.fastObjectListViewMatches.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnTrackName,
            this.olvColumnArtist,
            this.olvColumnAlbum,
            this.olvColumnReleaseDate});
            this.fastObjectListViewMatches.Cursor = System.Windows.Forms.Cursors.Default;
            this.fastObjectListViewMatches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fastObjectListViewMatches.EmptyListMsg = "None";
            this.fastObjectListViewMatches.FullRowSelect = true;
            this.fastObjectListViewMatches.GridLines = true;
            this.fastObjectListViewMatches.HideSelection = false;
            this.fastObjectListViewMatches.Location = new System.Drawing.Point(3, 16);
            this.fastObjectListViewMatches.MultiSelect = false;
            this.fastObjectListViewMatches.Name = "fastObjectListViewMatches";
            this.fastObjectListViewMatches.RowHeight = 22;
            this.fastObjectListViewMatches.ShowGroups = false;
            this.fastObjectListViewMatches.Size = new System.Drawing.Size(808, 421);
            this.fastObjectListViewMatches.TabIndex = 0;
            this.fastObjectListViewMatches.UseCompatibleStateImageBehavior = false;
            this.fastObjectListViewMatches.UseFilterIndicator = true;
            this.fastObjectListViewMatches.UseFiltering = true;
            this.fastObjectListViewMatches.View = System.Windows.Forms.View.Details;
            this.fastObjectListViewMatches.VirtualMode = true;
            // 
            // olvColumnTrackName
            // 
            this.olvColumnTrackName.AspectName = "trackName";
            this.olvColumnTrackName.Text = "Title";
            this.olvColumnTrackName.Width = 200;
            // 
            // olvColumnArtist
            // 
            this.olvColumnArtist.AspectName = "artistName";
            this.olvColumnArtist.Text = "Artist";
            this.olvColumnArtist.Width = 200;
            // 
            // olvColumnAlbum
            // 
            this.olvColumnAlbum.AspectName = "collectionName";
            this.olvColumnAlbum.Text = "Album";
            this.olvColumnAlbum.Width = 200;
            // 
            // olvColumnReleaseDate
            // 
            this.olvColumnReleaseDate.AspectName = "releaseDate";
            this.olvColumnReleaseDate.Text = "ReleaseDate";
            this.olvColumnReleaseDate.Width = 100;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.fastObjectListViewMatches);
            this.groupBox1.Location = new System.Drawing.Point(12, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(814, 440);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Found multiple tags:";
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(670, 505);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(751, 505);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Found data from:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Title:";
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.Location = new System.Drawing.Point(56, 33);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.Size = new System.Drawing.Size(202, 20);
            this.textBoxTitle.TabIndex = 6;
            // 
            // textBoxArtist
            // 
            this.textBoxArtist.Location = new System.Drawing.Point(305, 33);
            this.textBoxArtist.Name = "textBoxArtist";
            this.textBoxArtist.Size = new System.Drawing.Size(206, 20);
            this.textBoxArtist.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(264, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Artist:";
            // 
            // textBoxAlbum
            // 
            this.textBoxAlbum.Location = new System.Drawing.Point(558, 33);
            this.textBoxAlbum.Name = "textBoxAlbum";
            this.textBoxAlbum.Size = new System.Drawing.Size(195, 20);
            this.textBoxAlbum.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(517, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Album:";
            // 
            // buttonSearch
            // 
            this.buttonSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSearch.Location = new System.Drawing.Point(759, 31);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(64, 23);
            this.buttonSearch.TabIndex = 11;
            this.buttonSearch.Text = "Search";
            this.buttonSearch.UseVisualStyleBackColor = false;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // labelSource
            // 
            this.labelSource.AutoSize = true;
            this.labelSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSource.Location = new System.Drawing.Point(105, 9);
            this.labelSource.Name = "labelSource";
            this.labelSource.Size = new System.Drawing.Size(68, 13);
            this.labelSource.TabIndex = 12;
            this.labelSource.Text = "ACR Cloud";
            // 
            // TagSelectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 540);
            this.Controls.Add(this.labelSource);
            this.Controls.Add(this.buttonSearch);
            this.Controls.Add(this.textBoxAlbum);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxArtist);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBox1);
            this.Name = "TagSelectionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TagSelectionDialog";
            ((System.ComponentModel.ISupportInitialize)(this.fastObjectListViewMatches)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.FastObjectListView fastObjectListViewMatches;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private BrightIdeasSoftware.OLVColumn olvColumnTrackName;
        private BrightIdeasSoftware.OLVColumn olvColumnArtist;
        private BrightIdeasSoftware.OLVColumn olvColumnAlbum;
        private BrightIdeasSoftware.OLVColumn olvColumnReleaseDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxTitle;
        private System.Windows.Forms.TextBox textBoxArtist;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxAlbum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.Label labelSource;
    }
}