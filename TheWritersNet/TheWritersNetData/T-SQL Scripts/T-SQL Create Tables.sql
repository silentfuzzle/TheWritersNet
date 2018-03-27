USE TheWritersNetDB;
GO
CREATE SCHEMA WebsiteData;
GO

USE TheWritersNetDB;
CREATE TABLE WebsiteData.[User]
(
	UserID int NOT NULL IDENTITY PRIMARY KEY,
	LoginID nvarchar(50) NOT NULL,
	UserName nvarchar(50) NOT NULL,
	[Description] nvarchar(1000)
);

USE TheWritersNetDB;
CREATE TABLE WebsiteData.UserSocialMedia
(
	UserID int NOT NULL FOREIGN KEY REFERENCES WebsiteData.[User] (UserID),
	WebsiteName nvarchar(500) NOT NULL,
	AccountAddress nvarchar(500) NOT NULL
);

USE TheWritersNetDB;
CREATE TABLE WebsiteData.Visibility
(
	VisibilityID int NOT NULL IDENTITY PRIMARY KEY,
	[Name] nvarchar(50) NOT NULL
);

USE TheWritersNetDB;
INSERT INTO WebsiteData.Visibility ([Name])
VALUES ('Public'), ('Private');

USE TheWritersNetDB;
CREATE TABLE WebsiteData.[Page]
(
	PageID int NOT NULL IDENTITY PRIMARY KEY,
	Title nvarchar(100) NOT NULL
);

USE TheWritersNetDB;
CREATE TABLE WebsiteData.Website 
(
	WebsiteID int NOT NULL IDENTITY PRIMARY KEY,
	Title nvarchar(100) NOT NULL,
	HomePageID int FOREIGN KEY REFERENCES WebsiteData.[Page] (PageID),
	OwnerID int NOT NULL REFERENCES WebsiteData.[User] (UserID),
	VisibilityID int NOT NULL REFERENCES WebsiteData.Visibility (VisibilityID),
	[Description] nvarchar(1000)
);

USE TheWritersNetDB;
CREATE TABLE WebsiteData.Section
(
	SectionID int NOT NULL IDENTITY PRIMARY KEY,
	WebsiteID int NOT NULL FOREIGN KEY REFERENCES WebsiteData.Website (WebsiteID),
	Title nvarchar(100) NOT NULL,
	Text nvarchar(MAX) NOT NULL
);

USE TheWritersNetDB;
CREATE TABLE WebsiteData.PageSection
(
	PageID int NOT NULL FOREIGN KEY REFERENCES WebsiteData.[Page] (PageID),
	SectionID int NOT NULL FOREIGN KEY REFERENCES WebsiteData.Section (SectionID),
	Position nvarchar(500) NOT NULL
);

USE TheWritersNetDB;
CREATE TABLE WebsiteData.WebsitePage
(
	WebsiteID int NOT NULL FOREIGN KEY REFERENCES WebsiteData.Website (WebsiteID),
	PageID int NOT NULL FOREIGN KEY REFERENCES WebsiteData.[Page] (PageID)
);

Use TheWritersNetDB;
CREATE TABLE WebsiteData.Ability
(
	AbilityID int NOT NULL IDENTITY PRIMARY KEY,
	[Name] nvarchar(50) NOT NULL
);

USE TheWritersNetDB;
INSERT INTO WebsiteData.Ability ([Name])
VALUES ('Owner'), ('Admin'), ('Writer'), ('Viewer');

USE TheWritersNetDB;
CREATE TABLE WebsiteData.WebsitePermission
(
	WebsiteID int NOT NULL REFERENCES WebsiteData.Website (WebsiteID),
	UserID int NOT NULL REFERENCES WebsiteData.[User] (UserID),
	AbilityID int NOT NULL REFERENCES WebsiteData.Ability (AbilityID)
);

USE TheWritersNetDB;
CREATE TABLE WebsiteData.Tag
(
	TagID int NOT NULL IDENTITY PRIMARY KEY,
	[Text] nvarchar(100) NOT NULL,
	NumWebsites int NOT NULL,
	NumUsers int NOT NULL
);

USE TheWritersNetDB;
CREATE TABLE WebsiteData.TagWebsite
(
	TagID int NOT NULL REFERENCES WebsiteData.Tag (TagID),
	WebsiteID int NOT NULL REFERENCES WebsiteData.Website (WebsiteID)
);

USE TheWritersNetDB;
CREATE TABLE WebsiteData.TagUser
(
	TagID int NOT NULL REFERENCES WebsiteData.Tag (TagID),
	UserID int NOT NULL REFERENCES WebsiteData.[User] (UserID)
);