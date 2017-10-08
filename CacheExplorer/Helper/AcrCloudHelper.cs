using CacheExplorer.Extensions;
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

            var iTunesSuggestions = iTunesHelper.GetResults(musicInfo.Artists.Select(o => o.Name).Aggregate((i, j) => i + " " + j), musicInfo.Title, musicInfo.Album.Name);
            iTunesSuggestions = iTunesSuggestions.OrderBy(o => o.trackName.Similarity(musicInfo.Title)).ThenBy(o => o.collectionName.Similarity(musicInfo.Album.Name));
            var bestMatch = iTunesSuggestions.FirstOrDefault(o => String.IsNullOrEmpty(o.collectionArtistName) || !o.collectionArtistName.Equals("Various Artists", StringComparison.OrdinalIgnoreCase));
            if (bestMatch == null)
            {
                bestMatch = iTunesSuggestions.FirstOrDefault();
            }

            using (var file = File.Create(filePath))
            {
                file.Tag.Title = musicInfo.Title;
                file.Tag.Album = musicInfo.Album.Name;
                file.Tag.Performers = musicInfo.Artists.Select(o => o.Name).ToArray();
                file.Tag.Genres = musicInfo.Genres?.Select(o => o.Name).ToArray() ?? new string[] { };
                DateTime date1;
                if (DateTime.TryParse(musicInfo.ReleaseDate, out date1))
                {
                    file.Tag.Year = Convert.ToUInt32(date1.Year);
                }
                if (bestMatch != null)
                {
                    file.Tag.Title = bestMatch.trackName;
                    file.Tag.Album = bestMatch.collectionName;
                    file.Tag.Performers = new[] { bestMatch.artistName };
                    file.Tag.Disc = (uint)bestMatch.discNumber;
                    file.Tag.DiscCount = (uint)bestMatch.discCount;
                    file.Tag.Genres = new[] { bestMatch.primaryGenreName };
                    file.Tag.Track = (uint)bestMatch.trackNumber;
                    file.Tag.TrackCount = (uint)bestMatch.trackCount;
                    var albumArt = WebHelper.GetPicture(bestMatch.artworkUrl100);
                        if (albumArt != null)
                    {
                        file.Tag.Pictures = new IPicture[] { albumArt };
                    }

                    DateTime date2;
                    if (DateTime.TryParse(bestMatch.releaseDate, out date2))
                    {
                        file.Tag.Year = Convert.ToUInt32(date2.Year);
                    }
                }

                file.Save();
            }
        }
    }
}