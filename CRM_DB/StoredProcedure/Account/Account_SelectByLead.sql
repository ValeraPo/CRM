CREATE PROCEDURE [dbo].[Account_SelectByLead]
	@LeadId int
AS
BEGIN
	select
		[Name], 
		CurrencyType,
		LeadId,
		LockDate,
		IsBlocked
	from dbo.[Account]
	where LeadId = @LeadId
END