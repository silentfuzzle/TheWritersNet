using System;
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

        void UpdatePermission(PermissionModel permission);

        void DeletePermission(int permissionID);

        List<PermissionModel> SelectWebsitePermissions(int websiteID);

        #endregion

        #region Tags

        void InsertWebsiteTag(TagModel tag);

        void InsertUserTag(TagModel tag);

        void UpdateWebsiteTag(TagModel tag);

        void UpdateUserTag(TagModel tag);

        void DeleteWebsiteTag(TagModel tag);

        void DeleteUserTag(TagModel tag);

        List<TagModel> SelectWebsiteTags(int websiteID);

        List<TagModel> SelectUserTags(string loginID);

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

        DBPageModel SelectPage(int pageID);

        List<DBPageModel> SelectWebsitePages(int websiteID);

        #endregion
        
        #region Section Positions

        void InsertPositions(List<SectionModel> sections);

        void UpdatePosition(SectionModel section);

        void DeletePositions(List<SectionModel> sections);

        List<SectionModel> SelectPagePositions(int pageID);

        #endregion

        #region Sections

        void InsertSection(SectionModel section);

        void UpdateSection(SectionModel section);

        void DeleteSection(int sectionID);

        void DeleteSectionFromPage(int sectionID, int pageID);

        SectionModel SelectSection(int sectionID);

        List<SectionModel> SelectEditPageSections(int pageID);

        List<SectionModel> SelectViewPageSections(int pageID);

        #endregion
    }
}
