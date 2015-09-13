using System;

namespace HabraMiner.Articles
{
    public abstract class ArticleBase
    {
        public Uri Uri { get; set; }
        public int Number { get; set; }
        public abstract ArticleBase Parse(string data);

        public abstract int GetNumber();
    }
}