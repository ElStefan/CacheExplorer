using System;

namespace CacheExplorer.Model
{
    public class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string AlbumArt { get; set; }
    }

    public class Rating
    {
        public bool Liked { get; set; }
        public bool Disliked { get; set; }
    }

    public class Time
    {
        public int Current { get; set; }
        public int Total { get; set; }
    }

    public class PlaybackModel
    {
        public string Id { get { return $"{Song.Title}_{Song.Album}_{Song.Artist}"; } }
        public DateTime StartDate { get; set; }
        public bool Playing { get; set; }
        public Song Song { get; set; }
        public Rating Rating { get; set; }
        public Time Time { get; set; }
        public string SongLyrics { get; set; }
        public string Shuffle { get; set; }
        public string Repeat { get; set; }
        public int Volume { get; set; }
    }
}
