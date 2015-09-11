using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace HabraMiner
{
    class HabrArticleParser : IHabrArticleParser
    {
        public Article Parse(string data)
        {
            Article article = new Article();

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(data);
            HtmlNode articleNode = html.DocumentNode.GetElementByClassName("post shortcuts_item");

            article.Name = articleNode.GetElementByClassName("post_title").InnerText;

            article.Date = HtmlHelpers.ParseHabrFormatDate(articleNode.GetElementByClassName("published").InnerText);
            
            article.Habs = new List<string>();
            var habs = articleNode.GetElementByClassName("hubs").ChildNodes.Where(n => n.Name == "a");
            foreach (var hab in habs)
            {
                article.Habs.Add(hab.InnerText);
            }

            article.Tags = new List<string>();
            var tags = articleNode.GetElementByClassName("tags").ChildNodes.First(n => n.Name == "li")
                .ChildNodes.First(n => n.Name == "a");

            article.Rating = int.Parse(articleNode.GetElementByClassName("voting-wjt__counter-score js-score").InnerText);
            article.Views = int.Parse(articleNode.GetElementByClassName("views-count_post").InnerText);
            article.Favourites = int.Parse(articleNode.GetElementByClassName("favorite-wjt__counter js-favs_count").InnerText);

            // TODO add author props
            article.Author = articleNode.GetElementByClassName("author-info__name").InnerText;
            
            // this is html, not text
            // TODO parse Text
            article.Text = articleNode.GetElementByClassName("content html_format").InnerHtml;

            return null;
        }

        
    }
}
