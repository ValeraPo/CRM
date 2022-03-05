CREATE PROCEDURE [dbo].[Account_Update]
	@Id int,
	@Name varchar(20)
	
AS
BEGIN 
	update dbo.[Account]
	SET 
		[Name] = @Name
	WHERE Id = @Id
END
