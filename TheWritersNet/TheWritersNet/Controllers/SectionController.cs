using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheWritersNetData.Models;
using TheWritersNetData.DBConnectors;

namespace TheWritersNet.Controllers
{
    public class SectionController : Controller
    {
        // GET: Section
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult SelectSections(int pageID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            List<SectionModel> sections = db.SelectPagePositions(pageID);
            sections[0].PageID = pageID;

            return View(sections);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SelectSections(List<SectionModel> sections)
        {
            List<SectionModel> removePositions = new List<SectionModel>();
            List<SectionModel> addPositions = new List<SectionModel>();
            foreach (SectionModel section in sections)
            {
                section.PageID = sections[0].PageID;
                if (section.IsSelected)
                    addPositions.Add(section);
                else
                    removePositions.Add(section);
            }

            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.InsertPositions(addPositions);
            db.DeletePositions(removePositions);

            return RedirectToAction("EditFromID", "Page", new { pageID = sections[0].PageID });
        }

        [Authorize]
        public ActionResult Display(int pageID, int sectionID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            SectionModel section = db.SelectSection(sectionID);
            section.PageID = pageID;
            section.Text = MarkdownConverter.MarkdownToHTML(section.Text);

            return View(section);
        }

        [Authorize]
        public ActionResult Create(int pageID)
        {
            return View(new SectionModel() { PageID = pageID });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(SectionModel page)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.InsertSection(page);

            return RedirectToAction("EditFromID", "Page", new { pageID = page.PageID });
        }

        [Authorize]
        public ActionResult Edit(int pageID, int sectionID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            SectionModel section = db.SelectSection(sectionID);
            section.PageID = pageID;

            return View(section);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(SectionModel page)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.UpdateSection(page);
            db.UpdatePosition(page);

            return RedirectToAction("EditFromID", "Page", new { pageID = page.PageID });
        }

        [Authorize]
        public ActionResult Delete(int pageID, int sectionID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            SectionModel section = db.SelectSection(sectionID);
            section.PageID = pageID;

            return View(section);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(SectionModel section)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.DeleteSectionFromPage(section.SectionID, section.PageID);

            return RedirectToAction("EditFromID", "Page", new { pageID = section.PageID });
        }
    }
}