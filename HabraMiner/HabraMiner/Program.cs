using System;
using System.Linq;
using System.Text;
using System.Threading;
using HabraMiner.Articles;
using HabraMiner.PageDownloadTasks;
using HabraMiner.Storage;

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
                Enumerable.Range(25296, 200000)
                    .Select(
                        num =>
                            PageDownloadTaskFactory.CreateDownloadTask<HabrArticle>(
                                new Uri($"http://www.habrahabr.ru/post/{num}"), Encoding.UTF8, useragent));

            var loader = new PageLoader<HabrArticle>(tasks, article => saver.Save(article));
            loader.RunAllDellayedTasks(10, 19);
            Thread.CurrentThread.Join();
        }
    }
}
