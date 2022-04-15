CREATE TABLE [dbo].[Invoice]
(
	[Id] NVARCHAR(30) NOT NULL PRIMARY KEY, 
    [AccountId] INT NOT NULL,
	[Amount] DECIMAL(9, 2) NOT NULL, 
    [Status] TINYINT NOT NULL, 
    CONSTRAINT [FK_AccountId_ToAccount] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account]([Id])
)
