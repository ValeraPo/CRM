﻿CREATE PROCEDURE [dbo].[Lead_SelectByEmail]
	@Email varchar(30)
AS
BEGIN
	select
		[Name], 
		LastName,
		BirthDate,
		Email,
		Phone,
		[Password],
		[Role]
	from dbo.[Lead]
	where Email = @Email
END