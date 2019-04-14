using CacheExplorer.Extensions;
using CacheExplorer.Model;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            // so if taglib found a valid file, we let the shell check for real duration
            // this part will hurt every ssd..

            //var tempFileName = $"{Guid.NewGuid()}.m4a";
            //var tempFile = $@"{TempDir}\{tempFileName}";

            //if (!Directory.Exists(TempDir))
            //{
            //    return false;
            //}
            //File.WriteAllBytes(tempFile, file.Content);
            //var so = ShellFile.FromFilePath(tempFile);

            //long nanoseconds;
            //while (true)
            //{
            //    try
            //    {
                    
            //        // crashes sometimes
            //        if (so.Properties.System.Media.Duration.Value == null)
            //        {
            //            return false;
            //        }

            //        if (!long.TryParse(so.Properties.System.Media.Duration.Value.ToString(),
            //        out nanoseconds))
            //        {
            //            return false;
            //        }
            //        break;
            //    }
            //    catch (Exception)
            //    {
            //        continue;
            //    }
            //}

            //if (nanoseconds > 0 || file.Length > 0)
            //{
            //    file.Length = nanoseconds;
            //    return true;
            //}

            return false;
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

        #region UNDER CONSTRUCTION

        public static CacheIndex ReadIndexFile()
        {
            using (var stream = new FileStream(ChromeCachePath + @"\Index", FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                var hexData = new List<string>();

                for (int i = 1; i < Int32.MaxValue; i++)
                {
                    var value = stream.ReadByte();
                    if (value == -1)
                    {
                        break;
                    }
                    hexData.Add(string.Format("{0:X2}", value));
                    //if (i % 16 == 0)
                    //{
                    //    sb.AppendLine();
                    //}
                }

                var cacheIndex = new CacheIndex();
                cacheIndex.MagicNumber = hexData.Take(4).Reverse().ConvertToString("");
                cacheIndex.Version = hexData.Skip(4).Take(4).Reverse().ConvertToString("");
                cacheIndex.Entries = Convert.ToInt32(hexData.Skip(8).Take(4).Reverse().ConvertToString(""), 16);
                cacheIndex.TotalStoreSize = Convert.ToInt32(hexData.Skip(12).Take(4).Reverse().ConvertToString(""), 16);
                cacheIndex.LastFile = hexData.Skip(16).Take(4).Reverse().ConvertToString("");
                cacheIndex.DirtyFlag = hexData.Skip(20).Take(4).Reverse().ConvertToString("");
                cacheIndex.Storage = hexData.Skip(24).Take(4).Reverse().ConvertToString("");
                cacheIndex.TableSize = Convert.ToInt32(hexData.Skip(28).Take(4).Reverse().ConvertToString(""), 16);
                cacheIndex.PreviousCrash = hexData.Skip(32).Take(4).Reverse().ConvertToString("");
                cacheIndex.TestId = hexData.Skip(36).Take(4).Reverse().ConvertToString("");
                cacheIndex.CreationTime = hexData.Skip(40).Take(8).Reverse().ConvertToString("");
                cacheIndex.PaddedContent = hexData.Skip(48).Take(208).Reverse().ConvertToString("");
                cacheIndex.Pad1 = hexData.Skip(256).Take(8).Reverse().ConvertToString("");
                cacheIndex.CacheFilledFlag = hexData.Skip(264).Take(4).Reverse().ConvertToString("");
                cacheIndex.Sizes = hexData.Skip(268).Take(20).Reverse().Split(4).Select(o => o.ConvertToString("")).ToList();
                cacheIndex.HeadsCacheAddress = hexData.Skip(288).Take(20).Reverse().Split(4).Select(o => o.ConvertToString("")).ToList();
                cacheIndex.TailsCacheAddress = hexData.Skip(308).Take(20).Reverse().Split(4).Select(o => o.ConvertToString("")).ToList();
                cacheIndex.TransActionCacheAddress = hexData.Skip(312).Take(4).Reverse().ConvertToString("");
                cacheIndex.ActualInFlightOperation = hexData.Skip(316).Take(4).Reverse().ConvertToString("");
                cacheIndex.InFlightOperationList = hexData.Skip(320).Take(4).Reverse().ConvertToString("");
                cacheIndex.Pad2 = hexData.Skip(324).Take(28).Reverse().ConvertToString("");
                cacheIndex.CacheAddresses = hexData.Skip(352).Take(4 * cacheIndex.TableSize).Split(32).Select(o => o./*Skip(28).Take(4).*/Reverse().ConvertToString("")).ToList();
                //cacheIndex.Unknown = hexData.Skip(352).Reverse().ConvertToString("");
                return cacheIndex;
            }
        }

        public static List<CacheData> ReadDataFiles()
        {
            var dataFiles = new List<CacheData>();
            var files = Directory.GetFiles(ChromeCachePath, "data_*");
            foreach (var filePath in files)
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                {
                    var hexData = new List<string>();

                    for (int i = 1; i < Int32.MaxValue; i++)
                    {
                        var value = stream.ReadByte();
                        if (value == -1)
                        {
                            break;
                        }
                        hexData.Add(string.Format("{0:X2}", value));
                    }
                    var cacheData = new CacheData();
                    cacheData.MagicNumber = hexData.Take(4).Reverse().ConvertToString("");
                    cacheData.Version = hexData.Skip(4).Take(4).Reverse().ConvertToString("");
                    cacheData.Index = hexData.Skip(8).Take(2).Reverse().ConvertToString("");
                    cacheData.NextFile = hexData.Skip(10).Take(2).Reverse().ConvertToString("");
                    cacheData.BlockSize = Convert.ToInt32(hexData.Skip(12).Take(4).Reverse().ConvertToString(""), 16);
                    cacheData.NumberOfStoredEntries = Convert.ToInt32(hexData.Skip(16).Take(4).Reverse().ConvertToString(""), 16);
                    cacheData.MaxNumberOfEntries = Convert.ToInt32(hexData.Skip(20).Take(4).Reverse().ConvertToString(""), 16);
                    cacheData.Counters = hexData.Skip(36).Take(16).Split(4).Select(o => o.Reverse().ConvertToString("")).ToList();
                    cacheData.LastPosition = hexData.Skip(52).Take(16).Reverse().ConvertToString("");
                    cacheData.TrackUpdates = hexData.Skip(56).Take(4).Reverse().ConvertToString("");
                    cacheData.User = hexData.Skip(60).Take(20).Reverse().ConvertToString("");
                    cacheData.AllocationBitmap = hexData.Skip(80).Take(2028).Split(4).Select(o => o.Reverse().ConvertToString("")).ToList();
                    if (filePath.EndsWith("0"))
                    {
                        cacheData.RankingsNodes = GetRankingsNodes(hexData.Skip(4 * 2048));
                    }
                    else
                    {
                        cacheData.CacheEntries = GetCacheEntries(hexData.Skip(4 * 2048));
                    }
                    var content = hexData.Skip(4 * 2048).Split(256).Select(o => o.Aggregate((i, j) => j + i)).ToList();
                    dataFiles.Add(cacheData);
                }
            }
            return dataFiles;
        }

        private static List<RankingsNode> GetRankingsNodes(IEnumerable<string> source)
        {
            var entries = new List<RankingsNode>();
            var enumerable = source.ToList();
            while (true)
            {
                if (!enumerable.Any())
                {
                    break;
                }
                var rankingsNode = new RankingsNode();
                var currentPos = 0;
                rankingsNode.LastUsed = enumerable.Skip(currentPos).Take(8).Reverse().ConvertToString("");
                currentPos += 8;
                rankingsNode.LastModified = enumerable.Skip(currentPos).Take(8).Reverse().ConvertToString("");
                currentPos += 8;
                rankingsNode.NextRankingsAddress = enumerable.Skip(currentPos).Take(4).Reverse().ConvertToString("");
                currentPos += 4;
                rankingsNode.PreviousRankingsAddress = enumerable.Skip(currentPos).Take(4).Reverse().ConvertToString("");
                currentPos += 4;
                rankingsNode.CacheEntryCacheAddress = enumerable.Skip(currentPos).Take(4).Reverse().ConvertToString("");
                currentPos += 4;
                rankingsNode.DirtyFlag = enumerable.Skip(currentPos).Take(4).Reverse().ConvertToString("");
                currentPos += 4;
                rankingsNode.SelfHash = enumerable.Skip(currentPos).Take(4).Reverse().ConvertToString("");
                currentPos += 4;

                entries.Add(rankingsNode);
                enumerable = enumerable.Skip(36).ToList();
            }
            return entries;
        }

        private static List<CacheEntry> GetCacheEntries(IEnumerable<string> source)
        {
            var entries = new List<CacheEntry>();
            var enumerable = source.ToList();
            while (true)
            {
                if (!enumerable.Any())
                {
                    break;
                }
                var cacheEntry = new CacheEntry();
                var currentPos = 0;
                cacheEntry.Hash = enumerable.Skip(currentPos).Take(4).Reverse().ConvertToString("");
                currentPos += 4;
                cacheEntry.NextAddress = enumerable.Skip(currentPos).Take(4).Reverse().ConvertToString("");
                currentPos += 4;
                cacheEntry.RankingsNode = enumerable.Skip(currentPos).Take(4).Reverse().ConvertToString("");
                currentPos += 4;
                cacheEntry.ReuseCount = enumerable.Skip(currentPos).Take(4).Reverse().ConvertToString("");
                currentPos += 4;
                cacheEntry.RefetchCount = enumerable.Skip(currentPos).Take(4).Reverse().ConvertToString("");
                currentPos += 4;
                cacheEntry.CurrentState = enumerable.Skip(currentPos).Take(4).Reverse().ConvertToString("");
                currentPos += 4;
                cacheEntry.CreationDate = enumerable.Skip(currentPos).Take(8).Reverse().ConvertToString("");
                currentPos += 8;
                cacheEntry.KeyLength = enumerable.Skip(currentPos).Take(4).Reverse().ConvertToString("");
                currentPos += 4;
                cacheEntry.OptionalAddress = enumerable.Skip(currentPos).Take(4).Reverse().ConvertToString("");
                currentPos += 4;
                cacheEntry.Size = enumerable.Skip(currentPos).Take(16).Split(4).Select(o => o.Reverse().ConvertToString("")).ToList();
                currentPos += 16;
                cacheEntry.CacheAddresses = enumerable.Skip(currentPos).Take(16).Split(4).Select(o => o.Reverse().ConvertToString("")).ToList();
                currentPos += 16;
                cacheEntry.EntryFlags = enumerable.Skip(currentPos).Take(4).Reverse().ConvertToString("");
                currentPos += 4;
                cacheEntry.Padding = enumerable.Skip(currentPos).Take(16).Split(4).Select(o => o.Reverse().ConvertToString("")).ToList();
                currentPos += 16;
                cacheEntry.SelfHash = enumerable.Skip(currentPos).Take(4).Reverse().ConvertToString("");
                currentPos += 4;
                cacheEntry.KeyString = enumerable.Skip(currentPos).Take(160).ConvertHexToString(); // last work was here http://www.forensicswiki.org/wiki/Chrome_Disk_Cache_Format
                currentPos += 160;
                if (currentPos != 256)
                {
                    throw new Exception();
                }

                entries.Add(cacheEntry);
                enumerable = enumerable.Skip(256).ToList();
            }
            return entries;
        }

        #endregion UNDER CONSTRUCTION
    }
}