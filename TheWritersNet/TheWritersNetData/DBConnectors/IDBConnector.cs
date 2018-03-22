using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWritersNetData.Models;

namespace TheWritersNetData
{
    public interface IDBConnector
    {
        void InsertUser(string loginID);

        UserModel GetUser(string loginID);

        void InsertPermission(PermissionModel permission);

        void InsertWebsite(WebsiteModel website);

        List<WebsiteModel> GetWebsites(int userID);
    }
}
