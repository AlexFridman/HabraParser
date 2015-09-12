﻿using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HabraMiner.Articles;
using NLog;
using NLog.Fluent;

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

        private static Task<string> CreateDownloadPageContentTask(Uri uri, Encoding encoding, string userAgent,
            int delay = 0)
        {
            return new Task<string>(() =>
            {
                var client = new WebClient
                {
                    Headers = {["User-Agent"] = userAgent},
                    Encoding = encoding
                };
                try
                {
                    Logger.Info($"Downloading {uri.AbsolutePath}");
                    return client.DownloadString(uri);
                }
                catch (Exception ex)
                {
                    return "";
                }
            });
        }
    }
}