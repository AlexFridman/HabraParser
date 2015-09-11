using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabraMiner.Interfaces;
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
            HtmlNode articleNode = html.DocumentNode.GetElementByAttributeValue("post shortcuts_item");

            article.Name = articleNode.GetElementByAttributeValue("post_title").InnerText;

            article.Date = HtmlHelpers.ParseHabrFormatDate(articleNode.GetElementByAttributeValue("published").InnerText);
            
            article.Habs = new List<string>();
            var habs = articleNode.GetElementByAttributeValue("hubs").ChildNodes.Where(n => n.Name == "a");
            foreach (var hab in habs)
            {
                article.Habs.Add(hab.InnerText);
            }

            article.Tags = new List<string>();
            var tags = articleNode.GetElementByAttributeValue("tags").ChildNodes.First(n => n.Name == "li")
                .ChildNodes.First(n => n.Name == "a");

            article.Rating = int.Parse(articleNode.GetElementByAttributeValue("voting-wjt__counter-score js-score").InnerText);
            article.Views = int.Parse(articleNode.GetElementByAttributeValue("views-count_post").InnerText);
            article.Favourites = int.Parse(articleNode.GetElementByAttributeValue("favorite-wjt__counter js-favs_count").InnerText);

            //TODO check other comments
            article.CodeComments = articleNode.GetElementsByAttributeValue("nginx").Select(n => n.InnerText).ToList();

            var codeNotes = articleNode.GetElementsByAttributeValue("nginx");
            foreach (var node in codeNotes)
            {
                node.Remove();
            }
            codeNotes = articleNode.GetElementsByAttributeValue("bash");
            foreach (var node in codeNotes)
            {
                node.Remove();
            }

            var imageNotes = articleNode.GetElementsByClassName("img");
            foreach (var image in imageNotes)
            {
                image.Remove();
            }

            // TODO add author props
            article.Author = articleNode.GetElementByAttributeValue("author-info__name").InnerText;
            
            // this is html, not text
            // TODO parse Text
            article.Text = articleNode.GetElementByAttributeValue("content html_format").InnerText;

            return article;
        }

        
    }
}
