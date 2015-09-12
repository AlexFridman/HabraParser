using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HabraMiner.Articles;
using HabraMiner.Exceptions;
using HabraMiner.PageDownloadTasks;
using HabraMiner.Storage;
using NLog;

namespace HabraMiner
{
    class Program
    {
        private static void Main(string[] args)
        {
            var useragent =
                "";
           var saver = new MongoArticleSaver<HabrArticle>("localhost", 27017, "test_database", "habr");
            var tasks =
                Enumerable.Range(1, 200000)
                    .Select(
                        num =>
                            PageDownloadTaskFactory.CreateDownloadTask<HabrArticle>(
                                new Uri($"http://www.habrahabr.ru/post/{num}"), Encoding.UTF8, useragent));

            var loader = new PageLoader<HabrArticle>(tasks, article => saver.Save(article));
            loader.RunAllDellayedTasks(500);
            Thread.CurrentThread.Join();
        }
    }
}
