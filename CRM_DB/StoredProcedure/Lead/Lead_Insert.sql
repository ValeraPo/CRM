CREATE PROCEDURE [dbo].[Lead_Insert]
	@Id int,
	@Name varchar(30),
	@LastName varchar(30),
	@DateBirth date,
	@Email varchar(30),
	@Phone varchar(30),
	@Password varchar(30),
	@Role int
	
AS
BEGIN 
	update dbo.[Lead]
	SET 
		[Name] = @Name,
		LastName = @LastName,
		DateBirth = @DateBirth,
		Email = @Email,
		Phone = @Phone,
		[Password] = @Password,
		[Role] = @Role
	WHERE Id = @Id
END
