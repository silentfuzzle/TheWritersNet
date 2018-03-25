using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheWritersNetData.Models;

namespace TheWritersNet.Models
{
    public class WebsiteModel
    {
        public int WebsiteID { get; set; }
        public string Title { get; set; }
        public int Visibility { get; set; }
        public string Description { get; set; }
        public List<TagModel> Tags { get; set; }
        public List<PageModel> Pages { get; set; }
        public List<PermissionModel> Permissions { get; set; }
    }
}