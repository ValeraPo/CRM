CREATE PROCEDURE [dbo].[Account_SelectAll]

AS
BEGIN
	select
		[Name], 
		CurrencyType,
		LeadId,
		LockDate,
		IsBlocked
	from dbo.[Account]
END