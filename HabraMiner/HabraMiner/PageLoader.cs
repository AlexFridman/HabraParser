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
        private readonly Queue<Task<Tuple<Uri, string>>> _taskQueue;
        private readonly Action<Tuple<Uri, string>> _taskPostProcessor;
        private readonly string _userAgent;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public PageLoader(IEnumerable<Uri> uris,Action<Tuple<Uri, string>> taskPostProcessor, string userAgent)
        {
            _taskQueue = FormTaskQueue(uris);
            _taskPostProcessor = taskPostProcessor;
            _userAgent = userAgent;
        }

        private Queue<Task<Tuple<Uri, string>>> FormTaskQueue(IEnumerable<Uri> uris)
        {
            var tasks = uris.Select(uri => new Task<Tuple<Uri, string>>(DownloadPage, uri));

            return new Queue<Task<Tuple<Uri, string>>>(tasks);
        }

        private Tuple<Uri, string> DownloadPage(object arg)
        {
            var uri = (Uri) arg;

            try
            {
                var client = new WebClient {Headers = {["User-Agent"] = _userAgent}};

                var page = client.DownloadString(uri);
                Logger.Info($"Downloaded {uri.AbsolutePath}");

                return Tuple.Create(uri, page);
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

        private void TaskPostProcessing(Task<Tuple<Uri, string>> task)
        {
            if (task.Exception != null)
            {
                return;
            }

            

            try
            {
                _taskPostProcessor(task.Result);
            }
            catch (Exception ex)
            {
                Logger.Error($"Unsuccessful post processing {task.Result.Item1.AbsolutePath}");
            }
        }
    }
}