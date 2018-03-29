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
    public class SocialMediaController : Controller
    {
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.SocialMediaOptions = DropDownGenerator.GetSocialMediaDropDown(1);

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(SocialMediaModel model)
        {
            model.LoginID = User.Identity.GetUserId();
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.InsertSocialMedia(model);

            return RedirectToAction("Edit", "Profile");
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            SocialMediaModel model = db.SelectSocialMedia(id);

            ViewBag.SocialMediaOptions = DropDownGenerator.GetSocialMediaDropDown(model.SocialMediaID);

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(SocialMediaModel model)
        {
            model.LoginID = User.Identity.GetUserId();
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.UpdateSocialMedia(model);

            return RedirectToAction("Edit", "Profile");
        }

        [Authorize]
        public ActionResult Delete(SocialMediaModel model)
        {
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(int UserSocialMediaID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.DeleteSocialMedia(UserSocialMediaID);

            return RedirectToAction("Edit", "Profile");
        }
    }
}