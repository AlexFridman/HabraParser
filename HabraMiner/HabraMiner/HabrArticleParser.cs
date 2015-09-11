using System;
using System.Collections.Generic;
using System.Linq;
using HabraMiner.Interfaces;
using HtmlAgilityPack;

namespace HabraMiner
{
    internal class HabrArticleParser : IHabrArticleParser
    {
        public Article Parse(string data)
        {
            var html = ConstructHtmlDocument(data);

            var articleNode = html.DocumentNode.GetElementByClassName("post shortcuts_item");

            var article = new Article
            {
                Name = ExtractName(articleNode),
                Date = ExtractDate(articleNode),
                Habs = ExtractHabs(articleNode),
                Rating = ExtractRating(articleNode),
                Views = ExtractView(articleNode),
                Favourites = ExtractFavourites(articleNode)
            };


            // TODO parse tags
            article.Tags = new List<string>();
            var tags = articleNode.GetElementByClassName("tags").ChildNodes.First(n => n.Name == "li")
                .ChildNodes.First(n => n.Name == "a");

            // TODO add author props
            article.Author = articleNode.GetElementByClassName("author-info__name").InnerText;

            // this is html, not text
            // TODO parse Text
            article.Text = articleNode.GetElementByClassName("content html_format").InnerHtml;

            return null;
        }

        private static int ExtractFavourites(HtmlNode articleNode)
        {
            return int.Parse(articleNode.GetElementByClassName("favorite-wjt__counter js-favs_count").InnerText);
        }

        private static int ExtractView(HtmlNode articleNode)
        {
            return int.Parse(articleNode.GetElementByClassName("views-count_post").InnerText);
        }

        private static int ExtractRating(HtmlNode articleNode)
        {
            return int.Parse(articleNode.GetElementByClassName("voting-wjt__counter-score js-score").InnerText);
        }

        private static string ExtractName(HtmlNode articleNode)
        {
            return articleNode.GetElementByClassName("post_title").InnerText;
        }

        private static DateTime ExtractDate(HtmlNode articleNode)
        {
            return HtmlHelpers.ParseHabrFormatDate(articleNode.GetElementByClassName("published").InnerText);
        }

        private static ICollection<string> ExtractHabs(HtmlNode articleNode)
        {
            var habsNodes = articleNode.GetElementByClassName("hubs").ChildNodes.Where(n => n.Name == "a");
            return habsNodes.Select(hab => hab.InnerText).ToList();
        }

        private static HtmlDocument ConstructHtmlDocument(string data)
        {
            var html = new HtmlDocument();
            html.LoadHtml(data);
            return html;
        }
    }
}