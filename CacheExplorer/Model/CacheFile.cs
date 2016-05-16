using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheExplorer.Model
{
    public class CacheFile
    {
        public DateTime CreateDate { get; set; }
        public string FileName { get; set; }
        public int FileSize
        {
            get
            {
                return Content.Length;
            }
        }

        public string FilePath { get; set; }
        public byte[] Content { get; set; }
        public long Length { get; set; }
        public string MediaLength
        {
            get
            {
                return new TimeSpan(Length).ToString(@"hh\:mm\:ss");
            }
        }
    }
}
