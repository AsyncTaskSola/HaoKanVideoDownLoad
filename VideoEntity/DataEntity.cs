using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownLoadHaoKanVideo.VideoEntity
{
   public class DataEntity
    {
        public string Id { get; set; }
        //原始网站Url
        public string Url { get; set; }
        public string Title { get; set; }
        public string DownLoadUrl { get; set; }
        public long FileSize { get; set; }
        public string VideoInfo { get; set; }
    }
}
