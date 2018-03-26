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
            ViewBag.VisibilityOptions = DropDownGenerator.GetVisibilityDropDown(0);

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
        public ActionResult EditFromID(int websiteID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            UserWebsiteModel website = db.SelectWebsite(websiteID);

            return RedirectToAction("Edit", website);
        }

        [Authorize]
        public ActionResult Edit(UserWebsiteModel website)
        {
            ViewBag.VisibilityOptions = DropDownGenerator.GetVisibilityDropDown(website.VisibilityID - 1);

            return View(PopulateWebsiteModel(website));
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(WebsiteModel website)
        {
            UserWebsiteModel updateWebsite = new UserWebsiteModel()
            {
                WebsiteID = website.WebsiteID,
                Title = website.Title,
                VisibilityID = website.VisibilityID,
                Description = website.Description
            };

            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.UpdateWebsite(updateWebsite);

            ViewBag.VisibilityOptions = DropDownGenerator.GetVisibilityDropDown(website.VisibilityID - 1);

            return View(PopulateWebsiteModel(updateWebsite));
        }

        private WebsiteModel PopulateWebsiteModel(UserWebsiteModel website)
        {
            WebsiteModel details = new WebsiteModel()
            {
                WebsiteID = website.WebsiteID,
                Title = website.Title,
                VisibilityID = website.VisibilityID,
                Description = website.Description
            };

            IDBConnector db = DBConnectorFactory.GetDBConnector();
            details.Tags = db.SelectWebsiteTags(website.WebsiteID);
            details.Pages = db.SelectWebsitePages(website.WebsiteID);
            details.Permissions = db.SelectWebsitePermissions(website.WebsiteID);

            return details;
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