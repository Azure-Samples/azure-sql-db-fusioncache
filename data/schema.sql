DROP TABLE IF EXISTS [dbo].[Companies];
CREATE TABLE [dbo].[Companies] (
	[Id] [int] IDENTITY(1,1) NOT NULL CONSTRAINT pk__Companies PRIMARY KEY,
	[Name] [varchar](200) NOT NULL,
	[CatchPhrase] [varchar](200) NULL,
	[FoundationYear] [int] NOT NULL,
	[IsFullRemote] [bit] NOT NULL
)
GO

DROP TABLE IF EXISTS [dbo].[Products];
CREATE TABLE [dbo].[Products] (
	[Id] [int] IDENTITY(1,1) NOT NULL CONSTRAINT pk__Products PRIMARY KEY,
	[Name] [varchar](200) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastUpdatedAt] [datetime] NOT NULL
)
GO
