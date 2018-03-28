using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheWritersNetData.Models;

namespace TheWritersNet.Models
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public string Description { get; set; }
        public List<TagModel> Tags { get; set; }
        public List<SocialMediaModel> SocialMediaAccounts { get; set; }
    }
}