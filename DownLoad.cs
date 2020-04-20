using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DownLoadHaoKanVideo.VideoEntity;
using HtmlAgilityPack;

namespace DownLoadHaoKanVideo
{
    public class DownLoad
    {
        public int _threadCound { get; set; }
        public int _byteArraySize { get; set; }
        public string _basePath { get; set; }

        public HttpClient Client { get; set; }
        /// <summary>
        /// 当前视频路径。
        /// </summary>
        public string _currentVideoPath;

        public int _DounCount;
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="threadCount">线程数</param>
        /// <param name="byteArraySize">缓存的字节 1G？</param>
        /// <param name="webProxy">代理</param>
        /// <param name="basePath">基础下载路径</param>

        public DownLoad(int threadCount = 8, int byteArraySize = 1048576, WebProxy webProxy = null,
            string basePath = null)
        {
            _threadCound = threadCount;
            _byteArraySize = byteArraySize;
            if (!string.IsNullOrEmpty(basePath))
            {
                _basePath = basePath;
            }

            var count = byteArraySize / 1024 / 1024;
            Console.WriteLine($"当前的线程数为{threadCount}，每秒缓存{count}MB");
            var httpClientHander = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.All,
                Proxy = webProxy
            };
            httpClientHander.UseProxy = httpClientHander.Proxy != null;
            var client = new HttpClient(httpClientHander);
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,zh-CN;q=0.8,zh;q=0.7");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36");
            Client = client;
        }

        public async Task AutomationGo(string url)
        {
            var source = await GetVideoInfo(url);
            GetFile(source);
        }

        public async Task<DataEntity> GetVideoInfo(string url)
        {
            var response = await Client.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var downloadurl = Regex.Match(html, "<video class=\"video\" src=(.*?)></video>").Groups[1].Value;
            var title = doc.DocumentNode.SelectSingleNode("//h2/text()").InnerText;
            var VideoInfo = doc.DocumentNode.SelectSingleNode("//span class=\"videoinfo-playnums float-left\"").InnerText;
            Console.WriteLine($"下载链接为{downloadurl},标题为{title},视频信息为{VideoInfo}");
            return new DataEntity
            {
                DownLoadUrl = GetMd5(downloadurl),
                Title = title,
                VideoInfo = VideoInfo
            };
        }
        /// <summary>
        /// 链接加密md5
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetMd5(string url)
        {
            using var md5 = MD5.Create();
            var output = md5.ComputeHash(Encoding.UTF8.GetBytes(url));
            var result = BitConverter.ToString(output).Replace("-", "").ToLower();
            return result;
        }

        private async Task GetFile(DataEntity data)
        {
            Console.WriteLine($"正在下载的{data.Title}");

            var hrm = new HttpRequestMessage(HttpMethod.Head, data.DownLoadUrl)
            {
                Version = new Version(2, 0)
            };
            using var response = await Client.SendAsync(hrm);
            //文件字节数
            var length = response.Content.Headers.ContentLength;
            data.FileSize = length.Value;
            CreateFile(data);
            if (length.HasValue)
            {
                MultithreadDownload(data.DownLoadUrl, length.Value, data);
            }
        }
        private void CreateFile(DataEntity data)
        {
            _currentVideoPath = Path.Combine(_basePath, $"{data.Title}.mp4");
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
            using var filestream=new FileStream(_currentVideoPath,FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.ReadWrite);
            var count = data.FileSize / 1024 / 1024;
            _DounCount = 0;
            Console.WriteLine($"创建文件：{_currentVideoPath},大小{count}MB");
            filestream.SetLength(data.FileSize);
        }





        #region 重点
        private void MultithreadDownload(string url, long fileSize, DataEntity info)
        {
            long fileLocation = 0;
        }


        #endregion
    }
}
