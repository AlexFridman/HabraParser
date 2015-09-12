﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HabraMiner.Articles;
using HabraMiner.Exceptions;
using HabraMiner.PageDownloadTasks;
using NLog;

namespace HabraMiner
{
    public class PageLoader<TArticle> where TArticle:ArticleBase
    {
        private static readonly Logger Logger = LogManager.GetLogger("PageLoader");
        private readonly PageDownloadTask<TArticle>[] _tasks;
        private readonly Action<TArticle> _saveRoutine;

        public PageLoader(IEnumerable<PageDownloadTask<TArticle>> downloadTasks, Action<TArticle> saveRoutine)
        {
            _tasks = downloadTasks.ToArray();
            _saveRoutine = saveRoutine;
        }


        public void RunAllDellayedTasks(int delay)
        {
            Task.Run(() => RunTasksWithDelay(_tasks, delay));
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

        private PageDownloadTask<TArticle>[] GetCompletedTasks()
        {
            var completedTasks = _tasks.Where(t => t.DownloadTask.IsCompleted);

            return completedTasks.ToArray();
        }

        private PageDownloadTask<TArticle>[] GetFaultedTasks()
        {
            var faultedTasks = _tasks.Where(t => t.DownloadTask.IsFaulted);

            return faultedTasks.ToArray();
        }

        private void TaskPostProcessing(Task<string> task, object pPageDownloadTask)
        {
            var pageDownloadTask = (PageDownloadTask<TArticle>) pPageDownloadTask;
            var uri = pageDownloadTask.Uri;

            if (task.Exception != null || string.IsNullOrEmpty(task.Result))
            {
                Logger.Error($"Unsuccessful downloading {uri.AbsolutePath}");
            }

            try
            {

                var article = pageDownloadTask.ParceArticle();
                _saveRoutine((TArticle) article);
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


        private void RunTasksWithDelay(IEnumerable<PageDownloadTask<TArticle>> tasks, int delay)
        {
            foreach (var pageDownloadTask in tasks)
            {
                try
                {
                    pageDownloadTask.DownloadTask.ContinueWith(TaskPostProcessing, pageDownloadTask);
                    pageDownloadTask.DownloadTask.Start();
                }
                catch (Exception ex)
                {
                    Logger.Error("пиздец");
                }
                
                Thread.Sleep(delay);
            }
        }
    }
}