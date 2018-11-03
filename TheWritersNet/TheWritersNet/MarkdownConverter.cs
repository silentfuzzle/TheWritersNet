using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheWritersNet
{
    public static class MarkdownConverter
    {
        public static string MarkdownToHTML(string markdown)
        {
            return HyperlinkToHTML(NewlineToHTML(HashtagToHTML(FormattingToHTML(markdown))));
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

        private static string HyperlinkToHTML(string markdown)
        {
            int index = 0;
            int lastIndex = 0;
            string nextSymbol = "[";
            char hyperlinkType = 'x';
            int[] brackets = new int[2];

            while (index < markdown.Length)
            {
                index = markdown.IndexOf(nextSymbol, index);
                if (index == -1)
                {
                    if (nextSymbol == "[")
                        index = markdown.Length;
                    else
                    {
                        nextSymbol = "[";
                        index = lastIndex;
                    }
                }
                else
                {
                    if (nextSymbol == "[")
                    {
                        nextSymbol = "]";
                        brackets[0] = index;
                        lastIndex = index + 1;

                        if (index > 0)
                            hyperlinkType = markdown[index - 1];
                    }
                    else if (nextSymbol == "]")
                    {
                        if (index < markdown.Length - 1 && markdown[index + 1] == '(')
                        {
                            nextSymbol = ")";
                            brackets[1] = index;
                        }
                        else
                        {
                            nextSymbol = "[";
                            index = lastIndex;
                        }
                    }
                    else
                    {
                        string hyperlink = markdown.Substring(brackets[1] + 2, index - (brackets[1] + 2));
                        markdown = markdown.Remove(brackets[1], index - brackets[1] + 1);
                        if (hyperlinkType == '?')
                        {
                            markdown = markdown.Insert(brackets[1], "</a>");
                            markdown = markdown.Remove(brackets[0] - 1, 2);
                            markdown = markdown.Insert(brackets[0] - 1, "<a href=\"/Page/Display?pageID=" + hyperlink + "\">");
                        }
                        else if (hyperlinkType == '!')
                        {
                            string altText = markdown.Substring(brackets[0] + 1, brackets[1] - (brackets[0] + 1));
                            markdown = markdown.Remove(brackets[0] - 1, brackets[1] - brackets[0] + 1);
                            markdown = markdown.Insert(brackets[0] - 1, "<img src=\"" + hyperlink + "\" alt\"" + altText + "\" class=\"img-responsive\">");
                        }
                        else
                        {
                            markdown = markdown.Insert(brackets[1], "</a>");
                            markdown = markdown.Remove(brackets[0], 1);
                            markdown = markdown.Insert(brackets[0], "<a href=\"" + hyperlink + "\" target=\"_blank\" rel=\"noopener\">");
                        }

                        nextSymbol = "[";
                    }
                }
            }

            return markdown;
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