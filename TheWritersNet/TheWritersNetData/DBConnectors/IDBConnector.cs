﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWritersNetData.Models;
using TheWritersNetData.Models.Websites;

namespace TheWritersNetData.DBConnectors
{
    public interface IDBConnector
    {
        #region Users

        void InsertUser(UserModel user);

        void UpdateUser(UserModel user);

        UserModel SelectUser(string loginID);

        #endregion

        #region Permissions

        void InsertPermission(PermissionModel permission);

        List<PermissionModel> SelectWebsitePermissions(int websiteID);

        #endregion

        #region Tags

        void InsertWebsiteTag(TagModel tag);

        void UpdateWebsiteTag(TagModel tag);

        void DeleteWebsiteTag(TagModel tag);

        List<TagModel> SelectWebsiteTags(int websiteID);

        #endregion

        #region Websites

        void InsertWebsite(NewWebsiteModel website);

        void UpdateWebsiteHomePage(int websiteID, int pageID, bool homePage);

        void UpdateWebsite(UserWebsiteModel website);

        void DeleteWebsite(int websiteID);

        UserWebsiteModel SelectWebsite(int websiteID);

        List<UserWebsiteModel> SelectUserWebsites(string loginID);

        List<PublicWebsiteModel> SelectPublicWebsites();

        #endregion

        #region Pages

        void InsertPage(DBPageModel page);

        void UpdatePage(DBPageModel page);

        void DeletePage(int pageID);

        List<DBPageModel> SelectWebsitePages(int websiteID);

        #endregion

        List<SectionModel> SelectPageSections(int pageID);
    }
}
