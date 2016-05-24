using CacheExplorer.Model;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheExplorer.Helper
{
    public static class CacheHelper
    {
        private static readonly string LocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private const string ChromePath = @"\Google\Chrome\User Data\Default\Cache";
        private static readonly string TempDir = Environment.CurrentDirectory + @"\temp\";

        public static string ChromeCachePath
        {
            get
            {
                return LocalPath + ChromePath;
            }
        }

        public static IEnumerable<CacheFile> GetFiles(bool onlyMP3)
        {
            if (!CreateTempDir())
            {
                return null;
            }

            var files = Directory.GetFiles(ChromeCachePath, "f_*");
            var cacheFiles = files.Select(o => new CacheFile { FilePath = o, FileName = Path.GetFileName(o), Content = File.ReadAllBytes(o), CreateDate = File.GetCreationTime(o) }).ToList();
            if (!onlyMP3)
            {
                CleanupTempDir();
                return cacheFiles;
            }
            var mp3Files = cacheFiles.AsParallel().Where(o => IsMp3(o));
            CleanupTempDir();
            return FindFilesAndMerge(mp3Files.ToList());
        }

        private static bool CreateTempDir()
        {
            if (!Directory.Exists(TempDir))
            {
                try
                {
                    Directory.CreateDirectory(TempDir);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            if (Directory.GetFiles(TempDir).Any())
            {
                CleanupTempDir();
            }
            return true;
        }

        private static void CleanupTempDir()
        {
            foreach (var file in Directory.GetFiles(TempDir))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        private static bool IsMp3(CacheFile file)
        {
            var tempFileName = $"{Guid.NewGuid()}.mp3";
            var tempFile = $@"{TempDir}\{tempFileName}";

            if (!Directory.Exists(TempDir))
            {
                return false;
            }

            File.WriteAllBytes(tempFile, file.Content);
            var so = ShellFile.FromFilePath(tempFile);
            long nanoseconds;
            if (so.Properties.System.Media.Duration.Value == null)
            {
                return false;
            }
            if (!long.TryParse(so.Properties.System.Media.Duration.Value.ToString(),
            out nanoseconds))
            {
                return false;
            }

            if (nanoseconds > 0)
            {
                file.Length = nanoseconds;
                return true;
            }
            return false;
        }

        public static List<CacheFile> FindFilesAndMerge(List<CacheFile> files)
        {
            var fileParts = FindFiles(files).Where(o => o.Any());
            var mergedFiles = new List<CacheFile>();
            foreach (var file in fileParts)
            {
                var mergedFile = Merge(file);
                mergedFiles.Add(mergedFile);
            }
            return mergedFiles;
        }

        private static List<List<CacheFile>> FindFiles(List<CacheFile> files)
        {
            var foundFiles = new List<List<CacheFile>>();

            var singleFile = new List<CacheFile>();

            var firstFile = files.FirstOrDefault();
            if (firstFile == null)
            {
                return foundFiles;
            }
            var currentTime = firstFile.CreateDate;
            foreach (var item in files)
            {
                if (item.CreateDate <= currentTime.AddSeconds(2))
                {
                    singleFile.Add(item);
                    currentTime = item.CreateDate;
                    continue;
                }
                currentTime = item.CreateDate;
                foundFiles.Add(singleFile.ToList());
                singleFile = new List<CacheFile>();
            }
            if (singleFile.Any())
            {
                foundFiles.Add(singleFile.ToList());
            }

            return foundFiles;
        }

        private static CacheFile Merge(List<CacheFile> files)
        {
            var mergedFile = new CacheFile();
            mergedFile.Content = files.SelectMany(o => o.Content).ToArray();
            mergedFile.CreateDate = files.First().CreateDate;
            mergedFile.FileName = $"mergedFile{mergedFile.CreateDate.ToString("yyyyMMddHHmmss")}";
            mergedFile.FilePath = null;
            mergedFile.Length = files.Sum(o => o.Length);
            return mergedFile;
        }
    }
}