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
        public ActionResult Display(int pageID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            
            DBPageModel dbPage = db.SelectPage(pageID);
            PageModel page = new PageModel()
            {
                Title = dbPage.Title,
                DisplayTitle = dbPage.DisplayTitle,
                PageID = dbPage.PageID,
                WebsiteID = dbPage.WebsiteID
            };
            page.Sections = db.SelectViewPageSections(pageID);

            foreach (SectionModel section in page.Sections)
                section.Text = MarkdownConverter.MarkdownToHTML(section.Text);

            return View("Display", "_WebsiteLayout", page);
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
        public ActionResult EditFromID(int pageID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            DBPageModel page = db.SelectPage(pageID);

            return RedirectToAction("Edit", page);
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
            db.UpdatePage(new DBPageModel() { PageID = page.PageID, Title = page.Title, DisplayTitle = page.DisplayTitle });
            db.UpdateWebsiteHomePage(page.WebsiteID, page.PageID, page.HomePage);

            page.Sections = db.SelectEditPageSections(page.PageID);

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
                HomePage = webpage.HomePage,
                DisplayTitle = webpage.DisplayTitle
            };

            IDBConnector db = DBConnectorFactory.GetDBConnector();
            page.Sections = db.SelectEditPageSections(webpage.PageID);

            return page;
        }
    }
}