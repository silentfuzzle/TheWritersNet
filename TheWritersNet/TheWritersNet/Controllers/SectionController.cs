using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheWritersNet.Models;
using TheWritersNetData.Models;
using TheWritersNetData.DBConnectors;
using TheWritersNetLogic;

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
            List<DBSectionModel> sections = db.SelectPagePositions(pageID);
            sections[0].PageID = pageID;

            return View(sections);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SelectSections(List<DBSectionModel> sections)
        {
            List<DBSectionModel> removePositions = new List<DBSectionModel>();
            List<DBSectionModel> addPositions = new List<DBSectionModel>();
            foreach (DBSectionModel section in sections)
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
        public ActionResult Display(int websiteID, int pageID, int sectionID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            DBSectionModel section = db.SelectSection(sectionID);
            section.PageID = pageID;
            section.Text = MarkdownConverter.MarkdownToHTML(section.Text, websiteID);

            return View(ConvertSectionModel(section));
        }

        [Authorize]
        public ActionResult Create(int pageID)
        {
            return View(new SectionModel() { PageID = pageID });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(SectionModel section)
        {
            if (ModelState.IsValid)
            {
                IDBConnector db = DBConnectorFactory.GetDBConnector();
                int index = db.InsertSection(ConvertSectionModel(section));

                List<SectionLinkModel> links = MarkdownConverter.FindInternalLinks(section.Text, index);
                db.InsertSectionLinks(links);

                return RedirectToAction("EditFromID", "Page", new { pageID = section.PageID });
            }

            return View(section);
        }

        [Authorize]
        public ActionResult Edit(int pageID, int sectionID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            DBSectionModel section = db.SelectSection(sectionID);
            section.PageID = pageID;

            return View(ConvertSectionModel(section));
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(SectionModel section)
        {
            if (ModelState.IsValid)
            {
                IDBConnector db = DBConnectorFactory.GetDBConnector();
                DBSectionModel dbSection = ConvertSectionModel(section);
                db.UpdateSection(dbSection);
                db.UpdatePosition(dbSection);

                List<SectionLinkModel> links = MarkdownConverter.FindInternalLinks(section.Text, section.SectionID);
                db.MergeSectionLinks(links, section.SectionID);

                return RedirectToAction("EditFromID", "Page", new { pageID = section.PageID });
            }

            return View(section);
        }

        [Authorize]
        public ActionResult Delete(int pageID, int sectionID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            DBSectionModel section = db.SelectSection(sectionID);
            section.PageID = pageID;

            return View(section);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(DBSectionModel section)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.DeleteSection(section.SectionID);

            return RedirectToAction("EditFromID", "Page", new { pageID = section.PageID });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Remove(int sectionID, int pageID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.DeleteSectionFromPage(sectionID, pageID);
            
            return RedirectToAction("EditFromID", "Page", new { pageID });
        }

        private SectionModel ConvertSectionModel(DBSectionModel section)
        {
            return new SectionModel()
            {
                PageID = section.PageID,
                DisplayTitle = section.DisplayTitle,
                Position = section.Position,
                Text = section.Text,
                Title = section.Title
            };
        }

        private DBSectionModel ConvertSectionModel(SectionModel section)
        {
            return new DBSectionModel()
            {
                PageID = section.PageID,
                SectionID = section.SectionID,
                DisplayTitle = section.DisplayTitle,
                Position = section.Position,
                Text = section.Text,
                Title = section.Title
            };
        }
    }
}