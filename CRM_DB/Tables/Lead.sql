CREATE TABLE [dbo].[Lead]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] varchar(30) NOT NULL,
	[LastName] varchar(30) NOT NULL,
	[DateBirth] date NOT NULL,
	[Email] varchar(30) NOT NULL,
	[Phone] varchar(20) NULL,
	[Password] varchar(150) NOT NULL,
	[Role] INT NOT NULL, 
    CONSTRAINT AK_Email UNIQUE(Email),
)