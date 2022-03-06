CREATE PROCEDURE [dbo].[Lead_Insert]
	@Name varchar(30),
	@LastName varchar(30),
	@BirthDate date,
	@Email varchar(30),
	@Phone varchar(30),
	@Password varchar(150),
	@Role int,
	@IsBanned BIT
	
AS
BEGIN 
	insert into dbo.[Lead]
		([Name], 
		LastName,
		BirthDate,
		Email,
		Phone,
		[Password],
		[Role],
		IsBanned)
	values
		(@Name,
		@LastName,
		@BirthDate,
		@Email,
		@Phone,
		@Password,
		@Role,
		@IsBanned)
	select scope_identity()
END
