using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheExplorer.Model
{
    public class CacheEntry
    {
        public List<string> CacheAddresses { get; internal set; }
        public string CreationDate { get; internal set; }
        public string CurrentState { get; internal set; }
        public string EntryFlags { get; internal set; }
        public string Hash { get; internal set; }
        public string KeyLength { get; internal set; }
        public string KeyString { get; internal set; }
        public string NextAddress { get; internal set; }
        public string OptionalAddress { get; internal set; }
        public List<string> Padding { get; internal set; }
        public string RankingsNode { get; internal set; }
        public string RefetchCount { get; internal set; }
        public string ReuseCount { get; internal set; }
        public string SelfHash { get; internal set; }
        public List<string> Size { get; internal set; }
    }
}