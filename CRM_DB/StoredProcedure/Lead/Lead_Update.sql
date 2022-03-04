CREATE PROCEDURE [dbo].[Lead_Update]
	@Id int,
	@Name varchar(30),
	@LastName varchar(30),
	@BirthDate date,
	@Email varchar(30),
	@Phone varchar(30),
	@Role int
	
AS
BEGIN 
	update dbo.[Lead]
	SET 
		[Name] = @Name,
		LastName = @LastName,
		BirthDate = @BirthDate,
		Phone = @Phone
	WHERE Id = @Id
END
