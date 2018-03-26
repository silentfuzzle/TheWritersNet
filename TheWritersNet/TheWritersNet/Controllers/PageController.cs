using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheWritersNet.Models;
using TheWritersNetData.DBConnectors;
using TheWritersNetData.Models;

namespace TheWritersNet.Controllers
{
    public class PageController : Controller
    {
        // GET: Page
        public ActionResult Display()
        {
            return View();
        }

        [Authorize]
        public ActionResult Create(int websiteID)
        {
            return View(new DBPageModel() { WebsiteID = websiteID });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(DBPageModel page)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.InsertPage(page);

            return RedirectToAction("EditFromID", "Website", new { websiteID = page.WebsiteID });
        }

        [Authorize]
        public ActionResult Edit(DBPageModel webpage)
        {
            return View(PopulatePageModel(webpage));
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(PageModel page)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.UpdatePage(new DBPageModel() { PageID = page.PageID, Title = page.Title });
            db.UpdateWebsiteHomePage(page.WebsiteID, page.PageID, page.HomePage);

            page.Sections = db.SelectPageSections(page.PageID);

            return View(page);
        }

        [Authorize]
        public ActionResult Delete(DBPageModel webpage)
        {
            return View(PopulatePageModel(webpage));
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(int PageID, int WebsiteID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.DeletePage(PageID);

            return RedirectToAction("EditFromID", "Website", new { websiteID = WebsiteID });
        }

        private PageModel PopulatePageModel(DBPageModel webpage)
        {
            PageModel page = new PageModel()
            {
                PageID = webpage.PageID,
                WebsiteID = webpage.WebsiteID,
                Title = webpage.Title,
                HomePage = webpage.HomePage
            };

            IDBConnector db = DBConnectorFactory.GetDBConnector();
            page.Sections = db.SelectPageSections(webpage.PageID);

            return page;
        }
    }
}