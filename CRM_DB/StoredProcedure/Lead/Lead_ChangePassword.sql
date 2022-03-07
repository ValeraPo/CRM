CREATE PROCEDURE [dbo].[Lead_ChangePassword]
	@Id int,
	@Password varchar(150)	
AS
BEGIN 
	update dbo.[Lead]
	SET 
		[Password] = @Password
	WHERE Id = @Id
END
