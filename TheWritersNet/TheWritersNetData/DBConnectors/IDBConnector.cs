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

        List<PermissionModel> SelectWebsitePermissions(int websiteID);

        #endregion

        List<TagModel> SelectWebsiteTags(int websiteID);

        #region Websites

        void InsertWebsite(NewWebsiteModel website);

        void DeleteWebsite(int websiteID);

        List<UserWebsiteModel> SelectUserWebsites(string loginID);

        List<PublicWebsiteModel> SelectPublicWebsites();

        #endregion

        List<PageModel> SelectWebsitePages(int websiteID);
    }
}
