﻿CREATE PROCEDURE [dbo].[Lead_SelectById]
	@Id int
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
	where Id =@Id
END