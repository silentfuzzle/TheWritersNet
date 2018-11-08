using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;
using TheWritersNetData.DBConnectors;
using TheWritersNetData.Models;
using TheWritersNetLogic;

namespace TheWritersNet.Controllers
{
    public class MapController : Controller
    {
        [Authorize]
        [HttpPost]
        public JsonResult Display(int pageID, int websiteID)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            IDBConnector db = DBConnectorFactory.GetDBConnector();

            string loginID = User.Identity.GetUserId();
            WebsiteUserModel user = db.SelectWebsiteUser(websiteID, loginID);

            Map map;
            string sMap;
            if (user == null)
            {
                List<DBPageModel> pages = db.SelectWebsitePages(websiteID);

                map = new Map();
                foreach (DBPageModel page in pages)
                    map.nodes.Add(CreateNode(page));

                sMap = serializer.Serialize(map);
                db.InsertWebsiteUser(new WebsiteUserModel() { LoginID = loginID, WebsiteID = websiteID, Map = sMap, History = "" });
            }
            else if (user.Dirty)
            {
                map = serializer.Deserialize<Map>(user.Map);
                
                // Store the links in the user's map in a list and dictionary of start to end pages
                List<LinkModel> userLinks = new List<LinkModel>();
                Dictionary<int, HashSet<int>> linkDictionary = new Dictionary<int, HashSet<int>>();
                foreach (Link link in map.links)
                {
                    int startPage = map.nodes[link.source].spine;
                    int endPage = map.nodes[link.target].spine;
                    userLinks.Add(new LinkModel() { StartPage = startPage, EndPage = endPage });

                    if (linkDictionary.ContainsKey(startPage))
                        linkDictionary[startPage].Add(endPage);
                    else
                        linkDictionary.Add(startPage, new HashSet<int>() { endPage });
                }

                // Check if all the user's links exist in the master map
                List<LinkModel> authorLinks = db.SelectSectionLinks(userLinks);
                if (authorLinks.Count != userLinks.Count)
                {
                    // Update the dictionary of links to include only the links specified by the author
                    linkDictionary = new Dictionary<int, HashSet<int>>();
                    foreach (LinkModel link in authorLinks)
                    {
                        if (linkDictionary.ContainsKey(link.StartPage))
                            linkDictionary[link.StartPage].Add(link.EndPage);
                        else
                            linkDictionary.Add(link.StartPage, new HashSet<int>() { link.EndPage });
                    }
                }
                
                // Check if all the pages exist in the user's map
                List<DBPageModel> pages = db.SelectWebsitePages(websiteID);
                Dictionary<int, DBPageModel> pageDictionary = new Dictionary<int, DBPageModel>();
                foreach (DBPageModel page in pages)
                    pageDictionary.Add(page.PageID, page);

                bool nodesModified = false;
                int n = 0;
                while (n < map.nodes.Count)
                {
                    Node node = map.nodes[n];
                    if (!pageDictionary.ContainsKey(node.spine))
                    {
                        // This page no longer exists in the master map
                        map.nodes.RemoveAt(n);
                        nodesModified = true;
                    }
                    else
                    {
                        // This page still exists, make sure the title is up to date
                        node.title = pageDictionary[node.spine].Title;
                        pageDictionary.Remove(node.spine);
                        n++;
                    }
                }

                // Add any new pages from the master map to the user's map
                foreach (DBPageModel page in pageDictionary.Values)
                {
                    map.nodes.Add(CreateNode(page));
                    nodesModified = true;
                }
                
                if (nodesModified)
                {
                    // The nodes that exist in the user's map have been modified

                    // Map the current page IDs to their indexes in the list
                    Dictionary<int, int> pageToIndex = new Dictionary<int, int>();
                    for (int i = 0; i < map.nodes.Count; i++)
                        pageToIndex.Add(map.nodes[i].spine, i);

                    // Recreate the user's links from the new node indexes
                    map.links = new List<Link>();
                    foreach (KeyValuePair<int, HashSet<int>> link in linkDictionary)
                        foreach (int endPage in link.Value)
                            map.links.Add(new Link() { source = pageToIndex[link.Key], target = pageToIndex[endPage], type = "hyperlink" });
                }

                // Save the new map
                sMap = serializer.Serialize(map);
                db.UpdateWebsiteUser(new WebsiteUserModel() { LoginID = loginID, WebsiteID = websiteID, Map = sMap, History = "" });
            }
            else
                sMap = user.Map;


            return Json(sMap);
        }

        private Node CreateNode(DBPageModel page)
        {
            return (new Node() { title = page.Title, id = page.PageID.ToString(), spine = page.PageID, type = (page.HomePage) ? 1 : 2 });
        }

        [Authorize]
        [HttpPost]
        public JsonResult Update(int websiteID, int prevPageID, int nextPageID)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            List<LinkModel> links = db.SelectSectionLinks(new List<LinkModel>() { new LinkModel() { StartPage = prevPageID, EndPage = nextPageID } });

            string loginID = User.Identity.GetUserId();
            WebsiteUserModel user = db.SelectWebsiteUser(websiteID, loginID);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Map map = serializer.Deserialize<Map>(user.Map);

            string sMap = null;
            if (links.Count > 0)
            {
                Link link = new Link() { type = "hyperlink", source = -1, target = -1 };
                for (int n = 0; n < map.nodes.Count; n++)
                {
                    Node node = map.nodes[n];
                    if (node.spine == prevPageID)
                        link.source = n;
                    else if (node.spine == nextPageID)
                        link.target = n;

                    if (link.source != -1 && link.target != -1)
                        break;
                }

                bool exists = false;
                foreach (Link eLink in map.links)
                {
                    if (eLink.source == link.source && eLink.target == link.target)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    map.links.Add(link);

                    sMap = serializer.Serialize(map);
                    db.UpdateWebsiteUser(new WebsiteUserModel() { LoginID = loginID, WebsiteID = websiteID, Map = sMap, History = "" });
                }
            }

            if (sMap == null)
                 sMap = serializer.Serialize(map);

            return Json(sMap);
        }
    }
}