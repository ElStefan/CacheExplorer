using System;
using System.IO;
using System.Text;

namespace CacheExplorer.Model
{
  public class DownloadFile
  {
    private byte[] _content;

    public byte[] Content
    {
      get
      {
        if (_content == null)
        {
          _content = TryReadFile(FilePath);
        }

        return _content;
      }

      set
      {
        _content = value;
      }
    }

    private byte[] TryReadFile(string filePath)
    {
      try
      {
        using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write | FileShare.Delete))
        using (var reader = new StreamReader(fileStream, Encoding.Default))
        {
          var content = reader.ReadToEnd();
          return reader.CurrentEncoding.GetBytes(content);
        }
      }
      catch (Exception)
      {
        return null;
      }
    }

    public DownloadFile(string filePath)
    {
      FilePath = filePath;
      FileName = Path.GetFileName(filePath);
      CreateDate = File.GetCreationTime(filePath);
    }

    public DateTime CreateDate { get; set; }
    public string FileName { get; set; }

    public string FilePath { get; set; }

    public int FileSize
    {
      get
      {
        return Content?.Length ?? 0;
      }
    }
  }
}
