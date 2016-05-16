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
        private static string LocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private const string ChromePath = @"\Google\Chrome\User Data\Default\Cache";
        private static string TempDir = Environment.CurrentDirectory + @"\temp\";

        public static string ChromeCachePath
        {
            get
            {
                return LocalPath + ChromePath;
            }
        }
        public static IEnumerable<CacheFile> GetFiles(bool onlyMp3)
        {
            if(!CreateTempDir())
            {
                return null;
            }

            var files = Directory.GetFiles(ChromeCachePath, "f_*");
            var cacheFiles = files.Select(o => new CacheFile { FilePath = o, FileName = Path.GetFileName(o), Content = File.ReadAllBytes(o), CreateDate = File.GetCreationTime(o) }).ToList();
            if(!onlyMp3)
            {
                CleanupTempDir();
                return cacheFiles;
            }
            var mp3Files = cacheFiles.AsParallel().Where(o => IsMp3(o));
            CleanupTempDir();
            return mp3Files;
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

        public static bool IsMp3(CacheFile file)
        {
            var tempFileName = $"{Guid.NewGuid()}.mp3";
            var tempFile = $@"{TempDir}\{tempFileName}";

            if(!Directory.Exists(TempDir))
            {
                return false;
            }

            File.WriteAllBytes(tempFile, file.Content);
            ShellFile so = ShellFile.FromFilePath(tempFile);
            long nanoseconds;
            long.TryParse(so.Properties.System.Media.Duration.Value.ToString(),
            out nanoseconds);

            File.Delete(tempFile);
            if (nanoseconds > 0)
            {
                file.Length = nanoseconds;
                return true;
            }
            return false;
        }
    }
}
