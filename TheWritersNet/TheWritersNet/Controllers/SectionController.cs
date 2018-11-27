using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using TheWritersNet.Models;
using TheWritersNetData.Models;
using TheWritersNetData.DBConnectors;
using TheWritersNetLogic;

namespace TheWritersNet.Controllers
{
    public class SectionController : Controller
    {
        [Authorize]
        public ActionResult SelectSections(int pageID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            List<DBSectionModel> dbSections = db.SelectPagePositions(pageID);

            List<SectionModel> sections = new List<SectionModel>();
            foreach (DBSectionModel section in dbSections)
            {
                section.PageID = pageID;
                sections.Add(ConvertSectionModel(section));
            }

            return View(dbSections);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SelectSections(List<SectionModel> sections)
        {
            if (ModelState.IsValid)
            {
                List<DBSectionModel> removePositions = new List<DBSectionModel>();
                List<DBSectionModel> addPositions = new List<DBSectionModel>();
                foreach (SectionModel section in sections)
                {
                    if (section.IsSelected)
                        addPositions.Add(ConvertSectionModel(section));
                    else
                        removePositions.Add(ConvertSectionModel(section));
                }

                IDBConnector db = DBConnectorFactory.GetDBConnector();
                db.InsertPositions(addPositions);
                db.DeletePositions(removePositions);

                return RedirectToAction(StringKeys.EDIT_FROM_ID, StringKeys.PAGE_CONTROLLER, new { pageID = sections[0].PageID });
            }

            return View(sections);
        }

        [Authorize]
        public ActionResult Display(int websiteID, int pageID, int sectionID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            DBSectionModel section = db.SelectSection(sectionID);
            if (section != null)
            {
                section.PageID = pageID;
                section.Text = MarkdownConverter.MarkdownToHTML(section.Text, websiteID);

                return View(ConvertSectionModel(section));
            }

            return RedirectToAction(StringKeys.EDIT_FROM_ID, StringKeys.PAGE_CONTROLLER, new { pageID });
        }

        [Authorize]
        public ActionResult Create(int pageID)
        {
            return View(new SectionModel() { PageID = pageID });
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(SectionModel section)
        {
            if (GetValid(section))
            {
                IDBConnector db = DBConnectorFactory.GetDBConnector();
                int index = db.InsertSection(ConvertSectionModel(section));

                List<SectionLinkModel> links = MarkdownConverter.FindInternalLinks(section.Text, index);
                db.InsertSectionLinks(links);

                return RedirectToAction(StringKeys.EDIT_FROM_ID, StringKeys.PAGE_CONTROLLER, new { pageID = section.PageID });
            }

            return View(section);
        }

        [Authorize]
        public ActionResult Edit(int pageID, int sectionID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            DBSectionModel section = db.SelectSection(sectionID);
            if (section != null)
            {
                section.PageID = pageID;

                return View(ConvertSectionModel(section));
            }

            return RedirectToAction(StringKeys.EDIT_FROM_ID, StringKeys.PAGE_CONTROLLER, new { pageID });
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(SectionModel section)
        {
            if (GetValid(section))
            {
                IDBConnector db = DBConnectorFactory.GetDBConnector();
                DBSectionModel dbSection = ConvertSectionModel(section);
                db.UpdateSection(dbSection);
                db.UpdatePosition(dbSection);

                List<SectionLinkModel> links = MarkdownConverter.FindInternalLinks(section.Text, section.SectionID);
                db.MergeSectionLinks(links, section.SectionID);

                return RedirectToAction(StringKeys.EDIT_FROM_ID, StringKeys.PAGE_CONTROLLER, new { pageID = section.PageID });
            }

            return View(section);
        }

        [Authorize]
        public ActionResult Delete(int pageID, int sectionID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            DBSectionModel section = db.SelectSection(sectionID);
            if (section != null)
            {
                section.PageID = pageID;

                return View(ConvertSectionModel(section));
            }

            return RedirectToAction(StringKeys.EDIT_FROM_ID, StringKeys.PAGE_CONTROLLER, new { pageID });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(SectionModel section)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.DeleteSection(section.SectionID);

            return RedirectToAction(StringKeys.EDIT_FROM_ID, StringKeys.PAGE_CONTROLLER, new { pageID = section.PageID });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Remove(int sectionID, int pageID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            db.DeleteSectionFromPage(sectionID, pageID);
            
            return RedirectToAction(StringKeys.EDIT_FROM_ID, StringKeys.PAGE_CONTROLLER, new { pageID });
        }

        private SectionModel ConvertSectionModel(DBSectionModel section)
        {
            SectionModel convertedSection = new SectionModel()
            {
                PageID = section.PageID,
                SectionID = section.SectionID,
                DisplayTitle = section.DisplayTitle,
                Text = section.Text,
                Title = section.Title
            };

            int position;
            int.TryParse(section.Position, out position);
            convertedSection.Position = position;

            return convertedSection;
        }

        private DBSectionModel ConvertSectionModel(SectionModel section)
        {
            return new DBSectionModel()
            {
                PageID = section.PageID,
                SectionID = section.SectionID,
                DisplayTitle = section.DisplayTitle,
                Position = section.Position.ToString(),
                Text = section.Text,
                Title = section.Title
            };
        }

        private bool GetValid(SectionModel section)
        {
            bool valid = ModelState.IsValid;
            if (section.Title.Contains(">") || section.Title.Contains("<"))
            {
                section.Title = "";
                valid = false;
            }
            if (section.Text.Contains(">") || section.Text.Contains("<"))
            {
                section.Text = "";
                valid = false;
            }

            return valid;
        }
    }
}