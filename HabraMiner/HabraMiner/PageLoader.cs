using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using HabraMiner.Articles;
using HabraMiner.Exceptions;
using HabraMiner.PageDownloadTasks;
using NLog;

namespace HabraMiner
{
    public class PageLoader<TArticle> where TArticle : ArticleBase
    {
        private static readonly Logger Logger = LogManager.GetLogger("PageLoader");
        private readonly PageDownloadTask<TArticle>[] _tasks;
        private readonly Action<TArticle> _saveRoutine;

        public PageLoader(IEnumerable<PageDownloadTask<TArticle>> downloadTasks, Action<TArticle> saveRoutine)
        {
            _tasks = downloadTasks.ToArray();
            _saveRoutine = saveRoutine;
        }


        public void RunAllDellayedTasks(int delay, int simultaneousTasks)
        {
            Task.Run(() => RunTasksWithDelay(_tasks, delay, simultaneousTasks));
        }

        private void TaskPostProcessing(Task<string> task, object pPageDownloadTask)
        {
            string result = null;
            try
            {
                result = task.Result;
            }
            catch (Exception)
            {

            }
            var pageDownloadTask = (PageDownloadTask<TArticle>)pPageDownloadTask;
            var uri = pageDownloadTask.Uri;

            if (task.Exception != null || string.IsNullOrEmpty(result))
            {
                Logger.Error($"Unsuccessful downloading {uri.AbsolutePath}");
            }

            try
            {

                var article = pageDownloadTask.ParceArticle();
                _saveRoutine(article);
            }
            catch (NotFoundException ex)
            {
                Logger.Error($"Unsuccessful post downloading (404) {uri.AbsolutePath}");
            }
            catch (NotParcebleArticleException ex)
            {
                Logger.Error($"Unsuccessful parsing article at {uri.AbsolutePath}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Unsuccessful postprocessing {uri.AbsolutePath}");
            }
        }


        private void RunTasksWithDelay(IEnumerable<PageDownloadTask<TArticle>> tasks, int delay, int simultaneousTasks)
        {
            var workingTaskAwaiters = new LinkedList<TaskAwaiter>();
            foreach (var pageDownloadTask in tasks)
            {
                if (workingTaskAwaiters.Count > simultaneousTasks)
                {
                    while (workingTaskAwaiters.Any(a =>!a.IsCompleted))
                    {
                        Thread.SpinWait(100);
                        //Thread.Sleep(5);
                    }
                    workingTaskAwaiters.Where(a => a.IsCompleted).ToList().ForEach(a => workingTaskAwaiters.Remove(a));
                }
                try
                {
                    var awaiter = pageDownloadTask.DownloadTask.ContinueWith(TaskPostProcessing, pageDownloadTask,
                        TaskContinuationOptions.AttachedToParent |
                        TaskContinuationOptions.NotOnFaulted |
                        //TaskContinuationOptions.LongRunning|
                        TaskContinuationOptions.OnlyOnRanToCompletion).GetAwaiter();

                    workingTaskAwaiters.AddLast(awaiter);

                    pageDownloadTask.DownloadTask.Start();
                }
                catch (Exception ex)
                {
                    Logger.Error("плохо");
                }

                //Thread.Sleep(delay);
            }
        }
    }
}