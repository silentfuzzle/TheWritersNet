CREATE PROCEDURE WebsiteData.spUser_Insert
	@LoginID nvarchar(50)
AS
	SET NOCOUNT ON;

	INSERT INTO WebsiteData.[User] ([LoginID])
	VALUES(@LoginID)
GO

CREATE PROCEDURE WebsiteData.spUser_Select
	@LoginID nvarchar(50)
AS
	SET NOCOUNT ON;

	SELECT * FROM WebsiteData.[User];
GO

CREATE PROCEDURE WebsiteData.spPermission_Insert
	@WebsiteID int, @UserID int, @Ability int
AS
	SET NOCOUNT ON;

	INSERT INTO WebsiteData.WebsitePermission (WebsiteID, UserID, Ability)
	VALUES(@WebsiteID, @UserID, @Ability)
GO

CREATE PROCEDURE WebsiteData.spWebsite_Insert
	@Title nvarchar(100), @Owner int, @Visibility int, @Description nvarchar(1000)
AS
	SET NOCOUNT ON;

	DECLARE @WebsiteID int;

	INSERT INTO WebsiteData.Website (Title, HomePage, [Owner], Visibility, [Description])
	VALUES(@Title, NULL, @Owner, @Visibility, @Description)
	SET @WebsiteID = SCOPE_IDENTITY();

	EXEC WebsiteData.spPermission_Insert @WebsiteID, @Owner, 1;
GO

CREATE PROCEDURE WebsiteData.spWebsite_Select
	@UserID int
AS
	SET NOCOUNT ON;

	SELECT Website.WebsiteID, Website.Title, Website.HomePage, Website.Visibility, Website.[Description], Permission.Ability
	FROM WebsiteData.Website AS Website
	INNER JOIN WebsiteData.WebsitePermission AS Permission ON Website.WebsiteID = Permission.WebsiteID
	WHERE Permission.UserID = @UserID;
GO