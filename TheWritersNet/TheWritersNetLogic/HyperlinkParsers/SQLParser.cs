using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWritersNetData.Models;

namespace TheWritersNetLogic.HyperlinkParsers
{
    class SQLParser : HyperlinkParser
    {
        private int sectionID;

        public List<SectionLinkModel> Links { get; private set; }

        public SQLParser(int sectionID)
        {
            this.sectionID = sectionID;
            this.Links = new List<SectionLinkModel>();
        }

        protected override void ProcessHyperlink(string hyperlink, char hyperlinkType)
        {
            if (hyperlinkType == '?')
            {
                int pageID;
                bool success = int.TryParse(hyperlink, out pageID);
                if (success)
                    this.Links.Add(new SectionLinkModel() { PageID = pageID, SectionID = this.sectionID });
            }
        }
    }
}
