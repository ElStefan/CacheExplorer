using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace CacheExplorer
{
  public class CustomResourceRequestHandler : CefSharp.Handler.ResourceRequestHandler
  {
    public const string OutputFolder = ".\\output";
    private const string RecentlyDownloadedCachePath = ".\\RecentlyDownloaded.txt";
    private static readonly HashSet<string> _recentlyDownloaded = new HashSet<string>();
    private readonly MemoryStream memoryStream = new MemoryStream();

    public CustomResourceRequestHandler()
    {
      if (File.Exists(RecentlyDownloadedCachePath))
      {
        _recentlyDownloaded.UnionWith(File.ReadAllLines(RecentlyDownloadedCachePath));
      }
    }

    protected override IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
    {
      return new CefSharp.ResponseFilter.StreamResponseFilter(memoryStream);
    }

    protected override void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
    {
      //You can now get the data from the stream
      var bytes = memoryStream.ToArray();
      // write base64 string to file
      if (response.MimeType != "application/vnd.yt-ump")
      {
        return;
      }

      try
      {
        // Decode the UMP parts
        var umpDecoder = new UmpDecoder();
        bytes = bytes.SkipWhile(x => x != 20).ToArray();
        if (bytes.Length == 0)
        {
          return;
        }

        var videoId = umpDecoder.Decode(bytes);
        if (!_recentlyDownloaded.Add(videoId))
        {
          return;
        }

        _ = Task.Run(async () =>
        {
          var ytdl = new YoutubeDL();
          // set the path of yt-dlp and FFmpeg if they're not in PATH or current directory
          ytdl.YoutubeDLPath = ".\\ytdl\\yt-dlp.exe";
          ytdl.FFmpegPath = ".\\ytdl\\ffmpeg.exe";
          if (!File.Exists(ytdl.FFmpegPath))
          {
            ytdl.FFmpegPath = ".\\ffmpeg.exe";
          }

          var options = OptionSet.Default;
          options.EmbedMetadata = true;
          options.AudioFormat = AudioConversionFormat.Mp3;
          options.AudioQuality = 0;
          options.Part = true;
          ytdl.OutputFolder = Path.Combine(OutputFolder, "temp");

          if (!Directory.Exists(OutputFolder))
          {
            Directory.CreateDirectory(OutputFolder);
            Directory.CreateDirectory("temp");
          }
          // download a video
          var res = await ytdl.RunAudioDownload($"https://www.youtube.com/watch?v={videoId}", overrideOptions: options);
          if (!res.Success)
          {
            _recentlyDownloaded.Remove(videoId);
            return;
          }

          // move file to one folder level higher
          File.Move(res.Data, Path.Combine(OutputFolder, Path.GetFileName(res.Data)));
          File.AppendAllLines(RecentlyDownloadedCachePath, new[] { videoId });

        });
      }
      catch (Exception)
      {
        // ignored
      }
    }
  }

  public class CustomRequestHandler : CefSharp.Handler.RequestHandler
  {
    protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
    {
      //Only intercept specific Url's
      if (request.Url.Contains("videoplayback"))
      {
        return new CustomResourceRequestHandler();
      }

      //Default behaviour, url will be loaded normally.
      return null;
    }
  }

  public class UmpPart
  {
    public long Type { get; set; }
    public long Size { get; set; }
    public byte[] Data { get; set; }
  }
  public class UmpDecoder
  {
    public string Decode(byte[] response)
    {
      var position = 0;
      var type = ReadUmpVarInt(response, ref position);
      var size = ReadUmpVarInt(response, ref position);

      if (size <= response.Length - position)
      {
        var data = new byte[size];
        Array.Copy(response, position, data, 0, size);

        if (type == 20)
        {
          return ProcessMediaHeader(data);
        }
      }

      return null;
    }

    int ReadUmpVarInt(byte[] data, ref int offset)
    {
      var firstByte = data[offset];
      var size = GetUmpVarIntSize(firstByte);
      int value;

      if (size == 1)
      {
        // Use the lower 7 bits
        value = firstByte & 0b01111111;
      }
      else if (size == 2)
      {
        // Use the lower 6 bits of the first byte and all 8 bits of the second byte
        value = ((firstByte & 0b00111111) << 6) | data[offset + 1];
      }
      else if (size == 3)
      {
        // Use the lower 5 bits of the first byte and all 8 bits of the second and third bytes
        // E.g. 202, 73 and 9 are 11001010, 01001001 and 00001001 in binary
        // The value is 00001001 01001001 11001010 = 15158770
        value = ((firstByte & 0b00011111) << 16) | (data[offset + 1] << 8) | (data[offset + 2]);
      }
      else if (size == 4)
      {
        value = ((firstByte & 0b00001111) << 24) | (data[offset + 1] << 16) | (data[offset + 2] << 8) | data[offset + 3];
      }
      else if (size == 5)
      {
        // The first byte's lower bits are ignored in 5-byte encoding
        value = data[offset + 1] | (data[offset + 2] << 8) | (data[offset + 3] << 16) | (data[offset + 4] << 24);
      }
      else
      {
        throw new InvalidDataException("Invalid UMP varint encoding");
      }

      offset += size;
      return value;
    }
    private int GetUmpVarIntSize(byte b)
    {
      if ((b & 0b10000000) == 0)
      {
        return 1; // 0xxx xxxx - 1 byte encoding
      }

      if ((b & 0b11000000) == 0b10000000)
      {
        return 2; // 100x xxxx - 2 byte encoding
      }

      if ((b & 0b11100000) == 0b11000000)
      {
        return 3; // 1100 xxxx - 3 byte encoding
      }

      if ((b & 0b11110000) == 0b11100000)
      {
        return 4; // 1110 0xxx - 4 byte encoding
      }

      if ((b & 0b11111000) == 0b11110000)
      {
        return 5; // 1111 0xxx - 5 byte encoding
      }

      throw new InvalidDataException("Invalid UMP varint encoding");
    }

    private string ProcessMediaHeader(byte[] data)
    {
      //Console.WriteLine("Processing Media Header...");
      var pos = 0;
      while (pos < data.Length)
      {
        try
        {
          var fieldInfo = (int)ReadVarInt(data, ref pos);
          var fieldNumber = fieldInfo >> 3;

          //Console.WriteLine($"Field Info: {fieldInfo}, Field Number: {fieldNumber}, Wire Type: {wireType}");

          switch (fieldNumber)
          {
            case 1:
              ReadVarInt(data, ref pos);
              break;
            case 2:
              return ReadString(data, ref pos);

            default:
              return string.Empty;
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Error processing field at position {pos}: {ex.Message}");
        }
      }

      return string.Empty;

    }
    private long ReadVarInt(byte[] buf, ref int pos)
    {
      long result = 0;
      var shift = 0;

      while (true)
      {
        if (pos >= buf.Length)
          throw new IndexOutOfRangeException("Buffer overrun while reading varint.");

        var b = buf[pos++];
        result |= (long)(b & 0x7F) << shift;

        if ((b & 0x80) == 0)
          break;

        shift += 7;

        if (shift >= 64)
          throw new InvalidDataException("Varint too long.");
      }

      return result;
    }

    private string ReadString(byte[] buf, ref int pos)
    {
      var length = (int)ReadVarInt(buf, ref pos);
      if (pos + length > buf.Length)
        throw new IndexOutOfRangeException("Buffer overrun while reading string.");

      var result = Encoding.UTF8.GetString(buf, pos, length);
      pos += length;
      return result;
    }
  }

}
