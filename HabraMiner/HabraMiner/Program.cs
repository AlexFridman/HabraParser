using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using HabraMiner.Articles;
using HabraMiner.PageDownloadTasks;
using HabraMiner.Storage;

namespace HabraMiner
{
    class Program
    {
        class Configuration
        {
            public string DbServer { get; set; }
            public int  DbPort { get; set; }
            public string DbName{ get; set; }
            public string   CollectionName { get; set; }
            public int BatchSave { get; set; }
            public int Start { get; set; }

            public int Count { get; set; }
            public int TaskCount { get; set; }

            public static Configuration ReadConfiguration(string path)
            {
                var lines = File.ReadAllLines(path);
                return new Configuration
                {
                    DbServer = lines[0],
                    DbPort = int.Parse(lines[1]),
                    DbName = lines[2],
                    CollectionName = lines[3],
                    BatchSave = int.Parse(lines[4]),
                    Start = int.Parse(lines[5]),
                    Count = int.Parse(lines[6]),
                    TaskCount = int.Parse(lines[7]),
                };
            }

        }
        private static void Main(string[] args)
        {
            var conf = Configuration.ReadConfiguration("HabraMiner.cfg");
            var useragent =
                "";
           var saver = new MongoArticleSaver<HabrArticle>(conf.DbServer, conf.DbPort, conf.DbName, conf.CollectionName, conf.BatchSave);
            var tasks =
                Enumerable.Range(conf.Start, conf.Count)//30466
                    .Select(
                        num =>
                            PageDownloadTaskFactory.CreateDownloadTask<HabrArticle>(
                                new Uri($"http://www.habrahabr.ru/post/{num}"), Encoding.UTF8, useragent));

            var loader = new PageLoader<HabrArticle>(tasks, article => saver.Save(article));
            loader.RunAllDellayedTasks(1, conf.TaskCount);
            Thread.CurrentThread.Join();
        }
    }
}
