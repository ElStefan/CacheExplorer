using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CacheExplorer.Model
{
  public static class AcrCloudExtrTool
  {
    public static void Initialize()
    {
      acr_init();
    }

    public static byte[] CreateFingerprintByFile(string filePath, int startTimeSeconds, int audioLenSeconds, bool isDB)
    {
      if (!File.Exists(filePath))
      {
        return null;
      }

      var tIsDB = isDB ? (byte)1 : (byte)0;
      var pFpBuffer = IntPtr.Zero;
      var fpBufferLen = create_fingerprint_by_file(filePath, startTimeSeconds, audioLenSeconds, tIsDB, ref pFpBuffer);
      if (fpBufferLen <= 0)
      {
        return null;
      }

      var fpBuffer = new byte[fpBufferLen];
      Marshal.Copy(pFpBuffer, fpBuffer, 0, fpBufferLen);
      acr_free(pFpBuffer);
      return fpBuffer;
    }

    [DllImport("libacrcloud_extr_tool.dll")]
    private static extern int create_fingerprint_by_file(string file_path, int start_time_seconds, int audio_len_seconds, byte is_db_fingerprint, ref IntPtr fps_buffer);

    [DllImport("libacrcloud_extr_tool.dll")]
    private static extern void acr_free(IntPtr buffer);

    [DllImport("libacrcloud_extr_tool.dll")]
    private static extern void acr_init();
  }
}
