using System;
using System.Collections.Generic;
using System.Linq;
using HabraMiner.Interfaces;
using MongoDB.Bson;

namespace HabraMiner
{
    public class Article
    {
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
    }
}