using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWritersNetData.Models.Websites
{
    public class PublicWebsiteModel
    {
        public int WebsiteID { get; set; }

        public string Title { get; set; }

        public int HomePageID { get; set; }

        public int OwnerID { get; set; }

        public string OwnerName { get; set; }

        public string Description { get; set; }
    }
}
