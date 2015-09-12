using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HabraMiner.Interfaces;
using HtmlAgilityPack;

namespace HabraMiner
{
    public interface IArticleSaver<in TArticle>
    {
        public Article Parse(string data)
        {
            var html = LoadHtmlDocument(data);

            var article = new Article();

            var articleNode = GetArticleNode(html);
            article.Name = ExtractName(articleNode);
            article.Date = ExtractDate(articleNode);
            article.Hubs = ExtractHabs(articleNode);
            article.Tags = ExtractTags(articleNode);
            article.Rating = ExtractRating(articleNode);
            article.Views = ExtractViews(articleNode);
            article.Favourites = ExtractFavourites(articleNode);
            article.CodeComments = ExtractCodeComments(articleNode);
            article.Author = ExtractAuthor(articleNode);

            DeleteImageNodes(articleNode);

            DeleteCodeNodes(articleNode);
            article.Text = articleNode.GetElementByClassName("content html_format").InnerText;
            
            article.Comments = ExtractComments(articleNode);

            return article;
        }

        private static void DeleteCodeNodes(HtmlNode articleNode)
        {
            var codeNotes = articleNode.GetElementsByClassName("nginx");
            foreach (var node in codeNotes)
            {
                node.Remove();
            }
            codeNotes = articleNode.GetElementsByClassName("bash");
            foreach (var node in codeNotes)
            {
                node.Remove();
            }
        }

        private static void DeleteImageNodes(HtmlNode articleNode)
        {
            var imageNotes = articleNode.GetElementsByTagName("img");
            foreach (var image in imageNotes)
            {
                image.Remove();
            }
        }

        private static string ExtractAuthor(HtmlNode articleNode)
        {
            return HtmlHelpers.ReplaceHtml(articleNode.GetElementByClassName("author-info__name").InnerText);
        }

        private static List<string> ExtractCodeComments(HtmlNode articleNode)
        {
            return articleNode.GetElementsByClassName("nginx").Select(n => n.InnerText).ToList();
        }

        private static List<string> ExtractComments(HtmlNode articleNode)
        {
            var nodes = articleNode.GetElementByClassName("comments")
                .GetElementsByClassName("message html_format")
                .SelectMany(n => n.ChildNodes).ToArray();

            foreach (var node in nodes.Where(node => node.Name != "#text"))
            {
                node.Remove();
            }
            return nodes.Select(n => HtmlHelpers.ReplaceHtml(n.InnerText))
                .Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        }

        private static int ExtractFavourites(HtmlNode articleNode)
        {
            return int.Parse(articleNode.GetElementByClassName("favorite-wjt__counter js-favs_count").InnerText);
        }

        private static int ExtractViews(HtmlNode articleNode)
        {
            return int.Parse(articleNode.GetElementByClassName("views-count_post").InnerText);
        }

        private static int ExtractRating(HtmlNode articleNode)
        {
            return int.Parse(articleNode.GetElementByClassName("voting-wjt__counter-score js-score").InnerText);
        }

        private static ICollection<string> ExtractTags(HtmlNode articleNode)
        {
            return
                HtmlHelpers.ReplaceHtml(articleNode.GetElementByClassName("tags")
                    .InnerText).Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private static ICollection<string> ExtractHabs(HtmlNode articleNode)
        {

            return articleNode.GetElementByClassName("hubs")
                    .ChildNodes.Where(n => n.Name == "a")
                    .Select(n => n.InnerText).ToList();
        }

        private static DateTime ExtractDate(HtmlNode articleNode)
        {
            return HtmlHelpers.ParseHabrFormatDate(articleNode.GetElementByClassName("published").InnerText);
        }

        private static string ExtractName(HtmlNode articleNode)
        {
            return articleNode.GetElementByClassName("post_title").InnerText;
        }

        private static HtmlNode GetArticleNode(HtmlDocument html)
        {
            return html.DocumentNode.GetElementByClassName("post shortcuts_item");
        }

        private static HtmlDocument LoadHtmlDocument(string data)
        {
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(data);
            return html;
        }
    }
}
