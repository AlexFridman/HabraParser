using System;
using System.Collections.Generic;
using System.Linq;
using HabraMiner.Interfaces;
using MongoDB.Bson;

namespace HabraMiner
{
    public class Article : IMongoEntity
    {
        public string Author
        {
            get { return InternalDocument["Author"].AsString; }
            set { InternalDocument["Author"] = value; }
        }

        public string Name
        {
            get { return InternalDocument["Name"].AsString; }
            set { InternalDocument["Name"] = value; }
        }

        public string Company
        {
            get { return InternalDocument["Company"].AsString; }
            set { InternalDocument["Company"] = value; }
        }

        public ICollection<string> Habs { get; set; }

        public DateTime Date
        {
            get { return InternalDocument["Date"].ToUniversalTime(); }
            set { InternalDocument["Company"] = value.ToUniversalTime(); }
        }

        public string Text
        {
            get { return InternalDocument["Text"].AsString; }
            set { InternalDocument["Text"] = value; }
        }

        public ICollection<string> CodeComments
        {
            get { return InternalDocument["CodeComments"].AsBsonArray.Select(bv => bv.AsString).ToList(); }
            set { InternalDocument["Text"] = new BsonArray(value); }
        }

        public ICollection<string> Tags
        {
            get { return InternalDocument["Tags"].AsBsonArray.Select(bv => bv.AsString).ToList(); }
            set { InternalDocument["Tags"] = new BsonArray(value); }
        }

        public int Number
        {
            get { return InternalDocument["Number"].AsInt32; }
            set { InternalDocument["Number"] = value; }
        }

        public int Rating
        {
            get { return InternalDocument["Rating"].AsInt32; }
            set { InternalDocument["Rating"] = value; }
        }

        public int Views
        {
            get { return InternalDocument["Views"].AsInt32; }
            set { InternalDocument["Views"] = value; }
        }

        public int Favourites
        {
            get { return InternalDocument["Favourites"].AsInt32; }
            set { InternalDocument["Favourites"] = value; }
        }

        public BsonDocument InternalDocument { get; set; }
    }
}