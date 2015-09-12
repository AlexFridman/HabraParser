using System;
using System.Collections.Generic;

namespace HabraMiner.Articles
{
    public class HabrArticle : ArticleBase
    {
        public string Author { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public ICollection<string> Habs { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public ICollection<string> CodeComments { get; set; }
        public int Number { get; set; }
    }
}