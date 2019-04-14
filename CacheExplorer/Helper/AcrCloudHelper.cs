using CacheExplorer.Model;
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
        public static Result RecognizeFile(string filePath)
        {
            var accessKey = ConfigurationManager.AppSettings.Get("AcrCloudAccessKey");
            var accessSecret = ConfigurationManager.AppSettings.Get("AcrCloudAccessSecret");
            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(accessSecret))
            {
                return null;
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
                return  null;
            }
            if (!audioInfos.Status.Msg.Equals("success", StringComparison.OrdinalIgnoreCase))
            {
                audioInfos = null;
            }
            var musicInfo = audioInfos?.Metadata.Music.OrderBy(o => o.ReleaseDate).FirstOrDefault();
            if(musicInfo == null)
            {
                return null;
            }
            var artists = musicInfo?.Artists.Select(o => o.Name).ToList();
            var title = musicInfo?.Title;
            var album = musicInfo?.Album.Name;
            var genres = musicInfo?.Genres?.Select(o => o.Name).ToList();
            var releaseDate = musicInfo?.ReleaseDate;

            return new Result { artistName = artists.FirstOrDefault(), trackName = title, collectionName = album, primaryGenreName = genres.FirstOrDefault(), releaseDate = releaseDate };
        }

        
    }
}