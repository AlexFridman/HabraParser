using System;

namespace HabraMiner.Articles
{
    public abstract class ArticleBase
    {
        public Uri Uri { get; set; }

        public abstract ArticleBase Parse(string data);
    }
}