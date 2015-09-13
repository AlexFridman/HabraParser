using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HabraMiner.Articles;
using NLog;

namespace HabraMiner.PageDownloadTasks
{
    public class PageDownloadTaskFactory
    {
        public const string DefaultUserAgent = "";
        private static readonly Logger Logger = LogManager.GetLogger("PageDownloadTaskFactory");
        public static PageDownloadTask<TArticle> CreateDownloadTask<TArticle>(Uri uri, Encoding encoding,
            string userAgent = DefaultUserAgent) where TArticle : ArticleBase, new()
        {
            if (IsHabrUri(uri))
            {
                return new PageDownloadTask<TArticle>
                {
                    Uri = uri,
                    DownloadTask = CreateDownloadPageContentTask(uri, encoding, userAgent)
                };
            }
            return null;
        }

        private static bool IsHabrUri(Uri uri)
        {
            return true;
        }

        private static Task<string> CreateDownloadPageContentTask(Uri uri, Encoding encoding, string userAgent)
        {
            return new Task<string>(() =>
            {
                var client = new WebClient
                {
                    Headers = {["User-Agent"] = userAgent},
                    Encoding = encoding
                };
                string result;
                try
                {
                    Logger.Info($"Downloading {uri.AbsolutePath}");
                    result=  client.DownloadString(uri);

                }
                catch (Exception ex)
                {
                    //Logger.Fatal($"404 {uri.AbsolutePath}");
                    return "";
                }
                return result;
            });
        }
    }
}