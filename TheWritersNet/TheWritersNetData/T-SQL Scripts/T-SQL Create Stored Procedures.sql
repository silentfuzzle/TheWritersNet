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
	@WebsiteID int, @UserID int, @AbilityID int
AS
	SET NOCOUNT ON;

	INSERT INTO WebsiteData.WebsitePermission (WebsiteID, UserID, Ability)
	VALUES(@WebsiteID, @UserID, @AbilityID)
GO

CREATE PROCEDURE WebsiteData.spPermission_Select
	@WebsiteID int
AS
	SET NOCOUNT ON;

	SELECT [User].UserID, [User].UserName, WebsitePermission.Ability AS AbilityID, Ability.[Name] AS Ability
	FROM WebsiteData.[User]
	INNER JOIN WebsiteData.WebsitePermission ON [User].UserID = WebsitePermission.UserID
	INNER JOIN WebsiteData.Ability ON Ability.AbilityID = WebsitePermission.Ability
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

CREATE PROCEDURE WebsiteData.spWebsite_Insert
	@Title nvarchar(100), @LoginID nvarchar(50), @Visibility int, @Description nvarchar(1000)
AS
	SET NOCOUNT ON;

	DECLARE @WebsiteID int, @UserID int;

	EXEC WebsiteData.spUser_SelectID @LoginID, @UserID OUTPUT;

	INSERT INTO WebsiteData.Website (Title, HomePage, [Owner], Visibility, [Description])
	VALUES(@Title, NULL, @UserID, @Visibility, @Description)
	SET @WebsiteID = SCOPE_IDENTITY();

	EXEC WebsiteData.spPermission_Insert @WebsiteID, @UserID, 1;
GO

CREATE PROCEDURE WebsiteData.spWebsite_Update
	@WebsiteID int, @Title nvarchar(100), @VisibilityID int, @Description nvarchar(1000)
AS
	SET NOCOUNT ON;

	UPDATE WebsiteData.Website
	SET Website.Title = @Title, Website.Visibility = @VisibilityID, Website.[Description] = @Description
	WHERE Website.WebsiteID = @WebsiteID;
GO

CREATE PROCEDURE WebsiteData.spWebsite_UpdateHomePage
	@WebsiteID int, @PageID int, @HomePage bit
AS
	SET NOCOUNT ON;

	DECLARE @HomePageID int;

	SET @HomePageID = (SELECT TOP 1 Website.HomePage FROM WebsiteData.Website WHERE WebsiteID = @WebsiteID);

	IF (@HomePageID != @PageID AND @HomePage = 1)
		BEGIN
			UPDATE WebsiteData.Website
			SET Website.HomePage = @PageID
			WHERE Website.WebsiteID = @WebsiteID;
		END
	ELSE
		BEGIN
			IF (@HomePageID = @PageID AND @HomePage = 0)
				BEGIN
					UPDATE WebsiteData.Website
					SET Website.HomePage = (SELECT TOP 1 WebsitePage.PageID FROM WebsiteData.WebsitePage WHERE WebsiteID = @WebsiteID AND PageID != @PageID)
					WHERE Website.WebsiteID = @WebsiteID;
				END
		END
GO

CREATE PROCEDURE WebsiteData.spWebsite_Delete
	@WebsiteID int
AS
	SET NOCOUNT ON;

	DELETE Section
	FROM WebsiteData.Section
	INNER JOIN WebsiteData.PageSection ON PageSection.SectionID = Section.SectionID
	INNER JOIN WebsiteData.WebsitePage ON WebsitePage.PageID = PageSection.PageID
	WHERE WebsitePage.WebsiteID = @WebsiteID;

	DELETE PageSection
	FROM WebsiteData.PageSection
	INNER JOIN WebsiteData.WebsitePage ON WebsitePage.PageID = PageSection.PageID
	WHERE WebsitePage.WebsiteID = @WebsiteID;

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

	SELECT Website.WebsiteID, Website.Title, Website.Visibility AS VisibilityID, Website.[Description]
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
		Website.Visibility AS VisibilityID, 
		Visibility.[Name] AS Visibility, 
		Website.[Description], 
		Permission.Ability AS AbilityID, 
		Ability.[Name] AS Ability
	FROM WebsiteData.Website AS Website
	INNER JOIN WebsiteData.WebsitePermission AS Permission ON Website.WebsiteID = Permission.WebsiteID
	INNER JOIN WebsiteData.Ability AS Ability ON Permission.Ability = Ability.AbilityID
	INNER JOIN WebsiteData.Visibility AS Visibility ON Website.Visibility = Visibility.VisibilityID
	WHERE Permission.UserID = @UserID;
GO

CREATE PROCEDURE WebsiteData.spWebsite_SelectPublic
AS
	SET NOCOUNT ON;

	SELECT Website.WebsiteID, Website.Title, Website.[Owner] AS OwnerID, [User].UserName AS OwnerName, Website.[Description]
	FROM WebsiteData.Website AS Website
	INNER JOIN WebsiteData.[User] AS [User] ON [User].UserID = Website.[Owner]
	WHERE Website.Visibility = 1;
GO

CREATE PROCEDURE WebsiteData.spPage_Insert
	@WebsiteID int, @Title nvarchar(100), @HomePage bit
AS
	SET NOCOUNT ON;

	DECLARE @PageID int;
	
	INSERT INTO WebsiteData.[Page] (Title)
	VALUES(@Title)
	SET @PageID = SCOPE_IDENTITY();

	INSERT INTO WebsiteData.WebsitePage (WebsiteID, PageID)
	VALUES (@WebsiteID, @PageID);

	EXEC WebsiteData.spWebsite_UpdateHomePage @WebsiteID, @PageID, @HomePage; 
GO

CREATE PROCEDURE WebsiteData.spPage_Update
	@PageID int, @Title nvarchar(100)
AS
	SET NOCOUNT ON;
	
	UPDATE WebsiteData.[Page]
	SET [Page].Title = @Title
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

	DELETE Section
	FROM WebsiteData.Section
	LEFT JOIN WebsiteData.PageSection ON PageSection.SectionID = Section.SectionID
	WHERE PageSection.SectionID IS NULL;
	
	DELETE WebsitePage
	FROM WebsiteData.WebsitePage
	WHERE WebsitePage.PageID = @PageID;
GO

CREATE PROCEDURE WebsiteData.spPage_Select
	@WebsiteID int
AS
	SET NOCOUNT ON;

	SELECT [Page].PageID, [Page].Title, CASE WHEN Website.HomePage = [Page].PageID THEN 1 ELSE 0 END AS HomePage
	FROM WebsiteData.[Page]
	INNER JOIN WebsiteData.WebsitePage ON WebsitePage.PageID = [Page].PageID
	INNER JOIN WebsiteData.Website ON Website.WebsiteID = WebsitePage.WebsiteID
	WHERE WebsitePage.WebsiteID = 1;
GO

CREATE PROCEDURE WebsiteData.spSection_Select
	@PageID int
AS
	SET NOCOUNT ON;

	SELECT Section.SectionID, Section.Title, PageSection.Position
	FROM WebsiteData.Section
	INNER JOIN WebsiteData.PageSection ON PageSection.SectionID = Section.SectionID
	WHERE PageSection.PageID = @PageID
	ORDER BY PageSection.Position;
GO