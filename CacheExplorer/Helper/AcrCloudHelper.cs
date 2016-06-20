using CacheExplorer.Model;
using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TagLib;

namespace CacheExplorer.Helper
{
    public static class AcrCloudHelper
    {
        public static void RecognizeFile(string filePath)
        {
            var accessKey = ConfigurationManager.AppSettings.Get("AcrCloudAccessKey");
            var accessSecret = ConfigurationManager.AppSettings.Get("AcrCloudAccessSecret");
            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(accessSecret))
            {
                return;
            }

            var config = new Dictionary<string, object>();
            config.Add("host", "eu-west-1.api.acrcloud.com");
            config.Add("accessKey", accessKey);
            config.Add("accessSecret", accessSecret);
            config.Add("timeout", 10); // seconds

            var re = new AcrCloudRecognizer(config);
            var result = re.RecognizeByFile(filePath, 30);

            var audioInfos = JsonConvert.DeserializeObject<AcrResult>(result);
            if (audioInfos == null)
            {
                return;
            }
            if (audioInfos.Status.Msg.Equals("success", StringComparison.OrdinalIgnoreCase))
            {
                SetFileInfos(filePath, audioInfos);
            }
        }

        private static void SetFileInfos(string filePath, AcrResult audioInfos)
        {
            var musicInfo = audioInfos.Metadata.Music.OrderBy(o => o.ReleaseDate).FirstOrDefault();
            if (musicInfo == null)
            {
                return;
            }

            using (var file = File.Create(filePath))
            {
                file.Tag.Title = musicInfo.Title;
                file.Tag.Album = musicInfo.Album.Name;
                file.Tag.Performers = musicInfo.Artists.Select(o => o.Name).ToArray();
                file.Tag.Genres = musicInfo.Genres?.Select(o => o.Name).ToArray() ?? new string[] { };
                DateTime result;
                if (DateTime.TryParse(musicInfo.ReleaseDate, out result))
                {
                    file.Tag.Year = Convert.ToUInt32(result.Year);
                }

                file.Save();
            }
        }
    }
}