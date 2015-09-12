using System;
using System.Text;
using System.Threading;
using HabraMiner.PageDownloadTasks;

namespace HabraMiner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var tasks = new[]
            {
                PageDownloadTaskFactory.CreateDownloadTask(new Uri("http://www.google.com"), Encoding.ASCII, "", 1000),
                PageDownloadTaskFactory.CreateDownloadTask(new Uri("http://www.google.com"), Encoding.ASCII, "", 2000),
                PageDownloadTaskFactory.CreateDownloadTask(new Uri("http://www.google.com"), Encoding.ASCII, "", 3000)
            };

            var loader = new PageLoader(tasks.ToList(), dto => { });
            loader.RunAllDellayedTasks();

            Thread.CurrentThread.Join();
        }
    }
}