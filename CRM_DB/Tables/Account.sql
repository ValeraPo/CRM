CREATE TABLE [dbo].[Account]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(20) NOT NULL, 
    [CurrencyType] INT NOT NULL, 
    [LeadId] INT NOT NULL,
    [LockDate] date NULL,
    [IsBlocked] BIT NOT NULL,
    CONSTRAINT [FK_LeadId_ToLead] FOREIGN KEY ([LeadId]) REFERENCES [dbo].[Lead]([Id])
)
