﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWritersNetData.Models
{
    public class PermissionModel
    {
        public int PermissionID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int WebsiteID { get; set; }
        public int AbilityID { get; set; }
        public string Ability { get; set; }
    }
}
