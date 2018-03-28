using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWritersNetData.Models
{
    public class TagModel
    {
        public int TagID { get; set; }
        public string LoginID { get; set; }
        public int WebsiteID { get; set; } = -1;
        public string Text { get; set; }
    }
}
