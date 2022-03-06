CREATE PROCEDURE [dbo].[Account_Insert]
	@Name varchar(20),
	@CurrencyType varchar(30),
	@LeadId int,
	@LockDate date,
	@IsBlocked bit
	
AS
BEGIN 
	insert into dbo.[Account]
		([Name], 
		CurrencyType,
		LeadId,
		LockDate,
		IsBlocked)
	values
		(@Name,
		@CurrencyType,
		@LeadId,
		@LockDate,
		@IsBlocked)
	select scope_identity()
END
