using CacheExplorer.Extensions;
using CacheExplorer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using TagLib;

namespace CacheExplorer.Helper
{
    public static class TagLibHelper
    {
        public static void SetFileInfos(string filePath, Result match)
        {
            using (var file = File.Create(filePath))
            {
                file.Tag.Title = match.trackName;
                file.Tag.Album = match.collectionName;
                file.Tag.Performers = new[] { match.artistName };
                file.Tag.Disc = (uint)match.discNumber;
                file.Tag.DiscCount = (uint)match.discCount;
                file.Tag.Genres = new[] { match.primaryGenreName };
                file.Tag.Track = (uint)match.trackNumber;
                file.Tag.TrackCount = (uint)match.trackCount;
                file.Tag.Lyrics = match.lyrics;
                var albumArt = WebHelper.GetPicture(match.artworkUrl100);
                if (albumArt != null)
                {
                    file.Tag.Pictures = new[] { albumArt };
                }

                DateTime date2;
                if (DateTime.TryParse(match.releaseDate, out date2))
                {
                    file.Tag.Year = Convert.ToUInt32(date2.Year);
                }

                file.Save();
            }
        }

        private static bool FileHasTags(string filePath)
        {
            var tagLibFile = File.Create(filePath);
            return !tagLibFile.Tag.IsEmpty;
        }
    }
}
