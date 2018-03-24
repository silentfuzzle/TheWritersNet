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
        void InsertUser(UserModel user);

        UserModel GetUser(string loginID);

        void InsertPermission(PermissionModel permission);

        void InsertWebsite(NewWebsiteModel website);

        void DeleteWebsite(int websiteID);

        List<UserWebsiteModel> SelectUserWebsites(string loginID);

        List<PublicWebsiteModel> SelectPublicWebsites();
    }
}
