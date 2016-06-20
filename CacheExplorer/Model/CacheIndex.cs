using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheExplorer.Model
{
    public class CacheIndex
    {
        public string ActualInFlightOperation { get; set; }
        public List<string> CacheAddresses { get; set; }
        public string CacheFilledFlag { get; set; }
        public string CreationTime { get; set; }
        public string DirtyFlag { get; set; }
        public int Entries { get; set; }
        public List<string> HeadsCacheAddress { get; set; }
        public string InFlightOperationList { get; set; }
        public string LastFile { get; set; }
        public string MagicNumber { get; set; }
        public string Pad1 { get; set; }
        public string Pad2 { get; set; }
        public string PaddedContent { get; set; }
        public string PreviousCrash { get; set; }
        public List<string> Sizes { get; set; }
        public string Storage { get; set; }
        public int TableSize { get; set; }
        public List<string> TailsCacheAddress { get; set; }
        public string TestId { get; set; }
        public int TotalStoreSize { get; set; }
        public string TransActionCacheAddress { get; set; }
        public string Version { get; set; }
        public string Unknown { get; set; }
    }
}