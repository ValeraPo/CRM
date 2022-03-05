﻿CREATE PROCEDURE [dbo].[Lead_Update]
	@Id int,
	@Name varchar(30),
	@LastName varchar(30),
	@BirthDate date,
	@Phone varchar(30),
	@Role int
	
AS
BEGIN 
	update dbo.[Lead]
	SET 
		[Name] = @Name,
		LastName = @LastName,
		BirthDate = @BirthDate,
		Phone = @Phone,
		[Role] = @Role
	WHERE Id = @Id
END
