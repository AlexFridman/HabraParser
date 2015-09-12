using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabraMiner.Articles;

namespace HabraMiner
{
    public interface IArticleSaver<TArticle>
    {
        void Save(TArticle article);
    }
}
