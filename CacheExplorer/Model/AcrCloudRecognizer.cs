using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CacheExplorer
{
    public static class AcrCloudExtrTool
    {
        public static void Initialize()
        {
            acr_init();
        }

        public static byte[] CreateFingerprintByFile(string filePath, int startTimeSeconds, int audioLenSeconds, bool isDB)
        {
            byte[] fpBuffer = null;
            if (!File.Exists(filePath))
            {
                return fpBuffer;
            }
            var tIsDB = (isDB) ? (byte)1 : (byte)0;
            var pFpBuffer = IntPtr.Zero;
            var fpBufferLen = create_fingerprint_by_file(filePath, startTimeSeconds, audioLenSeconds, tIsDB, ref pFpBuffer);
            if (fpBufferLen <= 0)
            {
                return fpBuffer;
            }

            fpBuffer = new byte[fpBufferLen];
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

    internal class AcrCloudRecognizer
    {
        private readonly string host = "ap-southeast-1.api.acrcloud.com";
        private readonly string accessKey = "";
        private readonly string accessSecret = "";
        private readonly int timeout = 5 * 1000; // ms

        public AcrCloudRecognizer(IDictionary<string, Object> config)
        {
            if (config.ContainsKey(nameof(host)))
            {
                this.host = (string)config[nameof(host)];
            }
            if (config.ContainsKey(nameof(accessKey)))
            {
                this.accessKey = (string)config[nameof(accessKey)];
            }
            if (config.ContainsKey(nameof(accessSecret)))
            {
                this.accessSecret = (string)config[nameof(accessSecret)];
            }
            if (config.ContainsKey(nameof(timeout)))
            {
                this.timeout = 1000 * (int)config[nameof(timeout)];
            }
            AcrCloudExtrTool.Initialize();
        }

        public String RecognizeByFile(string filePath, int startSeconds)
        {
            var fp = AcrCloudExtrTool.CreateFingerprintByFile(filePath, startSeconds, 12, false);
            if (fp == null)
            {
                return "";
            }
            return this.DoRecognize(fp);
        }

        private string PostHttp(string url, IDictionary<string, Object> postParams)
        {
            var result = "";

            var BOUNDARYSTR = "acrcloud***copyright***2015***" + DateTime.Now.Ticks.ToString("x");
            var BOUNDARY = "--" + BOUNDARYSTR + "\r\n";
            var ENDBOUNDARY = Encoding.ASCII.GetBytes("--" + BOUNDARYSTR + "--\r\n\r\n");

            var stringKeyHeader = BOUNDARY +
                           "Content-Disposition: form-data; name=\"{0}\"" +
                           "\r\n\r\n{1}\r\n";
            var filePartHeader = BOUNDARY +
                            "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                            "Content-Type: application/octet-stream\r\n\r\n";

            using (var memStream = new MemoryStream())
            {
                foreach (var item in postParams)
                {
                    if (item.Value is string)
                    {
                        var tmpStr = string.Format(stringKeyHeader, item.Key, item.Value);
                        var tmpBytes = Encoding.UTF8.GetBytes(tmpStr);
                        memStream.Write(tmpBytes, 0, tmpBytes.Length);
                    }
                    else if (item.Value is byte[])
                    {
                        var header = string.Format(filePartHeader, "sample", "sample");
                        var headerbytes = Encoding.UTF8.GetBytes(header);
                        memStream.Write(headerbytes, 0, headerbytes.Length);
                        var sample = (byte[])item.Value;
                        memStream.Write(sample, 0, sample.Length);
                        memStream.Write(Encoding.UTF8.GetBytes("\r\n"), 0, 2);
                    }
                }
                memStream.Write(ENDBOUNDARY, 0, ENDBOUNDARY.Length);

                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Timeout = this.timeout;
                    request.Method = "POST";
                    request.ContentType = "multipart/form-data; boundary=" + BOUNDARYSTR;

                    memStream.Position = 0;
                    var tempBuffer = new byte[memStream.Length];
                    memStream.Read(tempBuffer, 0, tempBuffer.Length);

                    var writer = request.GetRequestStream();
                    writer.Write(tempBuffer, 0, tempBuffer.Length);
                    writer.Flush();
                    writer.Close();
                    writer = null;

                    var response = (HttpWebResponse)request.GetResponse();
                    using (var myReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = myReader.ReadToEnd();
                    }
                }
                catch (WebException e)
                {
                    Console.WriteLine("timeout:\n" + e);
                }
                catch (Exception e)
                {
                    Console.WriteLine("other excption:" + e);
                }

                return result;
            }
        }

        private static string EncryptByHmacSha1(string input, string key)
        {
            using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(key)))
            {
                var stringBytes = Encoding.UTF8.GetBytes(input);
                var hashedValue = hmac.ComputeHash(stringBytes);
                return EncodeToBase64(hashedValue);
            }
        }

        private static string EncodeToBase64(byte[] input)
        {
            var res = Convert.ToBase64String(input, 0, input.Length);
            return res;
        }

        private string DoRecognize(byte[] queryData)
        {
            const string method = "POST";
            const string httpURL = "/v1/identify";
            const string dataType = "fingerprint";
            const string sigVersion = "1";
            var timestamp = ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).ToString();

            var reqURL = "http://" + host + httpURL;

            var sigStr = method + "\n" + httpURL + "\n" + accessKey + "\n" + dataType + "\n" + sigVersion + "\n" + timestamp;
            var signature = EncryptByHmacSha1(sigStr, this.accessSecret);

            var dict = new Dictionary<string, object>();
            dict.Add("access_key", this.accessKey);
            dict.Add("sample_bytes", queryData.Length.ToString());
            dict.Add("sample", queryData);
            dict.Add(nameof(timestamp), timestamp);
            dict.Add(nameof(signature), signature);
            dict.Add("data_type", dataType);
            dict.Add("signature_version", sigVersion);

            var res = PostHttp(reqURL, dict);

            return res;
        }
    }
}