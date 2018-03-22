using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWritersNetData.Models
{
    public class PermissionModel
    {
        public int UserID { get; set; }
        public int WebsiteID { get; set; }
        public int Ability { get; set; }
    }
}
