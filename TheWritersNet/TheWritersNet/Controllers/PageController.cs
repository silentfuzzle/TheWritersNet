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
                PageID = dbPage.PageID
            };
            page.Sections = db.SelectViewPageSections(pageID);

            foreach (SectionModel section in page.Sections)
                section.Text = MarkdownConverter.MarkdownToHTML(section.Text);

            return View(page);
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

        [Authorize]
        [HttpPost]
        public JsonResult GetMap(int PageID)
        {
            string test = "{\"graph\": [], \"multigraph\": false, \"nodes\": [{\"type\": 2, \"id\": \"169.0\", \"spine\": 21, \"title\": \"Zarkis\", \"pages\": 5.0, \"label\": \"169.0\"}, {\"type\": 2, \"id\": \"187.0\", \"spine\": 24, \"title\": \"Ramda\", \"pages\": 11.0, \"label\": \"187.0\"}, {\"type\": 2, \"id\": \"129.0\", \"spine\": 13, \"title\": \"Dante Klerik\", \"pages\": 5.0, \"label\": \"129.0\"}, {\"type\": 2, \"id\": \"282.0\", \"spine\": 38, \"title\": \"A Timeline of Baltarik History\", \"pages\": 4.0, \"label\": \"282.0\"}, {\"type\": 2, \"id\": \"214.0\", \"spine\": 28, \"title\": \"Baltarik Military\", \"pages\": 4.0, \"label\": \"214.0\"}, {\"type\": 2, \"id\": \"121.0\", \"spine\": 11, \"title\": \"Shmee Bogart\", \"pages\": 3.0, \"label\": \"121.0\"}, {\"type\": 2, \"id\": \"286.0\", \"spine\": 39, \"title\": \"About the Author\", \"pages\": 1.0, \"label\": \"286.0\"}, {\"type\": 1, \"id\": \"20.0\", \"spine\": 3, \"title\": \"You seem to have misplaced your zarkis.\", \"pages\": 61.0, \"label\": \"20.0\"}, {\"type\": 2, \"id\": \"287.0\", \"spine\": 40, \"title\": \"About Pajar\", \"pages\": 2.0, \"label\": \"287.0\"}, {\"type\": 2, \"id\": \"143.0\", \"spine\": 16, \"title\": \"Kadas (People)\", \"pages\": 4.0, \"label\": \"143.0\"}, {\"type\": 2, \"id\": \"120.0\", \"spine\": 10, \"title\": \"Palo's house\", \"pages\": 1.0, \"label\": \"120.0\"}, {\"type\": 2, \"id\": \"280.0\", \"spine\": 37, \"title\": \"Barkes Lookup\", \"pages\": 2.0, \"label\": \"280.0\"}, {\"type\": 2, \"id\": \"124.0\", \"spine\": 12, \"title\": \"Trash\", \"pages\": 5.0, \"label\": \"124.0\"}, {\"type\": 1, \"id\": \"3.0\", \"spine\": 2, \"title\": \"Now we won't be able to train for the rest of the day.\", \"pages\": 17.0, \"label\": \"3.0\"}, {\"type\": 2, \"id\": \"177.0\", \"spine\": 23, \"title\": \"Pajar\", \"pages\": 10.0, \"label\": \"177.0\"}, {\"type\": 2, \"id\": \"289.0\", \"spine\": 41, \"title\": \"Adventurous Reader\", \"pages\": 1.0, \"label\": \"289.0\"}, {\"type\": 2, \"id\": \"275.0\", \"spine\": 36, \"title\": \"The Great Peace Organization\", \"pages\": 5.0, \"label\": \"275.0\"}, {\"type\": 2, \"id\": \"237.0\", \"spine\": 31, \"title\": \"Jadu Calendar\", \"pages\": 2.0, \"label\": \"237.0\"}, {\"type\": 2, \"id\": \"154.0\", \"spine\": 19, \"title\": \"Kadas (Planet)\", \"pages\": 8.0, \"label\": \"154.0\"}, {\"type\": 2, \"id\": \"162.0\", \"spine\": 20, \"title\": \"Pajar Court\", \"pages\": 7.0, \"label\": \"162.0\"}, {\"type\": 2, \"id\": \"224.0\", \"spine\": 30, \"title\": \"The Ramda Choosing\", \"pages\": 13.0, \"label\": \"224.0\"}, {\"type\": 2, \"id\": \"116.0\", \"spine\": 8, \"title\": \"Destruction with Ta\", \"pages\": 1.0, \"label\": \"116.0\"}, {\"type\": 2, \"id\": \"218.0\", \"spine\": 29, \"title\": \"Jigo's War\", \"pages\": 6.0, \"label\": \"218.0\"}, {\"type\": 2, \"id\": \"142.0\", \"spine\": 15, \"title\": \"Kadas (Disambiguation)\", \"pages\": 1.0, \"label\": \"142.0\"}, {\"type\": 2, \"id\": \"117.0\", \"spine\": 9, \"title\": \"Respect and communication\", \"pages\": 3.0, \"label\": \"117.0\"}, {\"type\": 2, \"id\": \"134.0\", \"spine\": 14, \"title\": \"Jigo Kadas\", \"pages\": 8.0, \"label\": \"134.0\"}, {\"type\": 2, \"id\": \"151.0\", \"spine\": 18, \"title\": \"Jadu Galaxy\", \"pages\": 3.0, \"label\": \"151.0\"}, {\"type\": 2, \"id\": \"239.0\", \"spine\": 32, \"title\": \"Hunting Gloves\", \"pages\": 4.0, \"label\": \"239.0\"}, {\"type\": 2, \"id\": \"252.0\", \"spine\": 34, \"title\": \"Baltarik Laboratories\", \"pages\": 17.0, \"label\": \"252.0\"}, {\"type\": 2, \"id\": \"202.0\", \"spine\": 26, \"title\": \"Portisk\", \"pages\": 3.0, \"label\": \"202.0\"}, {\"type\": 2, \"id\": \"114.0\", \"spine\": 7, \"title\": \"Hako's dares\", \"pages\": 2.0, \"label\": \"114.0\"}, {\"type\": 2, \"id\": \"174.0\", \"spine\": 22, \"title\": \"Sonkis\", \"pages\": 3.0, \"label\": \"174.0\"}, {\"type\": 2, \"id\": \"269.0\", \"spine\": 35, \"title\": \"Baltarik Social Hierarchy\", \"pages\": 6.0, \"label\": \"269.0\"}, {\"type\": 2, \"id\": \"147.0\", \"spine\": 17, \"title\": \"Kadas (Bloodline)\", \"pages\": 4.0, \"label\": \"147.0\"}, {\"type\": 1, \"id\": \"81.0\", \"spine\": 4, \"title\": \"Did something happen two months ago?\", \"pages\": 31.0, \"label\": \"81.0\"}, {\"type\": 2, \"id\": \"243.0\", \"spine\": 33, \"title\": \"Kantisk Energy\", \"pages\": 9.0, \"label\": \"243.0\"}, {\"type\": 1, \"id\": \"1\", \"spine\": 1, \"title\": \"Pajar\", \"pages\": 2.0, \"label\": \"1\"}, {\"type\": 2, \"id\": \"198.0\", \"spine\": 25, \"title\": \"Baltarik\", \"pages\": 4.0, \"label\": \"198.0\"}, {\"type\": 2, \"id\": \"112.0\", \"spine\": 5, \"title\": \"Thank you\", \"pages\": 1.0, \"label\": \"112.0\"}, {\"type\": 2, \"id\": \"113.0\", \"spine\": 6, \"title\": \"Wipe it away\", \"pages\": 1.0, \"label\": \"113.0\"}, {\"type\": 2, \"id\": \"205.0\", \"spine\": 27, \"title\": \"Jauni\", \"pages\": 9.0, \"label\": \"205.0\"}], \"directed\": true, \"links\": [{\"source\": 2, \"type\": \"hyperlink\", \"target\": 14}, {\"source\": 7, \"type\": \"scroll\", \"target\": 34}, {\"source\": 13, \"type\": \"scroll\", \"target\": 36}, {\"source\": 13, \"type\": \"hyperlink\", \"target\": 27}, {\"source\": 13, \"type\": \"scroll\", \"target\": 7}, {\"source\": 14, \"type\": \"hyperlink\", \"target\": 37}, {\"source\": 18, \"type\": \"hyperlink\", \"target\": 37}, {\"source\": 26, \"type\": \"hyperlink\", \"target\": 18}, {\"source\": 34, \"type\": \"hyperlink\", \"target\": 2}, {\"source\": 36, \"type\": \"scroll\", \"target\": 13}, {\"source\": 37, \"type\": \"hyperlink\", \"target\": 26}]}";

            Map test2 = new Map();
            test2.directed = true;
            test2.links = new List<Link>() { new Link() { source = 0, target = 2, type = "scroll" }, new Link() { source = 2, target = 1, type = "hyperlink" } };
            test2.nodes = new List<Node>() { new Node() { type = 2, id = "169.0", spine = 1, label = "169.0", title = "Node 1", pages = 5.0f }, new Node() { type = 2, id = "187.0", spine = 2, label = "187.0", title = "Node 2", pages = 11.0f }, new Node() { type = 1, id = "129.0", spine = 3, label = "129.0", title = "Node 3", pages = 5.0f } };
            System.Web.Script.Serialization.JavaScriptSerializer t = new System.Web.Script.Serialization.JavaScriptSerializer();
            string lol = t.Serialize(test2);

            Map dMap = t.Deserialize<Map>(lol);

            return Json(test);
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