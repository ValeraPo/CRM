CREATE PROCEDURE [dbo].[Account_SelectByLead]
	@LeadId int
AS
BEGIN
	SELECT
		Id,
		[Name], 
		CurrencyType,
		LeadId
	from dbo.[Account]
	where LeadId = @LeadId
END