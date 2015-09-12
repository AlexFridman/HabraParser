using System;
using System.Threading.Tasks;
using HabraMiner.Articles;

namespace HabraMiner.PageDownloadTasks
{
    public class PageDownloadTask<TArticle> where TArticle : ArticleBase
    {
        private static readonly TArticle Article = Activator.CreateInstance<TArticle>();
        public PageDownloadTask()
        {
        }

        public PageDownloadTask(Uri uri, Task<string> downloadTask)
        {
            Uri = uri;
            DownloadTask = downloadTask;
        }

        public Uri Uri { get; set; }
        public Task<string> DownloadTask { get; set; }

        public TArticle ParceArticle()
        {
            if (!DownloadTask.IsCompleted)
            {
                throw new Exception("Task not completed");
            }
            var parsed = Article.Parse(DownloadTask.Result);
            parsed.Uri = Uri;
            return (TArticle)parsed;
        }
    }
}