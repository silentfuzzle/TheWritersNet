using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWritersNetData.DBConnectors;

namespace TheWritersNetLogic.HyperlinkParsers
{
    class HTMLParser : HyperlinkParser
    {
        private int websiteID;

        public HTMLParser(int websiteID)
        {
            this.websiteID = websiteID;
        }

        protected override void ProcessHyperlink(string hyperlink, char hyperlinkType)
        {
            Markdown = Markdown.Remove(Brackets[1], Index - Brackets[1] + 1);
            bool maliciousCodeDetected = (hyperlink.Contains("javascript:"));

            if (hyperlinkType == '?')
            {
                // Make sure the parenthesis only contain a number, the ID of the page to link to
                int pageID;
                bool success = int.TryParse(hyperlink, out pageID);
                bool pageInWebsite = false;

                if (success)
                {
                    // Check if the page belongs to this website
                    IDBConnector db = DBConnectorFactory.GetDBConnector();
                    pageInWebsite = db.CheckPageExistence(pageID, this.websiteID);
                }

                if (success && pageInWebsite)
                {
                    // Create an action that opens the linked page
                    Markdown = Markdown.Insert(Brackets[1], "</a>");
                    Markdown = Markdown.Remove(Brackets[0] - 1, 2);
                    Markdown = Markdown.Insert(Brackets[0] - 1, "<a href=\"javascript:openPage(" + pageID + ")\">");
                }
                else
                    // Remove all traces of the link's markdown
                    Markdown = Markdown.Remove(Brackets[0] - 1, 2);
            }
            else if (hyperlinkType == '!')
            {
                if (maliciousCodeDetected)
                    // Remove all traces of the image's markdown
                    Markdown = Markdown.Remove(Brackets[0] - 1, 2);
                else
                {
                    string altText = Markdown.Substring(Brackets[0] + 1, Brackets[1] - (Brackets[0] + 1));
                    Markdown = Markdown.Remove(Brackets[0] - 1, Brackets[1] - Brackets[0] + 1);
                    Markdown = Markdown.Insert(Brackets[0] - 1, "<img src=\"" + hyperlink + "\" alt\"" + altText + "\" class=\"img-responsive\">");
                }
            }
            else
            {
                if (maliciousCodeDetected)
                    // Remove all traces of the link's markdown
                    Markdown = Markdown.Remove(Brackets[0], 1);
                else
                {
                    Markdown = Markdown.Insert(Brackets[1], "</a>");
                    Markdown = Markdown.Remove(Brackets[0], 1);
                    Markdown = Markdown.Insert(Brackets[0], "<a href=\"" + hyperlink + "\" target=\"_blank\" rel=\"noopener\">");
                }
            }
        }
    }
}
