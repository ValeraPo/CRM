CREATE PROCEDURE [dbo].[Lead_SelectAll]
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
END