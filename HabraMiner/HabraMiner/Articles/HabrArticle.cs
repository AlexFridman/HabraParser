using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace HabraMiner.Articles
{
    public class HabrArticle : ArticleBase
    {
        public ObjectId Id { get; set; }
        public string Author { get; set; }

        public string Name { get; set; }

        public string Company { get; set; }

        public ICollection<string> Habs { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }

        public ICollection<string> CodeComments { get; set; }
        public ICollection<string> Tags { get; set; }
        public int Number { get; set; }
        public int Rating { get; set; }
        public int Views { get; set; }
        public int Favourites { get; set; }

        public BsonDocumentWrapper ToBson()
        {
            return new BsonDocumentWrapper(this);
        }

        public override ArticleBase Parse(string data)
        {
            throw new NotImplementedException();
        }
    }
}