using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheExplorer.Model
{
    public class CacheFile
    {
        public byte[] Content { get; set; }
        public DateTime CreateDate { get; set; }
        public string FileName { get; set; }

        public string FilePath { get; set; }
        public int FileSize
        {
            get
            {
                return Content.Length;
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
    }
}
