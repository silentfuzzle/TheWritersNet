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
    public class TagController : Controller
    {
        // GET: Tag
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Create(int websiteID)
        {
            return View(new TagModel() { WebsiteID = websiteID });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(TagModel tag)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();

            if (tag.WebsiteID == -1)
            {
                tag.LoginID = User.Identity.GetUserId();
                db.InsertUserTag(tag);
            }
            else
                db.InsertWebsiteTag(tag);

            return Return(tag);
        }

        [Authorize]
        public ActionResult Edit(int tagID, int websiteID, string text)
        {
            return View(new TagModel() { TagID = tagID, WebsiteID = websiteID, Text = text });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(TagModel tag)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();

            if (tag.WebsiteID == -1)
            {
                tag.LoginID = User.Identity.GetUserId();
                db.UpdateWebsiteTag(tag);
            }
            else
                db.UpdateUserTag(tag);

            return Return(tag);
        }

        [Authorize]
        public ActionResult Delete(int tagID, int websiteID, string text)
        {
            return View(new TagModel() { TagID = tagID, WebsiteID = websiteID, Text = text });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(TagModel tag)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            if (tag.WebsiteID == -1)
            {
                tag.LoginID = User.Identity.GetUserId();
                db.DeleteUserTag(tag);
            }
            else
                db.DeleteWebsiteTag(tag);

            return Return(tag);
        }

        private ActionResult Return(TagModel tag)
        {
            if (tag.WebsiteID == -1)
                return RedirectToAction("Edit", "Profile");
            else
                return RedirectToAction("EditFromID", "Website", new { websiteID = tag.WebsiteID });
        }
    }
}