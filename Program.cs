using DownLoadHaoKanVideo.VideoEntity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DownLoadHaoKanVideo
{
    class Program
    {
        public static List<DataEntity> Downloadlist = new List<DataEntity>();
        static async Task Main(string[] args)
        {
            var download=new DownLoad(8,webProxy: new WebProxy("127.0.0.1",1080));
            //await download.AutomationGo("https://haokan.baidu.com/v?vid=13494221573256629561&tab=");
            //Console.ReadLine();
            if (!File.Exists("Video/list.txt"))
            {
                using var system = File.Create("Video/list.txt");
                return;
            }

            if (File.Exists("Videos/List.log"))
            {
                foreach (var json in File.ReadAllLines("Videos/List.log"))
                {
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        continue;
                    }

                    var info = JsonSerializer.Deserialize<DataEntity>(json);
                    Downloadlist.Add(info);
                }
            }
            var lins = File.ReadAllLines("Video/list.txt");
            foreach (var url in lins)
            {
               if(Downloadlist.Exists(o=>o.DownLoadUrl== url)) continue;
               try
               {
                    await download.AutomationGo(url);
               }
               catch (Exception e)
               {
                   Console.WriteLine(e);
               }
            }
            Console.ReadKey();
        }
    }
}
