using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HabraMiner.PageDownloadTasks
{
    public class PageDownloadTaskFactory
    {
        public const string DefaultUserAgent = "";

        public static PageDownloadTaskBase CreateDownloadTask(Uri uri, Encoding encoding,
            string userAgent = DefaultUserAgent, int delay = 0)
        {
            if (IsHabrUri(uri))
            {
                return new HabrDownloadTask
                {
                    Uri = uri,
                    DownloadTask = CreateDownloadPageContentTask(uri, encoding, userAgent, delay)
                };
            }
        return new HabrDownloadTask();
        }

        private static bool IsHabrUri(Uri uri)
        {
            return true;
        }

        private static Task<string> CreateDownloadPageContentTask(Uri uri, Encoding encoding, string userAgent, int delay = 0)
        {
            return new Task<string>(() =>
            { 
                var client = new WebClient
                {
                    Headers = {["User-Agent"] = userAgent },
                    Encoding = encoding
                };

                return client.DownloadString(uri);
            });
        }
    }
}