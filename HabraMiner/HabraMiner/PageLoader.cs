using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace HabraMiner
{
    public class PageLoader
    {
        private readonly Queue<Task<string>> _taskQueue;
        private readonly Action<string> _taskPostProcessor;
        private readonly string _userAgent;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public PageLoader(IEnumerable<Uri> uris,Action<string> taskPostProcessor, string userAgent)
        {
            _taskQueue = FormTaskQueue(uris);
            _taskPostProcessor = taskPostProcessor;
            _userAgent = userAgent;
        }

        private Queue<Task<string>> FormTaskQueue(IEnumerable<Uri> uris)
        {
            var tasks = uris.Select(uri => new Task<string>(DownloadPage, uri));

            return new Queue<Task<string>>(tasks);
        }

        private string DownloadPage(object arg)
        {
            var uri = (Uri) arg;

            try
            {
                var client = new WebClient {Headers = {["User-Agent"] = _userAgent}};

                var page = client.DownloadString(uri);
                Logger.Info($"Downloaded {uri.AbsolutePath}");

                return page;
            }
            catch (Exception ex)
            {
                Logger.Error($"Not downloaded {uri.AbsolutePath}");

                throw;
            }
        }

        public void RunAllWithDelay(int delay)
        {
            foreach (var task in _taskQueue)
            {
                task.ContinueWith(TaskPostProcessing);
                task.Start();

                Thread.Sleep(delay);
            }
        }

        private void TaskPostProcessing(Task<string> task)
        {
            if (task.Exception != null)
            {
                return;
            }

            var result = task.Result;

            _taskPostProcessor(result);
        }
    }
}