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
        private static readonly Dictionary<string, int> MonthConverter =new Dictionary<string, int>
        {
            { "января",1},{ "февраля",2},{ "марта",3},{ "апреля",4},{ "мая",5},{ "июня",6},
            { "июля",7},{ "августа",8},{ "сентября",9},{ "октября",10},{ "ноября",11},{ "декабря",12}
        }; 
        public static DateTime ParseHabrFormatDate(string dateString)
        {
            var digitsRegex = new Regex(@"\d{4}|\d{2}");
            var nums = digitsRegex.Matches(dateString);
            var monthRegex = new Regex(@" (\w+) ");
            var month = monthRegex.Match(dateString);
            var monthNumber = MonthConverter[month.Value.Trim()];
            var yearNumber = int.Parse(nums[1].Value);
            var dayNumber = int.Parse(nums[0].Value);
            var hours = int.Parse(nums[2].Value);
            var minutes = int.Parse(nums[3].Value);
            return new DateTime(yearNumber,monthNumber,dayNumber,hours,minutes,0);
            //return DateTime.ParseExact(dateString, "dd MMMM yyyy в hh:mm",CultureInfo.GetCultureInfo("ru-RU"));
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