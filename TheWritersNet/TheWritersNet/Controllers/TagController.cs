using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheWritersNetData.Models;
using TheWritersNetData.DBConnectors;

namespace TheWritersNet.Controllers
{
    public class TagController : Controller
    {
        // GET: Tag
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Create(int websiteID, int userID)
        {
            return View(new TagModel() { WebsiteID = websiteID, UserID = userID });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(TagModel tag)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.InsertWebsiteTag(tag);

            return Return(tag);
        }

        [Authorize]
        public ActionResult Edit(int tagID, int websiteID, int userID, string text)
        {
            return View(new TagModel() { TagID = tagID, WebsiteID = websiteID, UserID = userID, Text = text });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(TagModel tag)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.UpdateWebsiteTag(tag);

            return Return(tag);
        }

        [Authorize]
        public ActionResult Delete(int tagID, int websiteID, int userID, string text)
        {
            return View(new TagModel() { TagID = tagID, WebsiteID = websiteID, UserID = userID, Text = text });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(TagModel tag)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.DeleteWebsiteTag(tag);

            return Return(tag);
        }

        private ActionResult Return(TagModel tag)
        {
            if (tag.UserID == -1)
                return RedirectToAction("EditFromID", "Website", new { websiteID = tag.WebsiteID });
            else
                return RedirectToAction("Edit", "Profile");
        }
    }
}