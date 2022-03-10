CREATE PROCEDURE [dbo].[Account_Insert]
	@Name varchar(20),
	@CurrencyType varchar(30),
	@LeadId int
	
AS
BEGIN 
	insert into dbo.[Account]
		([Name], 
		CurrencyType,
		LeadId)
	values
		(@Name,
		@CurrencyType,
		@LeadId)
	select scope_identity()
END
