using HabraMiner.Articles;
using HabraMiner.PageDownloadTasks;

namespace HabraMiner.ArticleParsers
{
    public abstract class ArticleParserBase
    {
        public abstract ArticleBase Parse(PageDownloadTaskBase downloadTask);
    }
}