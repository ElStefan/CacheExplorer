using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheExplorer.Model
{
    public class Status
    {
        public string Msg { get; set; }
        public int Code { get; set; }
        public string Version { get; set; }
    }

    public class ExternalIds
    {
        public string Isrc { get; set; }
        public string Upc { get; set; }
    }

    public class Youtube
    {
        public string Vid { get; set; }
    }

    public class Lyricfind
    {
        public string Lfid { get; set; }
    }

    public class Album
    {
        public string Id { get; set; }
    }

    public class Artist
    {
        public string Id { get; set; }
    }

    public class Track
    {
        public string Id { get; set; }
    }

    public class Spotify
    {
        public Album Album { get; set; }
        public List<Artist> Artists { get; set; }
        public Track Track { get; set; }
    }

    public class Album2
    {
        public int Id { get; set; }
    }

    public class Artist2
    {
        public int Id { get; set; }
    }

    public class Track2
    {
        public int Id { get; set; }
    }

    public class Itunes
    {
        public Album2 Album { get; set; }
        public List<Artist2> Artists { get; set; }
        public Track2 Track { get; set; }
    }

    public class Album3
    {
        public int Id { get; set; }
    }

    public class Artist3
    {
        public int Id { get; set; }
    }

    public class Genre
    {
        public int Id { get; set; }
    }

    public class Track3
    {
        public int Id { get; set; }
    }

    public class Deezer
    {
        public Album3 Album { get; set; }
        public List<Artist3> Artists { get; set; }
        public List<Genre> Genres { get; set; }
        public Track3 Track { get; set; }
    }

    public class ExternalMetadata
    {
        public Youtube Youtube { get; set; }
        public Lyricfind Lyricfind { get; set; }
        public Spotify Spotify { get; set; }
        public Itunes Itunes { get; set; }
        public Deezer Deezer { get; set; }
    }

    public class Album4
    {
        public string Name { get; set; }
    }

    public class Genre2
    {
        public string Name { get; set; }
    }

    public class Artist4
    {
        public string Name { get; set; }
    }

    public class Music
    {
        [JsonProperty(PropertyName = "external_ids")]
        public ExternalIds ExternalIds { get; set; }

        [JsonProperty(PropertyName = "play_offset_ms")]
        public int PlayOffsetMs { get; set; }

        [JsonProperty(PropertyName = "external_metadata")]
        public ExternalMetadata ExternalMetadata { get; set; }

        public string Label { get; set; }

        [JsonProperty(PropertyName = "release_date")]
        public string ReleaseDate { get; set; }

        public string Title { get; set; }

        [JsonProperty(PropertyName = "duration_ms")]
        public string DurationMs { get; set; }

        public Album4 Album { get; set; }
        public string Acrid { get; set; }
        public List<Genre2> Genres { get; set; }
        public List<Artist4> Artists { get; set; }
    }

    public class Metadata
    {
        public List<Music> Music { get; set; }

        [JsonProperty(PropertyName = "timestamp_utc")]
        public string TimestampUtc { get; set; }
    }

    public class AcrResult
    {
        public Status Status { get; set; }

        [JsonProperty(PropertyName = "service_type")]
        public int ServiceType { get; set; }

        public Metadata Metadata { get; set; }

        [JsonProperty(PropertyName = "result_type")]
        public int ResultType { get; set; }
    }
}