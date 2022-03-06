CREATE PROCEDURE [dbo].[Account_SelectById]
	@Id int
AS
BEGIN
	select
		[Name], 
		CurrencyType,
		LeadId,
		LockDate,
		IsBlocked
	from dbo.[Account]
	where Id =@Id
END