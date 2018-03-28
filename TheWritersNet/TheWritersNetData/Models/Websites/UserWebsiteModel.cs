using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWritersNetData.Models.Websites
{
    public class UserWebsiteModel
    {
        public int WebsiteID { get; set; }

        public string Title { get; set; }

        public int HomePageID { get; set; }

        public int VisibilityID { get; set; }

        public string Visibility { get; set; }

        public string Description { get; set; }

        public int AbilityID { get; set; }

        public string Ability { get; set; }
    }
}
