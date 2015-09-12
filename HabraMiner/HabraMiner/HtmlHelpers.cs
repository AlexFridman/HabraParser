using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NLog.LayoutRenderers;

namespace HabraMiner
{
    public static class HtmlHelpers
    {
        public static ICollection<HtmlNode> GetElementsByClassName(this HtmlNode node, string attributeValue)
        {
            return (ICollection<HtmlNode>) node.SelectNodes($"//*[contains(@class,'{attributeValue}')]") ?? new List<HtmlNode>();
        }

        public static HtmlNode GetElementByClassName(this HtmlNode node, string attributeValue)
        {
            return node.GetElementsByClassName(attributeValue).FirstOrDefault();
        }

        public static ICollection<HtmlNode> GetElementsByTagName(this HtmlNode node, string tagName)
        {
            return (ICollection<HtmlNode>) node.SelectNodes($"//{tagName}") ?? new List<HtmlNode>();
        }

        public static HtmlNode GetElementByTagName(this HtmlNode node, string tagName)
        {
            return node.GetElementsByTagName(tagName).FirstOrDefault();
        }
        private static readonly Dictionary<string, int> MonthConverter =new Dictionary<string, int>
        {
            { "января",1},{ "февраля",2},{ "марта",3},{ "апреля",4},{ "мая",5},{ "июня",6},
            { "июля",7},{ "августа",8},{ "сентября",9},{ "октября",10},{ "ноября",11},{ "декабря",12}
        }; 
        public static DateTime ParseHabrFormatDate(string dateString)
        {
            var whiteSpaceSplit = dateString.Split(new []{ " "}, StringSplitOptions.RemoveEmptyEntries);
            var dayNumber = int.Parse(whiteSpaceSplit[0]);
            var month = MonthConverter[whiteSpaceSplit[1]];
            var year = int.Parse(whiteSpaceSplit[2]);
            var colonSplit = whiteSpaceSplit[4].Split(':');
            var hours = int.Parse(colonSplit[0]);
            var minutes = int.Parse(colonSplit[1]);
            return new DateTime(year,month,dayNumber,hours,minutes,0);
            ////return DateTime.ParseExact(dateString, "dd MMMM yyyy в hh:mm",CultureInfo.GetCultureInfo("ru-RU"));
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