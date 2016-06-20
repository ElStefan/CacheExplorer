using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheExplorer.Model
{
    public class CacheData
    {
        public List<string> AllocationBitmap { get; set; }
        public int BlockSize { get; set; }
        public List<string> Counters { get; set; }
        public string Index { get; set; }
        public string LastPosition { get; set; }
        public string MagicNumber { get; set; }
        public int MaxNumberOfEntries { get; set; }
        public string NextFile { get; set; }
        public int NumberOfStoredEntries { get; set; }
        public string TrackUpdates { get; set; }
        public string User { get; set; }
        public string Version { get; set; }
        public string Unknown { get; set; }
        public List<CacheEntry> CacheEntries { get; set; }
        internal List<RankingsNode> RankingsNodes { get; set; }
    }
}