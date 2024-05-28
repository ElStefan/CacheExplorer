using BrightIdeasSoftware;
using CacheExplorer.Helper;
using CacheExplorer.Model;
using CacheExplorer.Properties;
using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CacheExplorer
{
  public partial class MainForm : Form
  {
    private const string SavedFilesCachePath = ".\\SavedFiles.txt";
    private bool _lastTenMinuteFiles;
    private bool _stop;
    private static readonly List<DownloadFile> _downloadFileCache = new List<DownloadFile>();
    private static readonly HashSet<string> _savedFiles = new HashSet<string>();

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
    private readonly string DefaultWorkingFolder;
    private readonly string MusicFolder;

    public MainForm()
    {
      DefaultWorkingFolder = Settings.Default.WorkingFolder;
      MusicFolder = Settings.Default.MusicFolder;

      // save appsettings to user configuration file if not existing
      if (string.IsNullOrEmpty(DefaultWorkingFolder) || string.IsNullOrEmpty(MusicFolder))
      {
        if (string.IsNullOrEmpty(DefaultWorkingFolder))
        {
          var tempRoot = Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "temp");
          Settings.Default.WorkingFolder = tempRoot;
        }

        if (string.IsNullOrEmpty(MusicFolder))
        {
          var path1 = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
          var itunesMediaFolder = Path.Combine(path1, "iTunes\\iTunes Media\\Music");
          Settings.Default.MusicFolder = itunesMediaFolder;
        }

        Settings.Default.Save();
      }

      Task.Run(() =>
      {
        if (!Directory.Exists(".\\ytdl"))
        {
          Directory.CreateDirectory(".\\ytdl");
          YoutubeDLSharp.Utils.DownloadYtDlp("ytdl").GetAwaiter().GetResult();
          YoutubeDLSharp.Utils.DownloadFFmpeg("ytdl").GetAwaiter().GetResult();
        }
      });

      var cefSettings = new CefSettings
      {
        CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache"),
        UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36 /CefSharp Browser" + Cef.CefSharpVersion
      };
      Cef.Initialize(cefSettings);

      InitializeComponent();

      FormClosing += (sender, e) => _stop = true;

      olvColumnFileName.AspectGetter = x =>
      {
        if (x is DownloadFile file)
        {
          return file.FileName;
        }

        return null;
      };

      if (File.Exists(SavedFilesCachePath))
      {
        _savedFiles.UnionWith(File.ReadAllLines(SavedFilesCachePath));
      }

      Task.Run(AutoTaggingTask);
    }

    private async void AutoTaggingTask()
    {
      while (!_stop)
      {
        await Task.Delay(TimeSpan.FromSeconds(10));
        var files = GetDownloadsSince(DateTime.Now.AddMinutes(-10))
          .Where(f => !_savedFiles.Contains(f.FileName))
          .OrderBy(o => o.CreateDate)
          .ToList();

        if (files.Count == 0)
        {
          Debug.WriteLine("No new files found");
          continue;
        }

        foreach (var item in files)
        {
          var result = ReadExistingTags(item.FilePath);
          if (result == null)
          {
            Debug.WriteLine($"Failed to read tags for {item.FileName}");
            continue;
          }

          var savePath = DefaultWorkingFolder;
          var filename = $"{result.artistName} - {result.trackName}.mp3";
          foreach (var character in Path.GetInvalidFileNameChars())
          {
            filename = filename.Replace(character, '_');
          }

          var tempFilename = $"{Path.Combine(savePath, item.FileName)}";
          var foundInFolderAsFileName = File.Exists(Path.Combine(savePath, item.FileName + ".mp3"));
          var normalizedFilePath = Path.Combine(savePath, filename);
          var foundInFolderAsNormalizedFile = File.Exists(normalizedFilePath);
          var existsInLibrary = ExistsInLibrary(result.artistName, result.trackName);
          if (foundInFolderAsFileName || foundInFolderAsNormalizedFile || existsInLibrary)
          {
            Debug.WriteLine($"Found existing file: {item.FileName} (or {filename}), Reason:\n" +
                            $"Found in folder as downloaded file: {foundInFolderAsFileName}\n" +
                            $"Found in folder as normalized:      {foundInFolderAsNormalizedFile}\n" +
                            $"Exists in library:                  {existsInLibrary}");
            if (existsInLibrary)
            {
              File.Delete(item.FilePath);
              if (foundInFolderAsNormalizedFile)
              {
                File.Delete(normalizedFilePath);
              }
            }

            _savedFiles.Add(item.FileName);
            File.WriteAllLines(SavedFilesCachePath, _savedFiles);
            continue;
          }

          Debug.WriteLine($"Save new file: {filename}");
          await SaveFileAsync(tempFilename, item, true);

          _savedFiles.Add(item.FileName);
          File.WriteAllLines(SavedFilesCachePath, _savedFiles);
        }
      }
    }

    private bool ExistsInLibrary(string artist, string title)
    {
      var path = $@"{MusicFolder}\{artist}";
      foreach (var item in Path.GetInvalidPathChars().Concat(new[] { '.', '/' }))
      {
        path = path.Replace(item, '_');
      }

      if (!Directory.Exists(path))
      {
        return false;
      }

      var filetitle = title;
      foreach (var character in Path.GetInvalidFileNameChars())
      {
        filetitle = title.Replace(character, '_');
      }

      filetitle = filetitle.Substring(0, Math.Min(20, filetitle.Length));

      var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
      return files.Any(o => o.IndexOf(filetitle, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
    {
      LoadFilesAsync();
    }

    private async void LoadFilesAsync()
    {
      fastObjectListViewCacheFiles.EmptyListMsg = "";
      fastObjectListViewCacheFiles.OverlayText = OverlayText;
      fastObjectListViewCacheFiles.ClearObjects();

      DateTime? fromDate = null;
      if (_lastTenMinuteFiles)
      {
        fromDate = DateTime.Now.AddMinutes(-10);
      }

      var files = await Task.Run(() => GetDownloadsSince(fromDate).OrderBy(o => o.CreateDate));
      fastObjectListViewCacheFiles.SetObjects(files);
      fastObjectListViewCacheFiles.RebuildColumns();
      fastObjectListViewCacheFiles.OverlayText = null;
      fastObjectListViewCacheFiles.EmptyListMsg = "No files found";
    }

    private static IEnumerable<DownloadFile> GetDownloadsSince(DateTime? fromDate)
    {
      var downloadFiles = RefreshDownloadedFiles();
      if (fromDate.HasValue)
      {
        downloadFiles = downloadFiles.Where(o => o.CreateDate >= fromDate.Value);
      }

      return downloadFiles;
    }

    private static IEnumerable<DownloadFile> RefreshDownloadedFiles()
    {
      if (!Directory.Exists(CustomResourceRequestHandler.OutputFolder))
      {
        return new List<DownloadFile>();
      }

      var files = Directory.GetFiles(CustomResourceRequestHandler.OutputFolder, "*.mp3")
        .Union(Directory.GetFiles(CustomResourceRequestHandler.OutputFolder, "*.m4a")).ToList();
      var downloadFiles = files.Where(f => _downloadFileCache.All(df => df.FilePath != f)).AsParallel().Select(o => new DownloadFile(o));
      _downloadFileCache.AddRange(downloadFiles);
      // remove files that no longer exist (= not in the new list)
      _downloadFileCache.RemoveAll(o => !files.Contains(o.FilePath));

      return _downloadFileCache;
    }

    private void checkBoxLastTenMinutes_CheckedChanged(object sender, EventArgs e)
    {
      _lastTenMinuteFiles = checkBoxLastTenMinutes.Checked;
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      SaveFile().ConfigureAwait(false);
    }

    private async Task SaveFile()
    {
      var item = fastObjectListViewCacheFiles.SelectedObject as DownloadFile;
      string fileName;
      using (var saveFileDialog = new SaveFileDialog())
      {
        saveFileDialog.Title = "Save files as new file";
        saveFileDialog.FileName = item?.FileName;
        var result = saveFileDialog.ShowDialog();
        if (result != DialogResult.OK)
        {
          return;
        }

        fileName = saveFileDialog.FileName;
      }

      fastObjectListViewCacheFiles.OverlayText = SavingOverlayText;
      await SaveFileAsync(fileName, item, false);
      fastObjectListViewCacheFiles.OverlayText = null;
    }

    private async Task SaveFileAsync(string fileName, DownloadFile item, bool automatic)
    {
      var path = Path.GetDirectoryName(fileName);
      var filename = $@"{path}\{Path.GetFileNameWithoutExtension(fileName)}";
      var extension = Path.GetExtension(fileName);

      var fullFilename = $"{filename}{extension}";
      File.WriteAllBytes(fullFilename, item.Content);

      if (extension.EndsWith("m4a", StringComparison.OrdinalIgnoreCase))
      {
        await ConvertM4A(fullFilename);
        fullFilename = $"{filename}.mp3";
      }

      RecognizeFile(fullFilename, automatic);
    }

    private void RecognizeFile(string filePath, bool automatic)
    {
      var match = ReadExistingTags(filePath);
      if (!match.AllInfosGiven)
      {
        match = AcrCloudHelper.RecognizeFile(filePath);
      }

      if (match == null)
      {
        MessageBox.Show("Could not get any data");
        return;
      }

      var lyrics = match.lyrics;
      match = iTunesHelper.ImproveResult(match, automatic);
      if (match != null)
      {
        match.lyrics = lyrics;
        TagLibHelper.SetFileInfos(filePath, match);
        RenameFileByTag(filePath);
      }
    }

    private Result ReadExistingTags(string filePath)
    {
      var fileInfos = TagLibHelper.GetFileInfos(filePath);
      return new Result
      {
        trackName = fileInfos.Tag.Title,
        artistName = fileInfos.Tag.FirstPerformer,
        collectionName = fileInfos.Tag.Album,
        lyrics = fileInfos.Tag.Lyrics // optional
      };
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
        foreach (var file in files)
        {
          RecognizeFile(file, false);
        }
      });
    }

    private static async Task ConvertM4A(string sourceFile)
    {
      try
      {
        var newFileName = sourceFile.Replace("m4a", "mp3");
        await FFMpegCore.FFMpegArguments.FromFileInput(sourceFile)
            .OutputToFile(newFileName, true, options =>
            options.UsingMultithreading(true)
            .WithAudioBitrate(FFMpegCore.Enums.AudioQuality.VeryHigh)
            .WithAudioCodec(FFMpegCore.Enums.AudioCodec.LibMp3Lame))
            .ProcessAsynchronously().ConfigureAwait(false);

        File.Delete(sourceFile);
      }
      catch (Exception exception)
      {
        Console.WriteLine(exception);
      }
    }

    private static void RenameFileByTag(string file)
    {
      var directory = Path.GetDirectoryName(file) ?? string.Empty;
      var tagLibFile = TagLib.File.Create(file);
#pragma warning disable CS0618 // using obsolete property "FirstArtist" because the real property is null...
      var newFileName = (tagLibFile.Tag.FirstAlbumArtist ?? tagLibFile.Tag.FirstArtist) + " - " + tagLibFile.Tag.Title + Path.GetExtension(file);
#pragma warning restore CS0618
      foreach (var item in Path.GetInvalidFileNameChars())
      {
        newFileName = newFileName.Replace(item, '_');
      }

      var destFileName = Path.Combine(directory, newFileName);
      if (File.Exists(destFileName))
      {
        Debug.WriteLine($"File already exists: {destFileName}");
        File.Delete(file);
        return;
      }

      File.Move(file, destFileName);
    }

    private void browserToolStripMenuItem_Click(object sender, EventArgs e)
    {
      // Open a cefsharp browser
      var browser = new ChromiumWebBrowser();
      browser.RequestHandler = new CustomRequestHandler();
      browser.Dock = DockStyle.Fill;
      browser.Load("https://music.youtube.com");
      var form = new Form();
      form.Controls.Add(browser);
      form.Size = Size;
      form.StartPosition = FormStartPosition.CenterParent;
      form.Show();

      form.FormClosed += (s, args) =>
      {
        browser.Dispose();
        browser = null;
        form.Dispose();
      };

      // log every loaded network resource
      browser.LoadingStateChanged += (s, args) =>
      {
        if (!args.IsLoading)
        {
          return;
        }

        var url = args.Browser.MainFrame.Url;
        Debug.WriteLine("Browse to URL:" + url);
      };
    }
  }
}
