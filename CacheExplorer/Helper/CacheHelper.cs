using CacheExplorer.Extensions;
using CacheExplorer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CacheExplorer.Helper
{
    public static class CacheHelper
    {
        //private static readonly string LocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static readonly string LocalPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        //private const string ChromePath = @"\Google\Chrome\User Data\Default\Cache";
        private const string ChromePath = @"\Google Play Music Desktop Player\Cache";

        private static readonly string TempDir = Environment.CurrentDirectory + @"\temp\";

        private static string ChromeCachePath
        {
            get
            {
                return LocalPath + ChromePath;
                //return @"F:\temp\test";
            }
        }

        public static IEnumerable<CacheFile> GetFiles(bool onlyMediaFiles, DateTime? fromDate)
        {
            if (!CreateTempDir())
            {
                return null;
            }

            var files = Directory.GetFiles(ChromeCachePath, "f_*");
            var cacheFiles = files.AsParallel().Select(o => new CacheFile { FilePath = o, FileName = Path.GetFileName(o), Content = File.ReadAllBytes(o), CreateDate = File.GetCreationTime(o) });
            if (fromDate.HasValue)
            {
                cacheFiles = cacheFiles.Where(o => o.CreateDate >= fromDate.Value);
            }
            if (!onlyMediaFiles)
            {
                CleanupTempDir();
                return cacheFiles;
            }
            var mediaFiles = cacheFiles.Where(o => HasMediaLenght(o));
            CleanupTempDir();
            return FindFilesAndMerge(mediaFiles).Where(o => o.Length > 0);
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

        private static bool HasMediaLenght(CacheFile file)
        {
            // using taglib to find files with media length but duration seems to be wrong..
            try
            {
                using (var tagFile = TagLib.File.Create(file.FilePath, "taglib/mp4", TagLib.ReadStyle.Average))
                {
                    file.Length = tagFile.Properties.Duration.Ticks;
                }
            }
            catch (Exception)
            {
                //ignore
            }
            return true;
        }

        public static IEnumerable<CacheFile> FindFilesAndMerge(IEnumerable<CacheFile> files)
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

        private static IEnumerable<IEnumerable<CacheFile>> FindFiles(IEnumerable<CacheFile> files)
        {
            // TODO suche nach 32k, zähle immer *2 hoch bis 1024k solange die dateien zeitlich beieinander liegen und nimm die erste die kleiner als 1024k ist dazu und dann ist es eine datei.
            // ignoriere kleinere dateien dazwischen

            var foundFiles = new List<List<CacheFile>>();
            var singleFile = new List<CacheFile>();

            files = files.OrderBy(o => o.FileName);
            var firstFile = files.FirstOrDefault(o => Math.Abs(o.FileSize - Math.Pow(2, 15)) < double.Epsilon); // 32k
            if (firstFile == null)
            {
                return foundFiles;
            }
            var currentTime = firstFile.CreateDate;
            var currentFileSize = firstFile.FileSize;
            foreach (var item in files)
            {
                if (item.CreateDate <= currentTime.AddSeconds(2) && (item.FileSize == 2 * currentFileSize || Math.Abs(item.FileSize - Math.Pow(2, 20)) < double.Epsilon || (item.FileSize < currentFileSize && Math.Abs(currentFileSize - Math.Pow(2, 20)) < double.Epsilon))) // next file was created within 2 seconds
                {
                    singleFile.Add(item); // is it 2 times bigger than before? || is it 1024? || was it 1024 before && is now smaller? then it belongs to the file before
                    currentTime = item.CreateDate;
                    currentFileSize = item.FileSize;
                    continue;
                }

                // next file is older than 2 seconds before, so it's a new file
                currentTime = item.CreateDate;
                currentFileSize = item.FileSize;
                foundFiles.Add(singleFile);
                singleFile = new List<CacheFile> { item };
            }

            if (singleFile.Any())
            {
                foundFiles.Add(singleFile);
            }

            return foundFiles;
        }

        private static CacheFile Merge(IEnumerable<CacheFile> files)
        {
            var mergedFile = new CacheFile();
            mergedFile.Content = files.SelectMany(o => o.Content).ToArray();
            mergedFile.CreateDate = files.First().CreateDate;
            mergedFile.FileName = $"MergedFile{mergedFile.CreateDate.ToString("yyyyMMddHHmmss")}";
            mergedFile.FilePath = null;
            mergedFile.Length = files.Sum(o => o.Length);
            return mergedFile;
        }
    }
}