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
            section.Text = HyperlinkToHTML(NewlineToHTML(MarkdownToHTML(section.Text)));

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

        private string HyperlinkToHTML(string markdown)
        {
            int index = 0;
            int lastIndex = 0;
            string nextSymbol = "[";
            char hyperlinkType = 'x';
            int[] brackets = new int[2];

            while (index < markdown.Length)
            {
                index = markdown.IndexOf(nextSymbol, index);
                if (index == -1)
                {
                    if (nextSymbol == "[")
                        index = markdown.Length;
                    else
                    {
                        nextSymbol = "[";
                        index = lastIndex;
                    }
                }
                else
                {
                    if (nextSymbol == "[")
                    {
                        nextSymbol = "]";
                        brackets[0] = index;
                        lastIndex = index + 1;

                        if (index > 0)
                            hyperlinkType = markdown[index - 1];
                    }
                    else if (nextSymbol == "]")
                    {
                        if (index < markdown.Length - 1 && markdown[index + 1] == '(')
                        {
                            nextSymbol = ")";
                            brackets[1] = index;
                        }
                        else
                        {
                            nextSymbol = "[";
                            index = lastIndex;
                        }
                    }
                    else
                    {
                        string hyperlink = markdown.Substring(brackets[1] + 2, index - (brackets[1] + 2));
                        markdown = markdown.Remove(brackets[1], index - brackets[1] + 1);
                        if (hyperlinkType == '?')
                        {
                            markdown = markdown.Insert(brackets[1], "</a>");
                            markdown = markdown.Remove(brackets[0] - 1, 2);
                            markdown = markdown.Insert(brackets[0] - 1, "<a href=\"/Page/Display?pageID=" + hyperlink + "\">");
                        }
                        else if (hyperlinkType == '!')
                        {
                            string altText = markdown.Substring(brackets[0] + 1, brackets[1] - (brackets[0] + 1));
                            markdown = markdown.Remove(brackets[0] - 1, brackets[1] - brackets[0] + 1);
                            markdown = markdown.Insert(brackets[0] - 1, "<img src=\"" + hyperlink + "\" alt\"" + altText + "\" class=\"img-responsive\">");
                        }
                        else
                        {
                            markdown = markdown.Insert(brackets[1], "</a>");
                            markdown = markdown.Remove(brackets[0], 1);
                            markdown = markdown.Insert(brackets[0], "<a href=\"" + hyperlink + "\" target=\"_blank\" rel=\"noopener\">");
                        }

                        nextSymbol = "[";
                    }
                }
            }

            return markdown;
        }

        private string NewlineToHTML(string markdown)
        {
            markdown = markdown.Replace("\r\n", "<br>");
            markdown = markdown.Replace("\n", "<br>");

            return markdown;
        }

        private string MarkdownToHTML(string markdown)
        {
            markdown = SymbolPairToHTML("**", "strong", markdown);
            markdown = SymbolPairToHTML("*", "em", markdown);
            markdown = SymbolPairToHTML("_", "em", markdown);

            return markdown;
        }

        private string SymbolPairToHTML(string symbol, string htmlTag, string markdown)
        {
            int index = 0;
            int openIndex = -1;
            while (index < markdown.Length)
            {
                index = markdown.IndexOf(symbol, index);
                if (index == -1)
                    index = markdown.Length;
                else if (openIndex == -1)
                    openIndex = index;
                else
                {
                    markdown = markdown.Remove(index, symbol.Length);
                    markdown = markdown.Insert(index, "</" + htmlTag + ">");
                    markdown = markdown.Remove(openIndex, symbol.Length);
                    markdown = markdown.Insert(openIndex, "<" + htmlTag + ">");
                    openIndex = -1;
                }

                index++;
            }

            return markdown;
        }
    }
}