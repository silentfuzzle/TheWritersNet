using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TheWritersNet.Models;
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
        public ActionResult Edit(UserWebsiteModel website)
        {
            WebsiteModel details = new WebsiteModel()
            {
                WebsiteID = website.WebsiteID,
                Title = website.Title,
                Visibility = website.VisibilityID,
                Description = website.Description
            };

            IDBConnector db = DBConnectorFactory.GetDBConnector();
            details.Tags = db.SelectWebsiteTags(website.WebsiteID);
            details.Pages = db.SelectWebsitePages(website.WebsiteID);
            details.Permissions = db.SelectWebsitePermissions(website.WebsiteID);

            return View(details);
        }

        [Authorize]
        public ActionResult Delete(UserWebsiteModel website)
        {
            return View(website);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(int websiteID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.DeleteWebsite(websiteID);

            return RedirectToAction("MyWebsites");
        }
    }
}