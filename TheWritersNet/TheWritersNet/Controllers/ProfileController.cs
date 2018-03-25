using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
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
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            UserModel user = db.SelectUser(User.Identity.GetUserId());

            return View(user);
        }

        [Authorize]
        public ActionResult Edit()
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            UserModel user = db.SelectUser(User.Identity.GetUserId());

            return View(user);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(UserModel user)
        {
            user.LoginID = User.Identity.GetUserId();
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.UpdateUser(user);

            return View(user);
        }
    }
}