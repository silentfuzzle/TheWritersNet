using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWritersNetData.Models.Websites
{
    public class NewWebsiteModel
    {
        public int WebsiteID { get; set; }

        public string Title { get; set; }

        public string LoginID { get; set; }

        public int VisibilityID { get; set; }

        public string Description { get; set; }
    }
}
