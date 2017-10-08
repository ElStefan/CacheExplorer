using CacheExplorer.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CacheExplorer.Helper
{
    public static class iTunesHelper
    {
        public static IEnumerable<Result> GetResults(string interpret, string title, string album)
        {
            var query = $"{interpret} {title}";
            if (!String.IsNullOrEmpty(album))
            {
                query += (" " + album);
            }
            var urlEncodedQuery = HttpUtility.UrlEncode(query);
            var baseUrl = "https://itunes.apple.com/search?term=";
            using (var client = new HttpClient())
            {
                var result = client.GetAsync(baseUrl + urlEncodedQuery + "&media=music&country=AT").Result;
                var content = result.Content.ReadAsStringAsync().Result;
                var searchResult = JsonConvert.DeserializeObject<iTunesSearchResult>(content);
                searchResult.results = searchResult.results.Where(o => o.kind == "song").ToList();
                if (searchResult.resultCount == 0 && title.Contains("("))
                {
                    title = title.Substring(0, title.IndexOf("(", StringComparison.Ordinal));
                    return GetResults(interpret, title, album);
                }
                if (searchResult.resultCount == 0 && !String.IsNullOrEmpty(album))
                {
                    return GetResults(interpret, title, string.Empty);
                }
                return searchResult.results;
            }
        }
    }
}