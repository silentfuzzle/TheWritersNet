using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWritersNetData.Models
{
    public class WebsiteModel
    {
        public int WebsiteID { get; set; }

        public string Title { get; set; }

        public int HomePage { get; set; }

        public int Owner { get; set; }

        public int Visibility { get; set; }

        public string Description { get; set; }
    }
}
