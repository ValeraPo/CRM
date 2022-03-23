CREATE TABLE [dbo].[Lead]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] varchar(30) NOT NULL,
	[LastName] varchar(30) NOT NULL,
	[BirthDate] date NOT NULL,
	[Email] varchar(30) NOT NULL,
	[Phone] varchar(20) NULL,
	[Password] varchar(70) NOT NULL,
	[Role] TINYINT NOT NULL, 
    [IsBanned] BIT NOT NULL DEFAULT 0, 
    [City] VARCHAR(30) NULL, 
    CONSTRAINT AK_Email UNIQUE(Email),
)