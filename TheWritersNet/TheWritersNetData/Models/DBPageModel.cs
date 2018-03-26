using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWritersNetData.Models
{
    public class DBPageModel
    {
        public int PageID { get; set; }
        public int WebsiteID { get; set; }
        public string Title { get; set; }
        public bool HomePage { get; set; }
    }
}
