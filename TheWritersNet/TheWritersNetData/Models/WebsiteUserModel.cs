using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWritersNetData.Models
{
    public class WebsiteUserModel
    {
        public string LoginID { get; set; }
        public int WebsiteID { get; set; }
        public bool Dirty { get; set; }
        public string Map { get; set; }
        public string History { get; set; }
    }
}
