CREATE TABLE [Companies] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[CatchPhrase] [varchar](200) NULL,
	[FoundationYear] [int] NOT NULL,
	[IsFullRemote] [bit] NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [Products] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastUpdatedAt] [datetime] NOT NULL
) ON [PRIMARY]
GO
