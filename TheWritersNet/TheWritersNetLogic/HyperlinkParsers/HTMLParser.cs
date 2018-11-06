using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWritersNetLogic.HyperlinkParsers
{
    class HTMLParser : HyperlinkParser
    {
        protected override void ProcessHyperlink(string hyperlink, char hyperlinkType)
        {
            Markdown = Markdown.Remove(Brackets[1], Index - Brackets[1] + 1);
            if (hyperlinkType == '?')
            {
                Markdown = Markdown.Insert(Brackets[1], "</a>");
                Markdown = Markdown.Remove(Brackets[0] - 1, 2);
                Markdown = Markdown.Insert(Brackets[0] - 1, "<a href=\"javascript:openPage(" + hyperlink + ")\">");
            }
            else if (hyperlinkType == '!')
            {
                string altText = Markdown.Substring(Brackets[0] + 1, Brackets[1] - (Brackets[0] + 1));
                Markdown = Markdown.Remove(Brackets[0] - 1, Brackets[1] - Brackets[0] + 1);
                Markdown = Markdown.Insert(Brackets[0] - 1, "<img src=\"" + hyperlink + "\" alt\"" + altText + "\" class=\"img-responsive\">");
            }
            else
            {
                hyperlink = "javascript:alert(\'ooooooooo noooooooo\')";
                Markdown = Markdown.Insert(Brackets[1], "</a>");
                Markdown = Markdown.Remove(Brackets[0], 1);
                Markdown = Markdown.Insert(Brackets[0], "<a href=\"" + hyperlink + "\" target=\"_blank\" rel=\"noopener\">");
            }
        }
    }
}
