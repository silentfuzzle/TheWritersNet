using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheWritersNetData.Models;

namespace TheWritersNet.Models
{
    public class PageModel
    {
        public int PageID { get; set; }
        public int WebsiteID { get; set; }
        public string Title { get; set; }
        public bool HomePage { get; set; }
        public bool DisplayTitle { get; set; }
        public List<SectionModel> Sections { get; set; }
    }
}