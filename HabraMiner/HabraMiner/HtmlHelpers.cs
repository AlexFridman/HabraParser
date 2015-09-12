using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace HabraMiner
{
    public static class HtmlHelpers
    {
        public static ICollection<HtmlNode> GetElementsByClassName(this HtmlNode node, string attributeValue)
        {
            return node.SelectNodes($"//*[contains(@class,'{attributeValue}')]");
        }

        public static HtmlNode GetElementByClassName(this HtmlNode node, string attributeValue)
        {
            return node.GetElementsByClassName(attributeValue).FirstOrDefault();
        }

        public static ICollection<HtmlNode> GetElementsByTagName(this HtmlNode node, string tagName)
        {
            return node.SelectNodes($"//{tagName}");
        }

        public static HtmlNode GetElementByTagName(this HtmlNode node, string tagName)
        {
            return node.GetElementsByTagName(tagName).FirstOrDefault();
        }

        public static DateTime ParseHabrFormatDate(string dateString)
        {
            return DateTime.ParseExact(dateString, "dd MMMM yyyy в hh:mm", new CultureInfo("ru-RU"));
        }

        public static string ReplaceLinks(string text)
        {
            return Regexes.LinkRegex.Replace(text, "$4");
        }

        public static string RemoveTags(string text)
        {
            return Regexes.TagRegex.Replace(text, " ");
        }

        public static string ReplaceHtml(string text)
        {
            return
                text.Replace("&gt;", ">")
                    .Replace("&amp;", "<")
                    .Replace("&quot;", @"""")
                    .Replace("\n", " ")
                    .Replace("\r", "")
                    .Trim();
        }

        class Regexes
        {
            public static readonly Regex LinkRegex = new Regex(@"(<a\s[^>]*href="")(.*?)(""[^>]*>)(.*?)(</a>)", RegexOptions.Compiled);
            public static readonly Regex TagRegex = new Regex(@"<[^>]*>", RegexOptions.Compiled);
        }
    }
}