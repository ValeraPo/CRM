CREATE PROCEDURE [dbo].[Lead_SelectAllEmails]
AS
BEGIN
	select
		Email
	from dbo.[Lead]
	where IsBanned = 0
END