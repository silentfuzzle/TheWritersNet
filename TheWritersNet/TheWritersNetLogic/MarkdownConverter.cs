using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheWritersNetData.Models;
using TheWritersNetLogic.HyperlinkParsers;

namespace TheWritersNetLogic
{
    public static class MarkdownConverter
    {
        public static string MarkdownToHTML(string markdown, int websiteID)
        {
            markdown = markdown.Replace("<", "&lt;").Replace(">", "&gt;");
            markdown = NewlineToHTML(HashtagToHTML(FormattingToHTML(markdown)));
            return HyperlinkToHTML(markdown, websiteID);
        }

        public static List<SectionLinkModel> FindInternalLinks(string markdown, int sectionID)
        {
            SQLParser parser = new SQLParser(sectionID);
            parser.Execute(markdown);

            return parser.Links;
        }

        private static string HashtagToHTML(string markdown)
        {
            string hashTags = "\n######";
            int h = 6;

            while (hashTags.Length > 1)
            {
                int index = markdown.IndexOf(hashTags);
                if (index != -1)
                {
                    index++;
                    markdown = markdown.Remove(index, hashTags.Length - 1);
                    markdown = markdown.Insert(index, "<h" + h + ">");

                    index = markdown.IndexOf('\n', index);
                    if (index != -1)
                    {
                        while (markdown[index] == '\n' || markdown[index] == '\r')
                            index--;

                        markdown = markdown.Insert(index + 1, "</h" + h + ">");
                    }
                    else
                        markdown += "</h" + h + ">";
                }
                else
                {
                    hashTags = hashTags.Substring(0, hashTags.Length - 1);
                    h--;
                    index = 0;
                }
            }

            return markdown;
        }

        private static string HyperlinkToHTML(string markdown, int websiteID)
        {
            HTMLParser parser = new HTMLParser(websiteID);
            parser.Execute(markdown);

            return parser.Markdown;
        }

        private static string NewlineToHTML(string markdown)
        {
            markdown = markdown.Replace("\r\n", "<br>");
            markdown = markdown.Replace("\n", "<br>");

            return markdown;
        }

        private static string FormattingToHTML(string markdown)
        {
            markdown = SymbolPairToHTML("**", "strong", markdown);
            markdown = SymbolPairToHTML("*", "em", markdown);
            markdown = SymbolPairToHTML("_", "em", markdown);

            return markdown;
        }

        private static string SymbolPairToHTML(string symbol, string htmlTag, string markdown)
        {
            int index = 0;
            int openIndex = -1;
            while (index < markdown.Length)
            {
                index = markdown.IndexOf(symbol, index);
                if (index == -1)
                    index = markdown.Length;
                else if (openIndex == -1)
                    openIndex = index;
                else
                {
                    markdown = markdown.Remove(index, symbol.Length);
                    markdown = markdown.Insert(index, "</" + htmlTag + ">");
                    markdown = markdown.Remove(openIndex, symbol.Length);
                    markdown = markdown.Insert(openIndex, "<" + htmlTag + ">");
                    openIndex = -1;
                }

                index++;
            }

            return markdown;
        }
    }
}