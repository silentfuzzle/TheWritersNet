using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheWritersNet.Models;
using TheWritersNetData.Models;
using TheWritersNetData.DBConnectors;

namespace TheWritersNet.Controllers
{
    public class PermissionController : Controller
    {
        [Authorize]
        public ActionResult Create(int websiteID)
        {
            ViewBag.PermissionOptions = DropDownGenerator.GetPermissionDropDown(0);

            PermissionModel permission = new PermissionModel() { WebsiteID = websiteID };
            return View(permission);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(PermissionModel permission)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.InsertPermission(permission);

            return RedirectToAction("EditFromID", "Website", new { websiteID = permission.WebsiteID });
        }

        [Authorize]
        public ActionResult Edit(int websiteID, string userName, int abilityID)
        {
            ViewBag.PermissionOptions = DropDownGenerator.GetPermissionDropDown(abilityID - 1);

            PermissionModel permission = new PermissionModel() { WebsiteID = websiteID, UserName = userName, AbilityID = abilityID };
            return View(permission);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(PermissionModel permission)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.UpdatePermission(permission);

            return RedirectToAction("EditFromID", "Website", new { websiteID = permission.WebsiteID });
        }

        [Authorize]
        public ActionResult Delete(int websiteID, string userName, string ability)
        {
            PermissionModel permission = new PermissionModel() { WebsiteID = websiteID, UserName = userName, Ability = ability };
            return View(permission);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(PermissionModel permission)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.DeletePermission(permission.WebsiteID, permission.UserName);

            return RedirectToAction("EditFromID", "Website", new { websiteID = permission.WebsiteID });
        }
    }
}