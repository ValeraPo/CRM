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
	insert into dbo.[Lead]
		([Name], 
		LastName,
		DateBirth,
		Email,
		Phone,
		[Password],
		[Role])
	values
		(@Name,
		@DateBirth,
		@Email,
		@Phone,
		@Password,
		@Role)
SELECT SCOPE_IDENTITY()
END
