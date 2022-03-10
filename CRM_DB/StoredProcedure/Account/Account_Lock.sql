CREATE PROCEDURE [dbo].[Account_Lock]
	@Id int,
	@IsBlocked bit
	
AS
BEGIN 
	update dbo.[Account]
	SET 
		IsBlocked = @IsBlocked,
		LockDate = GETDATE()
	WHERE Id = @Id
END