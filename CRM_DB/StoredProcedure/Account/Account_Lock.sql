CREATE PROCEDURE [dbo].[Account_Lock]
	@Id int,
	@IsBlocked bit,
	@LockDate date
	
AS
BEGIN 
	update dbo.[Account]
	SET 
		IsBlocked = @IsBlocked,
		LockDate = @LockDate
	WHERE Id = @Id
END