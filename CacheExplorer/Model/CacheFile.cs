using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheExplorer.Model
{
    public class CacheFile
    {
        private byte[] _content;

        public byte[] Content
        {
            get
            {
                if (_content == null)
                {
                    _content = File.ReadAllBytes(FilePath);
                }

                return _content;
            }

            set
            {
                _content = value;
            }
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

        public long Length { get; set; }

        public string MediaLength
        {
            get
            {
                return new TimeSpan(Length).ToString(@"hh\:mm\:ss");
            }
        }

        public string HexData { get; set; }
    }
}