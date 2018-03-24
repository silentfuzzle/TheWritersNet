using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using TheWritersNetData.Models;
using TheWritersNetData.Models.Websites;

namespace TheWritersNetData.DBConnectors
{
    class SQLServerDapperConnector : IDBConnector
    {
        private string connectionString = @"Server=localhost\SQLEXPRESS;Database=TheWritersNetDB;Trusted_Connection=True;";

        public void InsertUser(UserModel user)
        {
            List<UserModel> users = new List<UserModel>() { user };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spUser_Insert @LoginID, @UserName", users);
            }
        }

        public UserModel GetUser(string loginID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                IEnumerable<UserModel> users = connection.Query<UserModel>("WebsiteData.spUser_Select @LoginID", new { LoginID = loginID });
                if (users.Count() > 0)
                    return users.First();
            }

            return null;
        }

        public void InsertPermission(PermissionModel permission)
        {
            List<PermissionModel> permissions = new List<PermissionModel>() { permission };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spPermission_Insert @WebsiteID, @UserID, @Ability", permissions);
            }
        }

        public void InsertWebsite(NewWebsiteModel website)
        {
            List<NewWebsiteModel> websites = new List<NewWebsiteModel>() { website };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spWebsite_Insert @Title, @LoginID, @Visibility, @Description", websites);
            }
        }

        public void DeleteWebsite(int websiteID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spWebsite_Delete @WebsiteID", new { WebsiteID = websiteID });
            }
        }

        public List<UserWebsiteModel> SelectUserWebsites(string loginID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                return connection.Query<UserWebsiteModel>("WebsiteData.spWebsite_SelectForUser @LoginID", new { LoginID = loginID }).ToList();
            }
        }

        public List<PublicWebsiteModel> SelectPublicWebsites()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                return connection.Query<PublicWebsiteModel>("WebsiteData.spWebsite_SelectPublic").ToList();
            }
        }
    }
}
