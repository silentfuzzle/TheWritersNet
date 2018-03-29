using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TheWritersNet.Models;
using TheWritersNetData.Models;
using TheWritersNetData.DBConnectors;

namespace TheWritersNet.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        [Authorize]
        public ActionResult Index()
        {
            return View(PopulateUserModel());
        }

        [Authorize]
        public ActionResult Edit()
        {
            return View(PopulateUserModel());
        }

        private UserViewModel PopulateUserModel()
        {
            string loginID = User.Identity.GetUserId();
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            UserModel user = db.SelectUser(loginID);

            UserViewModel userView = new UserViewModel()
            {
                UserName = user.UserName,
                Description = user.Description
            };
            userView.Tags = db.SelectUserTags(loginID);
            userView.SocialMediaAccounts = db.SelectUserSocialMedia(loginID);

            return userView;
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(UserViewModel user)
        {
            UserModel dbUser = new UserModel()
            {
                LoginID = User.Identity.GetUserId(),
                UserName = user.UserName,
                Description = user.Description
            };

            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.UpdateUser(dbUser);
            user.Tags = db.SelectUserTags(dbUser.LoginID);
            user.SocialMediaAccounts = db.SelectUserSocialMedia(dbUser.LoginID);

            return View(user);
        }
    }
}