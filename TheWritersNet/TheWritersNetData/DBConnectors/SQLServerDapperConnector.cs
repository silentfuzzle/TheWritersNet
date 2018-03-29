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

        #region Users

        public void InsertUser(UserModel user)
        {
            List<UserModel> users = new List<UserModel>() { user };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spUser_Insert @LoginID, @UserName", users);
            }
        }

        public void UpdateUser(UserModel user)
        {
            List<UserModel> users = new List<UserModel>() { user };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spUser_Update @LoginID, @UserName, @Description", users);
            }
        }

        public UserModel SelectUser(string loginID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                IEnumerable<UserModel> users = connection.Query<UserModel>("WebsiteData.spUser_Select @LoginID", new { LoginID = loginID });
                if (users.Count() > 0)
                    return users.First();
            }

            return null;
        }

        #endregion

        #region Social Media

        public void InsertSocialMedia(SocialMediaModel socialMedia)
        {
            List<SocialMediaModel> accounts = new List<SocialMediaModel>() { socialMedia };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spSocialMedia_Insert @LoginID, @SocialMediaID, @Address, @AlternateText", accounts);
            }
        }

        public void UpdateSocialMedia(SocialMediaModel socialMedia)
        {
            List<SocialMediaModel> accounts = new List<SocialMediaModel>() { socialMedia };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spSocialMedia_Update @UserSocialMediaID, @SocialMediaID, @Address, @AlternateText", accounts);
            }
        }

        public void DeleteSocialMedia(int userSocialMediaID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spSocialMedia_Delete @UserSocialMediaID", new { UserSocialMediaID = userSocialMediaID });
            }
        }

        public SocialMediaModel SelectSocialMedia(int userSocialMediaID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                IEnumerable<SocialMediaModel> socialMedia = connection.Query<SocialMediaModel>(
                    "WebsiteData.spSocialMedia_Select @UserSocialMediaID", 
                    new { UserSocialMediaID = userSocialMediaID }).ToList();

                if (socialMedia.Count() > 0)
                    return socialMedia.First();
            }

            return null;
        }

        public List<SocialMediaModel> SelectUserSocialMedia(string loginID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                return connection.Query<SocialMediaModel>("WebsiteData.spSocialMedia_SelectForUser @LoginID", new { LoginID = loginID }).ToList();
            }
        }

        public List<SocialMediaModel> SelectSocialMediaOptions()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                return connection.Query<SocialMediaModel>("WebsiteData.spSocialMedia_SelectOptions").ToList();
            }
        }

        #endregion

        #region Permissions

        public void InsertPermission(PermissionModel permission)
        {
            List<PermissionModel> permissions = new List<PermissionModel>() { permission };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spPermission_Insert @WebsiteID, @UserName, @AbilityID", permissions);
            }
        }

        public void UpdatePermission(PermissionModel permission)
        {
            List<PermissionModel> permissions = new List<PermissionModel>() { permission };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spPermission_Update @PermissionID, @AbilityID", permissions);
            }
        }

        public void DeletePermission(int permissionID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spPermission_Delete @PermissionID", new { PermissionID = permissionID });
            }
        }

        public List<PermissionModel> SelectWebsitePermissions(int websiteID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                return connection.Query<PermissionModel>("WebsiteData.spPermission_Select @WebsiteID", new { WebsiteID = websiteID }).ToList();
            }
        }

        #endregion

        #region Tags

        public void InsertWebsiteTag(TagModel tag)
        {
            List<TagModel> tags = new List<TagModel>() { tag };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spTag_InsertForWebsite @WebsiteID, @Text", tags);
            }
        }

        public void InsertUserTag(TagModel tag)
        {
            List<TagModel> tags = new List<TagModel>() { tag };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spTag_InsertForUser @LoginID, @Text", tags);
            }
        }

        public void UpdateWebsiteTag(TagModel tag)
        {
            List<TagModel> tags = new List<TagModel>() { tag };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spTag_UpdateForWebsite @TagID, @WebsiteID, @Text", tags);
            }
        }

        public void UpdateUserTag(TagModel tag)
        {
            List<TagModel> tags = new List<TagModel>() { tag };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spTag_UpdateForUser @TagID, @LoginID, @Text", tags);
            }
        }

        public void DeleteWebsiteTag(TagModel tag)
        {
            List<TagModel> tags = new List<TagModel>() { tag };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spTag_DeleteForWebsite @TagID, @WebsiteID", tags);
            }
        }

        public void DeleteUserTag(TagModel tag)
        {
            List<TagModel> tags = new List<TagModel>() { tag };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spTag_DeleteForUser @TagID, @LoginID", tags);
            }
        }

        public List<TagModel> SelectWebsiteTags(int websiteID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                return connection.Query<TagModel>("WebsiteData.spTag_SelectForWebsite @WebsiteID", new { WebsiteID = websiteID }).ToList();
            }
        }

        public List<TagModel> SelectUserTags(string loginID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                return connection.Query<TagModel>("WebsiteData.spTag_SelectForUser @LoginID", new { LoginID = loginID }).ToList();
            }
        }

        #endregion

        #region Websites

        public void InsertWebsite(NewWebsiteModel website)
        {
            List<NewWebsiteModel> websites = new List<NewWebsiteModel>() { website };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spWebsite_Insert @Title, @LoginID, @VisibilityID, @Description", websites);
            }
        }

        public void UpdateWebsite(UserWebsiteModel website)
        {
            List<UserWebsiteModel> websites = new List<UserWebsiteModel>() { website };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spWebsite_Update @WebsiteID, @Title, @VisibilityID, @Description", websites);
            }
        }

        public void UpdateWebsiteHomePage(int websiteID, int pageID, bool homePage)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spWebsite_UpdateHomePage @WebsiteID, @PageID, @HomePage", new { WebsiteID = websiteID, PageID = pageID, HomePage = homePage });
            }
        }

        public void DeleteWebsite(int websiteID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spWebsite_Delete @WebsiteID", new { WebsiteID = websiteID });
            }
        }

        public UserWebsiteModel SelectWebsite(int websiteID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                IEnumerable<UserWebsiteModel> websites = connection.Query<UserWebsiteModel>("WebsiteData.spWebsite_Select @WebsiteID", new { WebsiteID = websiteID }).ToList();
                if (websites.Count() > 0)
                    return websites.First();
            }

            return null;
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

        #endregion

        #region Pages

        public void InsertPage(DBPageModel page)
        {
            List<DBPageModel> pages = new List<DBPageModel>() { page };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spPage_Insert @WebsiteID, @Title, @DisplayTitle, @HomePage", pages);
            }
        }

        public void UpdatePage(DBPageModel page)
        {
            List<DBPageModel> pages = new List<DBPageModel>() { page };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spPage_Update @PageID, @Title, @DisplayTitle", pages);
            }
        }

        public void DeletePage(int pageID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spPage_Delete @PageID", new { PageID = pageID });
            }
        }

        public DBPageModel SelectPage(int pageID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                IEnumerable<DBPageModel> pages = connection.Query<DBPageModel>("WebsiteData.spPage_Select @PageID", new { PageID = pageID }).ToList();
                if (pages.Count() > 0)
                    return pages.First();
            }

            return null;
        }

        public List<DBPageModel> SelectWebsitePages(int websiteID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                List<DBPageModel> pages = connection.Query<DBPageModel>("WebsiteData.spPage_SelectForWebsite @WebsiteID", new { WebsiteID = websiteID }).ToList();
                foreach (DBPageModel page in pages)
                    page.WebsiteID = websiteID;
                return pages;
            }
        }

        #endregion

        #region Section Positions

        public void InsertPositions(List<SectionModel> sections)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spPosition_Insert @PageID, @SectionID, @Position, @DisplayTitle", sections);
            }
        }

        public void UpdatePosition(SectionModel section)
        {
            List<SectionModel> sections = new List<SectionModel>() { section };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spPosition_Update @SectionID, @PageID, @Position, @DisplayTitle", sections);
            }
        }

        public void DeletePositions(List<SectionModel> sections)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spPosition_Delete @PageID, @SectionID", sections);
            }
        }

        public List<SectionModel> SelectPagePositions(int pageID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                return connection.Query<SectionModel>("WebsiteData.spPosition_SelectForPage @PageID", new { PageID = pageID }).ToList();
            }
        }

        #endregion

        #region Sections

        public void InsertSection(SectionModel section)
        {
            List<SectionModel> sections = new List<SectionModel>() { section };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spSection_Insert @PageID, @Title, @Text, @Position, @DisplayTitle", sections);
            }
        }

        public void UpdateSection(SectionModel section)
        {
            List<SectionModel> sections = new List<SectionModel>() { section };

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spSection_Update @SectionID, @Title, @Text", sections);
            }
        }

        public void DeleteSection(int sectionID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spSection_Delete @SectionID", new { SectionID = sectionID });
            }
        }

        public void DeleteSectionFromPage(int sectionID, int pageID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute("WebsiteData.spSection_DeleteFromPage @SectionID, @PageID", new { SectionID = sectionID, PageID = pageID });
            }
        }

        public SectionModel SelectSection(int sectionID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                IEnumerable<SectionModel> sections = connection.Query<SectionModel>("WebsiteData.spSection_Select @SectionID", new { SectionID = sectionID }).ToList();
                if (sections.Count() > 0)
                    return sections.First();
            }

            return null;
        }

        public List<SectionModel> SelectEditPageSections(int pageID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                return connection.Query<SectionModel>("WebsiteData.spSection_SelectForPageEdit @PageID", new { PageID = pageID }).ToList();
            }
        }

        public List<SectionModel> SelectViewPageSections(int pageID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                return connection.Query<SectionModel>("WebsiteData.spSection_SelectForPageView @PageID", new { PageID = pageID }).ToList();
            }
        }

        #endregion
    }
}
