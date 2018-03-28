CREATE PROCEDURE WebsiteData.spUser_Insert
	@LoginID nvarchar(50), @UserName nvarchar(50)
AS
	SET NOCOUNT ON;

	INSERT INTO WebsiteData.[User] (LoginID, UserName)
	VALUES(@LoginID, @UserName)
GO

CREATE PROCEDURE WebsiteData.spUser_Update
	@LoginID nvarchar(50), @UserName nvarchar(50), @Description nvarchar(1000)
AS
	SET NOCOUNT ON;

	UPDATE WebsiteData.[User]
	SET WebsiteData.[User].UserName = @UserName, WebsiteData.[User].Description = @Description
	WHERE WebsiteData.[User].LoginID = @LoginID;
GO

CREATE PROCEDURE WebsiteData.spUser_SelectID
	@LoginID nvarchar(50),
	@UserID int OUTPUT
AS
	SET NOCOUNT ON;
		
	SET @UserID = (SELECT TOP 1 UserID FROM WebsiteData.[User] WHERE @LoginID = LoginID)
GO

CREATE PROCEDURE WebsiteData.spUser_Select
	@LoginID nvarchar(50)
AS
	SET NOCOUNT ON;

	SELECT * FROM WebsiteData.[User] WHERE LoginID = @LoginID;
GO

CREATE PROCEDURE WebsiteData.spPermission_Insert
	@WebsiteID int, @UserName nvarchar(50), @AbilityID int
AS
	SET NOCOUNT ON;
	
	INSERT INTO WebsiteData.WebsitePermission (WebsiteID, UserID, AbilityID)
	VALUES(@WebsiteID, (SELECT TOP 1 UserID FROM WebsiteData.[User] WHERE UserName = @UserName), @AbilityID)
GO

CREATE PROCEDURE WebsiteData.spPermission_Update
	@PermissionID int, @AbilityID int
AS
	SET NOCOUNT ON;

	UPDATE WebsiteData.WebsitePermission
	SET AbilityID = @AbilityID
	WHERE WebsitePermission.PermissionID = @PermissionID;
GO

CREATE PROCEDURE WebsiteData.spPermission_Delete
	@PermissionID int
AS
	SET NOCOUNT ON;
	
	DELETE WebsitePermission
	FROM WebsiteData.WebsitePermission
	WHERE WebsitePermission.PermissionID = @PermissionID;
GO

CREATE PROCEDURE WebsiteData.spPermission_Select
	@WebsiteID int
AS
	SET NOCOUNT ON;

	SELECT WebsitePermission.PermissionID, [User].UserName, WebsitePermission.AbilityID, Ability.[Name] AS Ability
	FROM WebsiteData.[User]
	INNER JOIN WebsiteData.WebsitePermission ON [User].UserID = WebsitePermission.UserID
	INNER JOIN WebsiteData.Ability ON Ability.AbilityID = WebsitePermission.AbilityID
	WHERE WebsitePermission.WebsiteID = @WebsiteID;
GO

CREATE PROCEDURE WebsiteData.spTag_InsertForWebsite
	@WebsiteID int, @Text nvarchar(100)
AS
	SET NOCOUNT ON;

	DECLARE @TagID int, @UserID int;

	IF NOT EXISTS (SELECT TagID FROM WebsiteData.Tag WHERE Tag.[Text] = @Text)
		BEGIN
			INSERT INTO WebsiteData.Tag ([Text], NumUsers, NumWebsites)
			VALUES(@Text, 0, 1)
			SET @TagID = SCOPE_IDENTITY()

			INSERT INTO WebsiteData.TagWebsite (TagID, WebsiteID)
			VALUES (@TagID, @WebsiteID)
		END
	ELSE 
		BEGIN
			SET @TagID = (SELECT TOP 1 TagID FROM WebsiteData.Tag WHERE [Text] LIKE @Text)

			IF NOT EXISTS (SELECT WebsiteID FROM WebsiteData.TagWebsite WHERE TagID = @TagID AND WebsiteID = @WebsiteID)
				BEGIN
					INSERT INTO WebsiteData.TagWebsite (TagID, WebsiteID)
					VALUES (@TagID, @WebsiteID)
					
					UPDATE Tag
					SET Tag.NumWebsites = Tag.NumWebsites + 1
					FROM WebsiteData.Tag
					INNER JOIN WebsiteData.TagWebsite ON TagWebsite.TagID = Tag.TagID
					WHERE TagWebsite.WebsiteID = @WebsiteID AND TagWebsite.TagID = @TagID;
				END
		END
GO

CREATE PROCEDURE WebsiteData.spTag_InsertForUser
	@LoginID nvarchar(50), @Text nvarchar(100)
AS
	SET NOCOUNT ON;

	DECLARE @TagID int, @UserID int;

	SET @UserID = (SELECT TOP 1 UserID FROM WebsiteData.[User] WHERE @LoginID = LoginID)
	SET @TagID = (SELECT TOP 1 TagID FROM WebsiteData.Tag WHERE [Text] = @Text)

	IF @TagID IS NULL
		BEGIN
			INSERT INTO WebsiteData.Tag ([Text], NumUsers, NumWebsites)
			VALUES(@Text, 1, 0)
			SET @TagID = SCOPE_IDENTITY()

			INSERT INTO WebsiteData.TagUser (TagID, UserID)
			VALUES (@TagID, @UserID)
		END
	ELSE 
		BEGIN
			IF NOT EXISTS (SELECT UserID FROM WebsiteData.TagUser WHERE TagID = @TagID AND UserID = @UserID)
				BEGIN
					INSERT INTO WebsiteData.TagUser (TagID, UserID)
					VALUES (@TagID, @UserID)
					
					UPDATE Tag
					SET Tag.NumUsers = Tag.NumUsers + 1
					FROM WebsiteData.Tag
					INNER JOIN WebsiteData.TagUser ON TagUser.TagID = Tag.TagID
					WHERE TagUser.UserID = @UserID AND Tag.TagID = @TagID;
				END
		END
GO

CREATE PROCEDURE WebsiteData.spTag_DeleteForWebsite
	@TagID int, @WebsiteID int
AS
	SET NOCOUNT ON;
	
	UPDATE WebsiteData.Tag
	SET Tag.NumWebsites = Tag.NumWebsites - 1
	WHERE Tag.TagID = @TagID;

	DELETE TagWebsite
	FROM WebsiteData.TagWebsite
	WHERE TagWebsite.WebsiteID = @WebsiteID AND TagWebsite.TagID = @TagID;

	IF (SELECT (NumUsers + NumWebsites) AS NumUsing FROM WebsiteData.Tag WHERE Tag.TagID = @TagID) = 0
		BEGIN
			DELETE Tag
			FROM WebsiteData.Tag
			WHERE Tag.TagID = @TagID;
		END
GO

CREATE PROCEDURE WebsiteData.spTag_DeleteForUser
	@TagID int, @LoginID nvarchar(50)
AS
	SET NOCOUNT ON;

	UPDATE WebsiteData.Tag
	SET Tag.NumUsers = Tag.NumUsers - 1
	WHERE Tag.TagID = @TagID;

	DELETE TagUser
	FROM WebsiteData.TagUser
	INNER JOIN WebsiteData.[User] ON [User].UserID = TagUser.UserID
	WHERE [User].LoginID = @LoginID AND TagUser.TagID = @TagID;

	IF (SELECT (NumUsers + NumWebsites) AS NumUsing FROM WebsiteData.Tag WHERE Tag.TagID = @TagID) = 0
		BEGIN
			DELETE Tag
			FROM WebsiteData.Tag
			WHERE Tag.TagID = @TagID;
		END
GO

CREATE PROCEDURE WebsiteData.spTag_UpdateForWebsite
	@TagID int, @WebsiteID int, @Text nvarchar(100)
AS
	SET NOCOUNT ON;

	IF @Text != (SELECT [Text] FROM WebsiteData.Tag WHERE Tag.TagID = @TagID)
		BEGIN
			IF (SELECT (NumUsers + NumWebsites) AS NumUsing FROM WebsiteData.Tag WHERE Tag.TagID = @TagID) = 1
				BEGIN
					UPDATE WebsiteData.Tag
					SET Tag.Text = @Text
					WHERE Tag.TagID = @TagID;
				END
			ELSE
				BEGIN
					EXEC WebsiteData.spTag_DeleteForWebsite @TagID, @WebsiteID;
					EXEC WebsiteData.spTag_InsertForWebsite @WebsiteID, @Text;
				END
		END
GO

CREATE PROCEDURE WebsiteData.spTag_UpdateForUser
	@TagID int, @LoginID nvarchar(50), @Text nvarchar(100)
AS
	SET NOCOUNT ON;

	IF @Text != (SELECT [Text] FROM WebsiteData.Tag WHERE Tag.TagID = @TagID)
		BEGIN
			IF (SELECT (NumUsers + NumWebsites) AS NumUsing FROM WebsiteData.Tag WHERE Tag.TagID = @TagID) = 1
				BEGIN
					UPDATE WebsiteData.Tag
					SET Tag.[Text] = @Text
					WHERE Tag.TagID = @TagID;
				END
			ELSE
				BEGIN
					EXEC WebsiteData.spTag_DeleteForUser @TagID, @LoginID;
					EXEC WebsiteData.spTag_InsertForUser @LoginID, @Text;
				END
		END
GO

CREATE PROCEDURE WebsiteData.spTag_DeleteEmpty
AS
	SET NOCOUNT ON;
	
	DELETE Tag
	FROM WebsiteData.Tag
	WHERE Tag.NumUsers = 0 AND Tag.NumWebsites = 0;
GO

CREATE PROCEDURE WebsiteData.spTag_SelectForWebsite
	@WebsiteID int
AS
	SET NOCOUNT ON;

	SELECT Tag.TagID, Tag.[Text]
	FROM WebsiteData.Tag AS Tag
	INNER JOIN WebsiteData.TagWebsite AS TagWebsite ON TagWebsite.TagID = Tag.TagID
	WHERE TagWebsite.WebsiteID = @WebsiteID;
GO

CREATE PROCEDURE WebsiteData.spTag_SelectForUser
	@LoginID nvarchar(50)
AS
	SET NOCOUNT ON;

	SELECT Tag.TagID, Tag.[Text]
	FROM WebsiteData.Tag AS Tag
	INNER JOIN WebsiteData.TagUser ON TagUser.TagID = Tag.TagID
	INNER JOIN WebsiteData.[User] ON TagUser.UserID = [User].UserID
	WHERE [User].LoginID = @LoginID;
GO

CREATE PROCEDURE WebsiteData.spWebsite_Insert
	@Title nvarchar(100), @LoginID nvarchar(50), @VisibilityID int, @Description nvarchar(1000)
AS
	SET NOCOUNT ON;

	DECLARE @WebsiteID int, @UserID int;

	EXEC WebsiteData.spUser_SelectID @LoginID, @UserID OUTPUT;

	INSERT INTO WebsiteData.Website (Title, HomePageID, OwnerID, VisibilityID, [Description])
	VALUES(@Title, NULL, @UserID, @VisibilityID, @Description)
	SET @WebsiteID = SCOPE_IDENTITY();
	
	INSERT INTO WebsiteData.WebsitePermission (WebsiteID, UserID, AbilityID)
	VALUES(@WebsiteID, @UserID, 1)
GO

CREATE PROCEDURE WebsiteData.spWebsite_Update
	@WebsiteID int, @Title nvarchar(100), @VisibilityID int, @Description nvarchar(1000)
AS
	SET NOCOUNT ON;

	UPDATE WebsiteData.Website
	SET Website.Title = @Title, Website.VisibilityID = @VisibilityID, Website.[Description] = @Description
	WHERE Website.WebsiteID = @WebsiteID;
GO

CREATE PROCEDURE WebsiteData.spWebsite_UpdateHomePage
	@WebsiteID int, @PageID int, @HomePage bit
AS
	SET NOCOUNT ON;

	DECLARE @HomePageID int;

	SET @HomePageID = (SELECT TOP 1 Website.HomePageID FROM WebsiteData.Website WHERE WebsiteID = @WebsiteID);

	IF (@HomePageID IS NULL OR (@HomePageID != @PageID AND @HomePage = 1))
		BEGIN
			UPDATE WebsiteData.Website
			SET Website.HomePageID = @PageID
			WHERE Website.WebsiteID = @WebsiteID;
		END
	ELSE
		BEGIN
			IF (@HomePageID = @PageID AND @HomePage = 0)
				BEGIN
					UPDATE WebsiteData.Website
					SET Website.HomePageID = (SELECT TOP 1 WebsitePage.PageID FROM WebsiteData.WebsitePage WHERE WebsiteID = @WebsiteID AND PageID != @PageID)
					WHERE Website.WebsiteID = @WebsiteID;
				END
		END
GO

CREATE PROCEDURE WebsiteData.spWebsite_Delete
	@WebsiteID int
AS
	SET NOCOUNT ON;

	DELETE PageSection
	FROM WebsiteData.PageSection
	INNER JOIN WebsiteData.WebsitePage ON WebsitePage.PageID = PageSection.PageID
	WHERE WebsitePage.WebsiteID = @WebsiteID;

	DELETE Section
	FROM WebsiteData.Section
	WHERE Section.WebsiteID = @WebsiteID;

	DELETE [Page]
	FROM WebsiteData.[Page]
	INNER JOIN WebsiteData.WebsitePage ON WebsitePage.PageID = [Page].PageID
	WHERE WebsitePage.WebsiteID = @WebsiteID;

	DELETE WebsitePage
	FROM WebsiteData.WebsitePage
	WHERE WebsitePage.WebsiteID = @WebsiteID;

	UPDATE Tag
	SET Tag.NumWebsites = Tag.NumWebsites - 1
	FROM WebsiteData.Tag AS Tag
	INNER JOIN WebsiteData.TagWebsite ON TagWebsite.TagID = Tag.TagID
	WHERE TagWebsite.WebsiteID = @WebsiteID;

	EXEC WebsiteData.spTag_DeleteEmpty;

	DELETE TagWebsite
	FROM WebsiteData.TagWebsite
	WHERE TagWebsite.WebsiteID = @WebsiteID;

	DELETE Permission
	FROM WebsiteData.WebsitePermission AS Permission
	WHERE Permission.WebsiteID = @WebsiteID;

	DELETE Website
	FROM WebsiteData.Website AS Website
	WHERE Website.WebsiteID = @WebsiteID;
GO

CREATE PROCEDURE WebsiteData.spWebsite_Select
	@WebsiteID int
AS
	SET NOCOUNT ON;

	SELECT Website.WebsiteID, Website.Title, Website.VisibilityID, Website.[Description]
	FROM WebsiteData.Website
	WHERE Website.[WebsiteID] = @WebsiteID;
GO

CREATE PROCEDURE WebsiteData.spWebsite_SelectForUser
	@LoginID nvarchar(50)
AS
	SET NOCOUNT ON;

	DECLARE @UserID int;
	
	EXEC WebsiteData.spUser_SelectID @LoginID, @UserID OUTPUT;

	SELECT 
		Website.WebsiteID, 
		Website.Title,
		Website.HomePageID,
		Website.VisibilityID, 
		Visibility.[Name] AS Visibility, 
		Website.[Description], 
		Permission.AbilityID, 
		Ability.[Name] AS Ability
	FROM WebsiteData.Website AS Website
	INNER JOIN WebsiteData.WebsitePermission AS Permission ON Website.WebsiteID = Permission.WebsiteID
	INNER JOIN WebsiteData.Ability AS Ability ON Permission.AbilityID = Ability.AbilityID
	INNER JOIN WebsiteData.Visibility AS Visibility ON Website.VisibilityID = Visibility.VisibilityID
	WHERE Permission.UserID = @UserID;
GO

CREATE PROCEDURE WebsiteData.spWebsite_SelectPublic
AS
	SET NOCOUNT ON;

	SELECT Website.WebsiteID, Website.Title, Website.HomePageID, Website.OwnerID, [User].UserName AS OwnerName, Website.[Description]
	FROM WebsiteData.Website AS Website
	INNER JOIN WebsiteData.[User] AS [User] ON [User].UserID = Website.OwnerID
	WHERE Website.VisibilityID = 1;
GO

CREATE PROCEDURE WebsiteData.spPage_Insert
	@WebsiteID int, @Title nvarchar(100), @DisplayTitle bit, @HomePage bit
AS
	SET NOCOUNT ON;

	DECLARE @PageID int;
	
	INSERT INTO WebsiteData.[Page] (Title, DisplayTitle)
	VALUES(@Title, @DisplayTitle);
	SET @PageID = SCOPE_IDENTITY();

	INSERT INTO WebsiteData.WebsitePage (WebsiteID, PageID)
	VALUES (@WebsiteID, @PageID);

	EXEC WebsiteData.spWebsite_UpdateHomePage @WebsiteID, @PageID, @HomePage; 
GO

CREATE PROCEDURE WebsiteData.spPage_Update
	@PageID int, @Title nvarchar(100), @DisplayTitle bit
AS
	SET NOCOUNT ON;
	
	UPDATE WebsiteData.[Page]
	SET [Page].Title = @Title, [Page].DisplayTitle = @DisplayTitle
	WHERE [Page].PageID = @PageID;
GO

CREATE PROCEDURE WebsiteData.spPage_Delete
	@PageID int
AS
	SET NOCOUNT ON;

	DECLARE @WebsiteID int

	SET @WebsiteID = (SELECT TOP 1 WebsitePage.WebsiteID FROM WebsiteData.WebsitePage WHERE WebsitePage.PageID = @PageID);

	EXEC WebsiteData.spWebsite_UpdateHomePage @WebsiteID, @PageID, 0;

	DELETE PageSection
	FROM WebsiteData.PageSection
	WHERE PageSection.PageID = @PageID;
	
	DELETE WebsitePage
	FROM WebsiteData.WebsitePage
	WHERE WebsitePage.PageID = @PageID;

	DELETE [Page]
	FROM WebsiteData.[Page]
	WHERE [Page].PageID = @PageID;
GO

CREATE PROCEDURE WebsiteData.spPage_SelectForWebsite
	@WebsiteID int
AS
	SET NOCOUNT ON;

	SELECT [Page].PageID, [Page].Title, [Page].DisplayTitle, CASE WHEN Website.HomePageID = [Page].PageID THEN 1 ELSE 0 END AS HomePage
	FROM WebsiteData.[Page]
	INNER JOIN WebsiteData.WebsitePage ON WebsitePage.PageID = [Page].PageID
	INNER JOIN WebsiteData.Website ON Website.WebsiteID = WebsitePage.WebsiteID
	WHERE WebsitePage.WebsiteID = @WebsiteID;
GO

CREATE PROCEDURE WebsiteData.spPage_Select
	@PageID int
AS
	SET NOCOUNT ON;

	SELECT [Page].PageID, [Page].Title, [Page].DisplayTitle, CASE WHEN Website.HomePageID = [Page].PageID THEN 1 ELSE 0 END AS HomePage, WebsitePage.WebsiteID
	FROM WebsiteData.[Page]
	INNER JOIN WebsiteData.WebsitePage ON WebsitePage.PageID = [Page].PageID
	INNER JOIN WebsiteData.Website ON Website.WebsiteID = WebsitePage.WebsiteID
	WHERE [Page].PageID = @PageID;
GO

CREATE PROCEDURE WebsiteData.spPosition_Update
	@SectionID int, @PageID int, @Position nvarchar(500), @DisplayTitle bit
AS
	SET NOCOUNT ON;
	
	UPDATE WebsiteData.PageSection
	SET PageSection.Position = @Position, PageSection.DisplayTitle = @DisplayTitle
	WHERE PageSection.SectionID = @SectionID AND PageSection.PageID = @PageID;
GO

CREATE PROCEDURE WebsiteData.spPosition_Insert
	@PageID int, @SectionID int, @Position nvarchar(500), @DisplayTitle bit
AS
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT PageSection.PageID FROM WebsiteData.PageSection WHERE PageID = @PageID AND SectionID = @SectionID)
		BEGIN
			INSERT INTO WebsiteData.PageSection (PageID, SectionID, Position, DisplayTitle)
			VALUES (@PageID, @SectionID, @Position, @DisplayTitle);
		END
	ELSE
		BEGIN
			EXEC WebsiteData.spPosition_Update @SectionID, @PageID, @Position, @DisplayTitle
		END
GO

CREATE PROCEDURE WebsiteData.spPosition_Delete
	@PageID int, @SectionID int
AS
	SET NOCOUNT ON;
	
	DELETE PageSection
	FROM WebsiteData.PageSection
	WHERE PageSection.PageID = @PageID AND PageSection.SectionID = @SectionID;
GO

CREATE PROCEDURE WebsiteData.spPosition_SelectForPage
	@PageID int
AS
	SET NOCOUNT ON;

	DECLARE @WebsiteID int;

	SET @WebsiteID = (SELECT TOP 1 WebsitePage.WebsiteID FROM WebsiteData.WebsitePage WHERE WebsitePage.PageID = @PageID);
	
	SELECT 
		Section.SectionID, Section.Title, 
		PageSection.Position, PageSection.DisplayTitle, 
		MAX(CASE WHEN PageSection.PageID = @PageID THEN 1 ELSE 0 END) AS IsSelected
	FROM WebsiteData.Section
	LEFT JOIN WebsiteData.PageSection ON Section.SectionID = PageSection.SectionID AND PageSection.PageID = @PageID
	WHERE Section.WebsiteID = @WebsiteID
	GROUP BY Section.SectionID, Section.Title, PageSection.Position, PageSection.DisplayTitle;
GO

CREATE PROCEDURE WebsiteData.spSection_Insert
	@PageID int, @Title nvarchar(100), @Text nvarchar(max), @Position nvarchar(500), @DisplayTitle bit
AS
	SET NOCOUNT ON;
	
	DECLARE @SectionID int, @WebsiteID int;

	SET @WebsiteID = (SELECT TOP 1 WebsitePage.WebsiteID FROM WebsiteData.WebsitePage WHERE WebsitePage.PageID = @PageID);
	
	INSERT INTO WebsiteData.Section (WebsiteID, Title, [Text])
	VALUES(@WebsiteID, @Title, @Text)
	SET @SectionID = SCOPE_IDENTITY();

	EXEC WebsiteData.spPosition_Insert @PageID, @SectionID, @Position, @DisplayTitle;
GO

CREATE PROCEDURE WebsiteData.spSection_Update
	@SectionID int, @Title nvarchar(100), @Text nvarchar(max)
AS
	SET NOCOUNT ON;
	
	UPDATE WebsiteData.Section
	SET Section.Title = @Title, Section.[Text] = @Text
	WHERE Section.SectionID = @SectionID;
GO

CREATE PROCEDURE WebsiteData.spSection_Delete
	@SectionID int
AS
	SET NOCOUNT ON;
	
	DELETE PageSection
	FROM WebsiteData.PageSection
	WHERE PageSection.SectionID = @SectionID;

	DELETE Section
	FROM WebsiteData.Section
	WHERE Section.SectionID = @SectionID;
GO

CREATE PROCEDURE WebsiteData.spSection_DeleteFromPage
	@SectionID int, @PageID int
AS
	SET NOCOUNT ON;

	DELETE PageSection
	FROM WebsiteData.PageSection
	WHERE PageSection.SectionID = @SectionID AND PageSection.PageID = @PageID;
GO

CREATE PROCEDURE WebsiteData.spSection_SelectForPageEdit
	@PageID int
AS
	SET NOCOUNT ON;

	SELECT Section.SectionID, Section.Title, PageSection.Position
	FROM WebsiteData.Section
	INNER JOIN WebsiteData.PageSection ON PageSection.SectionID = Section.SectionID
	WHERE PageSection.PageID = @PageID
	ORDER BY PageSection.Position;
GO

CREATE PROCEDURE WebsiteData.spSection_SelectForPageView
	@PageID int
AS
	SET NOCOUNT ON;

	SELECT Section.Title, Section.Text, PageSection.Position, PageSection.DisplayTitle
	FROM WebsiteData.Section
	INNER JOIN WebsiteData.PageSection ON PageSection.SectionID = Section.SectionID
	WHERE PageSection.PageID = @PageID
	ORDER BY PageSection.Position;
GO

CREATE PROCEDURE WebsiteData.spSection_Select
	@SectionID int
AS
	SET NOCOUNT ON;

	SELECT Section.SectionID, Section.Title, Section.[Text], PageSection.Position, PageSection.DisplayTitle
	FROM WebsiteData.Section
	INNER JOIN WebsiteData.PageSection ON PageSection.SectionID = Section.SectionID
	WHERE Section.SectionID = @SectionID;
GO