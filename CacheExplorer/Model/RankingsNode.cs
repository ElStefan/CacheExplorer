namespace CacheExplorer.Model
{
    public class RankingsNode
    {
        public string CacheEntryCacheAddress { get; internal set; }
        public string DirtyFlag { get; internal set; }
        public string LastModified { get; internal set; }
        public string LastUsed { get; internal set; }
        public string NextRankingsAddress { get; internal set; }
        public string PreviousRankingsAddress { get; internal set; }
        public string SelfHash { get; internal set; }
    }
}