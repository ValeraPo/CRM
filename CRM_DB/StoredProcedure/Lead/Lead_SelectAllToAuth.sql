CREATE PROCEDURE [dbo].[Lead_SelectAllToAuth]
AS
BEGIN
	select
		Id,
		Email,
		[Password] as HashPassword,
		[Role]
	from dbo.[Lead]
	where IsBanned = 0 and Id > 4004005
END