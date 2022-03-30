CREATE PROCEDURE [dbo].[Lead_SelectAllToAuth]
AS
BEGIN
	select
		Id,
		Email,
		[Password],
		[Role]
	from dbo.[Lead]
	where IsBanned = 0
END