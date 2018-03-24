using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TheWritersNetData.DBConnectors;
using TheWritersNetData.Models.Websites;

namespace TheWritersNet.Controllers
{
    public class WebsiteController : Controller
    {
        // GET: Website
        public ActionResult Index()
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            return View(db.SelectPublicWebsites());
        }

        [Authorize]
        public ActionResult MyWebsites()
        {
            string userID = User.Identity.GetUserId();
            List<UserWebsiteModel> websites = new List<UserWebsiteModel>();
            if (userID != null)
            {
                IDBConnector db = DBConnectorFactory.GetDBConnector();
                websites = db.SelectUserWebsites(userID);
            }

            return View(websites);
        }

        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(NewWebsiteModel website)
        {
            website.LoginID = User.Identity.GetUserId();
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.InsertWebsite(website);

            return RedirectToAction("MyWebsites");
        }

        [Authorize]
        public ActionResult Delete(UserWebsiteModel website)
        {
            return View(website);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(int WebsiteID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.DeleteWebsite(WebsiteID);

            return RedirectToAction("MyWebsites");
        }
    }
}