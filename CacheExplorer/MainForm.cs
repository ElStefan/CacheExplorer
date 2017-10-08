using BrightIdeasSoftware;
using CacheExplorer.Helper;
using CacheExplorer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CacheExplorer
{
    public partial class MainForm : Form
    {
        private bool _onlyMediaFiles;
        private bool _lastFiveMinuteFiles;

        private static readonly TextOverlay OverlayText = new TextOverlay
        {
            BackColor = Color.LightGray,
            CornerRounding = 5f,
            BorderColor = Color.Black,
            BorderWidth = 1.2f,
            Transparency = 240,
            Font = new Font("Arial", 14),
            Text = "Loading...",
            TextColor = Color.Black
        };

        public MainForm()
        {
            this.InitializeComponent();

            // work in progress
             // var indexFile = CacheHelper.ReadIndexFile();
            // var dataFiles = CacheHelper.ReadDataFiles();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LoadFilesAsync();
        }

        private async void LoadFilesAsync()
        {
            this.fastObjectListViewCacheFiles.EmptyListMsg = "";
            this.fastObjectListViewCacheFiles.OverlayText = OverlayText;
            this.fastObjectListViewCacheFiles.ClearObjects();
            this.olvColumnMediaLength.IsVisible = this._onlyMediaFiles;
            DateTime? fromDate = null;
            if (this._lastFiveMinuteFiles)
            {
                fromDate = DateTime.Now.AddMinutes(-5);
            }
            var files = await Task.Run(() => CacheHelper.GetFiles(this._onlyMediaFiles, fromDate));
            this.fastObjectListViewCacheFiles.SetObjects(files);
            this.fastObjectListViewCacheFiles.RebuildColumns();
            this.fastObjectListViewCacheFiles.OverlayText = null;
            this.fastObjectListViewCacheFiles.EmptyListMsg = "No files found";
        }

        private void checkBoxOnlyMedia_CheckedChanged(object sender, EventArgs e)
        {
            this._onlyMediaFiles = this.checkBoxOnlyMedia.Checked;
            this.LoadFilesAsync();
        }

        private void checkBoxLastFiveMinutes_CheckedChanged(object sender, EventArgs e)
        {
            this._lastFiveMinuteFiles = this.checkBoxLastFiveMinutes.Checked;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(false);
        }

        private void SaveFile(bool merge)
        {
            var items = this.fastObjectListViewCacheFiles.SelectedObjects.Cast<CacheFile>().ToList();
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Save files as new file";
                saveFileDialog.FileName = $"CacheFiles{DateTime.Now.ToString("yyyyMMddHHmmss")}";
                saveFileDialog.DefaultExt = "m4a";
                var result = saveFileDialog.ShowDialog();
                if (result != DialogResult.OK)
                {
                    return;
                }
                SaveFiles(saveFileDialog, items, merge);
            }
        }

        private static void SaveFiles(SaveFileDialog saveFileDialog, List<CacheFile> items, bool merge)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var path = Path.GetDirectoryName(saveFileDialog.FileName);
                var filename = $@"{path}\{Path.GetFileNameWithoutExtension(saveFileDialog.FileName)}";
                var extension = Path.GetExtension(saveFileDialog.FileName);
                if (items.Count > 1)
                {
                    filename = $"{filename}_{i}";
                }

                byte[] content;
                if (merge)
                {
                    content = items.SelectMany(o => o.Content).ToArray();
                    i = items.Count;
                    filename = $@"{path}\{Path.GetFileNameWithoutExtension(saveFileDialog.FileName)}";
                }
                else
                {
                    content = items[i].Content;
                }

                var fullFilename = $"{filename}{extension}";
                File.WriteAllBytes(fullFilename, content);

                if(extension.EndsWith("m4a", StringComparison.OrdinalIgnoreCase))
                {
                    ConvertM4A(fullFilename);
                    fullFilename = $"{filename}.mp3";
                }

                RecognizeFile(fullFilename);
            }
        }

        private static void RecognizeFile(string filePath)
        {
            AcrCloudHelper.RecognizeFile(filePath);
        }

        private void fastObjectListViewCacheFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void fastObjectListViewCacheFiles_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                AcrCloudHelper.RecognizeFile(file);
            }
        }

        private static void ConvertM4A(string sourceFile)
        {
            using (var engine = new MediaToolkit.Engine())
            {
                var newFileName = sourceFile.Replace("m4a","mp3");

                engine.CustomCommand($@"-i ""{sourceFile}"" -acodec libmp3lame -ab 320k {newFileName}");
                
                File.Delete(sourceFile);
            }
        }


        private void saveAsSingleFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(true);
        }
    }
}