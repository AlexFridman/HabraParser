using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HabraMiner.Articles;
using HabraMiner.PageDownloadTasks;
using NLog;

namespace HabraMiner
{
    public class PageLoader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly PageDownloadTask<HabrArticle>[] _tasks;

        public PageLoader(IEnumerable<PageDownloadTask<HabrArticle>> downloadTasks)
        {
            _tasks = downloadTasks.ToArray();
        }


        public void RunAllDellayedTasks(int delay)
        {
            Task.Run(() => new TaskRunner().RunTasksWithDelay(_tasks, delay));
            var timeout = TimeSpan.FromMinutes(1);
            var tasks = _tasks.Select(t => t.DownloadTask).ToArray();
            var finishedTasksCount = 0;
            while (finishedTasksCount < tasks.Length)
            {
                Task.WaitAny(tasks, timeout);

                var completedTasks = GetCompletedTasks();
                var faultedTasks = GetFaultedTasks();


                finishedTasksCount += (completedTasks.Length + faultedTasks.Length);
            }
        }

        private PageDownloadTask<HabrArticle>[] GetCompletedTasks()
        {
            var completedTasks = _tasks.Where(t => t.DownloadTask.IsCompleted);

            return completedTasks.ToArray();
        }

        private PageDownloadTask<HabrArticle>[] GetFaultedTasks()
        {
            var faultedTasks = _tasks.Where(t => t.DownloadTask.IsFaulted);

            return faultedTasks.ToArray();
        }

        private static void TaskPostProcessing(Task<string> task, object pPageDownloadTask)
        {
            var pageDownloadTask = (PageDownloadTask<HabrArticle>) pPageDownloadTask;
            var uri = pageDownloadTask.Uri;

            if (task.Exception != null)
            {
                // TODO: Log
            }

            try
            {
                var article = pageDownloadTask.ParceArticle();
                // TODO: save
            }
            catch (Exception ex)
            {
                Logger.Error($"Unsuccessful post processing {uri.AbsolutePath}");
            }
        }

        private class TaskRunner
        {
            public void RunTasksWithDelay(PageDownloadTask<HabrArticle>[] tasks, int delay)
            {
                foreach (var pageDownloadTask in tasks)
                {
                    pageDownloadTask.DownloadTask.ContinueWith(TaskPostProcessing, pageDownloadTask);
                    pageDownloadTask.DownloadTask.Start();

                    Thread.Sleep(delay);
                }
            }
        }
    }
}