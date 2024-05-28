using System.Net;
using TagLib;

namespace CacheExplorer.Helper
{
  public static class WebHelper
  {
    public static IPicture GetPicture(string artworkUrl100)
    {
      if (string.IsNullOrEmpty(artworkUrl100))
      {
        return null;
      }

      using (var client = new WebClient())
      {
        var address = artworkUrl100.Replace("100x100bb.jpg", "500x500bb.jpg");
        var picture = client.DownloadData(address);
        return new Picture(new ByteVector(picture));
      }
    }
  }
}
