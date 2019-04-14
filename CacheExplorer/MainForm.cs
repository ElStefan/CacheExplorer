using BrightIdeasSoftware;
using CacheExplorer.Helper;
using CacheExplorer.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CacheExplorer
{
    public partial class MainForm : Form
    {
        private bool _onlyMediaFiles;
        private bool _lastFiveMinuteFiles;
        private readonly ConcurrentDictionary<string, PlaybackModel> _playbackCache = new ConcurrentDictionary<string, PlaybackModel>();
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private bool _stop;

        private static readonly TextOverlay SavingOverlayText = new TextOverlay
        {
            BackColor = Color.LightGray,
            CornerRounding = 5f,
            BorderColor = Color.Black,
            BorderWidth = 1.2f,
            Transparency = 240,
            Font = new Font("Arial", 14),
            Text = "Saving file...",
            TextColor = Color.Black,
            Alignment = ContentAlignment.MiddleCenter
        };

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

            worker.DoWork += ReadPlaybackApi;
            worker.RunWorkerAsync();
            this.FormClosing += (sender, e) => _stop = true;
        }

        private void ReadPlaybackApi(object sender, DoWorkEventArgs e)
        {
            while(!_stop)
            {
                try
                {
                    var file = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Google Play Music Desktop Player\json_store\playback.json";
                    var playbackItem = Newtonsoft.Json.JsonConvert.DeserializeObject<PlaybackModel>(File.ReadAllText(file));
                    if (!string.IsNullOrEmpty(playbackItem.Id) && playbackItem.Id != "__")
                    {
                        playbackItem.StartDate = DateTime.Now - TimeSpan.FromMilliseconds(playbackItem.Time.Current);
                        _playbackCache.AddOrUpdate(playbackItem.Id, playbackItem, (key,value) => playbackItem);                        
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception)
                {
                    //ignore
                }
                
            }
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
            SaveFile(false).ConfigureAwait(false);
        }

        private void saveAsSingleFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(true).ConfigureAwait(false);
        }

        private async Task SaveFile(bool merge)
        {
            this.fastObjectListViewCacheFiles.OverlayText = SavingOverlayText;
            var items = this.fastObjectListViewCacheFiles.SelectedObjects.Cast<CacheFile>().ToList();
            string fileName;
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

                fileName = saveFileDialog.FileName;
            }

            await Task.Run(() =>
            {
                SaveFiles(fileName, items, merge);
            });
            this.fastObjectListViewCacheFiles.OverlayText = null;
        }

        private void SaveFiles(string  fileName, List<CacheFile> items, bool merge)
        {
            var orderedItems = items.OrderBy(o => o.FileName).ToList();
            for (var i = 0; i < orderedItems.Count; i++)
            {
                var path = Path.GetDirectoryName(fileName);
                var filename = $@"{path}\{Path.GetFileNameWithoutExtension(fileName)}";
                var extension = Path.GetExtension(fileName);
                if (orderedItems.Count > 1)
                {
                    filename = $"{filename}_{i}";
                }

                byte[] content;
                if (merge)
                {
                    content = orderedItems.SelectMany(o => o.Content).ToArray();
                    i = orderedItems.Count;
                    filename = $@"{path}\{Path.GetFileNameWithoutExtension(fileName)}";
                }
                else
                {
                    content = orderedItems[i].Content;
                }

                var fullFilename = $"{filename}{extension}";
                File.WriteAllBytes(fullFilename, content);

                if(extension.EndsWith("m4a", StringComparison.OrdinalIgnoreCase))
                {
                    ConvertM4A(fullFilename);
                    fullFilename = $"{filename}.mp3";
                }

                RecognizeFile(fullFilename, merge ? orderedItems : null);
            }
        }

        private void RecognizeFile(string filePath, List<CacheFile> cacheFiles)
        {
            var match = TryReadPlaybackApi(cacheFiles);
            if (match == null)
            {
                match = AcrCloudHelper.RecognizeFile(filePath);
            }
            var lyrics = match.lyrics;
            match = iTunesHelper.ImproveResult(match);
            if(match != null)
            {
                match.lyrics = lyrics;
                TagLibHelper.SetFileInfos(filePath, match);
                RenameFileByTag(filePath);
            }

        }

        private Result TryReadPlaybackApi(List<CacheFile> cacheFiles)
        {
            if(cacheFiles == null)
            {
                return null;
            }
            var earliestStartDate = cacheFiles.Max(o => o.CreateDate).AddSeconds(-10); // min time to download before playing
            var latestStartDate = cacheFiles.Max(o => o.CreateDate).AddSeconds(30); // max time between download and before playing next song
            var playbackItem = _playbackCache.Values.OrderBy(o => o.StartDate).FirstOrDefault(o => o.StartDate > earliestStartDate && o.StartDate < latestStartDate);
            if (playbackItem == null)
            {
                return null;
            }

            return new Result { GpmdpDataAvailable = true, trackName = playbackItem.Song.Title, artistName = playbackItem.Song.Artist, collectionName = playbackItem.Song.Album, lyrics = playbackItem.SongLyrics };
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
            Task.Run(() =>
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    RecognizeFile(file, null);
                }
            });
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

        private static void RenameFileByTag(string file)
        {
            var directory = Path.GetDirectoryName(file);
            var tagLibFile = TagLib.File.Create(file);
#pragma warning disable CS0618 // using obsolete property "FirstArtist" because the real property is null...
            var newFileName = (tagLibFile.Tag.FirstAlbumArtist ?? tagLibFile.Tag.FirstArtist) + " - " + tagLibFile.Tag.Title + Path.GetExtension(file);
#pragma warning restore CS0618
            newFileName = newFileName.Replace("?", "_");
            File.Move(file, Path.Combine(directory, newFileName));
        }
    }
}