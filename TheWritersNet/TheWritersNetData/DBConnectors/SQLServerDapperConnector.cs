using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using TheWritersNetData.Models;

namespace TheWritersNetData.DBConnectors
{
    public class SQLServerDapperConnector : IDBConnector
    {
        private string connectionString = @"Server=localhost\SQLEXPRESS;Database=TheWritersNetDB;Trusted_Connection=True;";

        public void InsertUser(string loginID)
        {
            List<UserModel> users = new List<UserModel>() { new UserModel() { LoginID = loginID } };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spUser_Insert @LoginID", users);
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

        public void InsertWebsite(WebsiteModel website)
        {
            List<WebsiteModel> websites = new List<WebsiteModel>() { website };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute(" WebsiteData.spWebsite_Insert @Title, @Owner, @Visibility, @Description", websites);
            }
        }

        public List<WebsiteModel> GetWebsites(int userID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                return connection.Query<WebsiteModel>("WebsiteData.spWebsite_Select @UserID", new { UserID = userID }).ToList();
            }
        }
    }
}
