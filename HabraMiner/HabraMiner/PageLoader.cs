using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace HabraMiner
{
    public class PageLoader
    {
        private readonly Queue<Task<PageDTO>> _taskQueue;
        private readonly Action<PageDTO> _taskPostProcessor;
        private readonly string _userAgent;
        private readonly Encoding _encoding;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public PageLoader(IEnumerable<Uri> uris, Action<PageDTO> taskPostProcessor, string userAgent)
            : this(uris, taskPostProcessor, userAgent, Encoding.UTF8)
        { }

        public PageLoader(IEnumerable<Uri> uris,Action<PageDTO> taskPostProcessor, string userAgent, Encoding encoding)
        {
            _encoding = encoding;
            _taskQueue = FormTaskQueue(uris);
            _taskPostProcessor = taskPostProcessor;
            _userAgent = userAgent;
        }

        private Queue<Task<PageDTO>> FormTaskQueue(IEnumerable<Uri> uris)
        {
            var tasks = uris.Select(uri => new Task<PageDTO>(DownloadPage, uri));

            return new Queue<Task<PageDTO>>(tasks);
        }

        private PageDTO DownloadPage(object arg)
        {
            var uri = (Uri) arg;

            try
            {
                var client = new WebClient
                {
                    Headers = {["User-Agent"] = _userAgent},
                    Encoding = _encoding
                };

                var page = client.DownloadString(uri);
                Logger.Info($"Downloaded {uri.AbsolutePath}");

                return new PageDTO(uri, page);
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

        private void TaskPostProcessing(Task<PageDTO> task)
        {
            if (task.Exception != null)
            {
                return;
            }

            var result = task.Result;

            try
            {
                _taskPostProcessor(task.Result);
            }
            catch (Exception ex)
            {
                Logger.Error($"Unsuccessful post processing {result.Uri.AbsolutePath}");
            }
        }
    }
}