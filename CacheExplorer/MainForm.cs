using CacheExplorer.Helper;
using CacheExplorer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CacheExplorer
{
    public partial class MainForm : Form
    {
        private bool _onlyMp3;

        public MainForm()
        {
            this.InitializeComponent();
            this.LoadFiles();
            var timespan = new TimeSpan(1223245);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LoadFiles();
        }

        private async void LoadFiles()
        {
            this.olvColumnMediaLength.IsVisible = this._onlyMp3;
            var files = await Task.Run(() => CacheHelper.GetFiles(this._onlyMp3));
            this.fastObjectListViewCacheFiles.SetObjects(files);
            this.fastObjectListViewCacheFiles.RebuildColumns();
        }
        private void checkBoxMp3_CheckedChanged(object sender, EventArgs e)
        {
            this._onlyMp3 = this.checkBoxMp3.Checked;
            this.LoadFiles();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var items = this.fastObjectListViewCacheFiles.SelectedObjects.Cast<CacheFile>().ToList();
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Save files as new file";
                saveFileDialog.FileName = $"CacheFiles{DateTime.Now.ToString("yyyyMMddHHmm")}";
                saveFileDialog.DefaultExt = "mp3";
                var result = saveFileDialog.ShowDialog();
                if(result != DialogResult.OK)
                {
                    return;
                }
                this.SaveFiles(saveFileDialog, items);
            }
        }

        private void SaveFiles(SaveFileDialog saveFileDialog, List<CacheFile> items)
        {
            var content = items.SelectMany(o => o.Content).ToArray();
            File.WriteAllBytes(saveFileDialog.FileName, content);
        }
    }
}
