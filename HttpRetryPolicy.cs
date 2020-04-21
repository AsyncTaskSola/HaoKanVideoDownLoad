using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DownLoadHaoKanVideo
{
   public  class HttpRetryPolicy
    {
        //参考 https://www.cnblogs.com/lonelyxmas/p/10236933.html
        public static async Task<bool> DownPolicyTask(Func<Task> func)
        {
            var retryPolicy = Policy.Handle<HttpRequestException>().RetryAsync(3, (ex, count) =>
            {
                Console.WriteLine($"错误为{ex}");
                Console.WriteLine($"重试次数为{count}");
            });
            try
            {
                await retryPolicy.ExecuteAsync(func);
                return true;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
                return false;
            }

        }
    }
}
