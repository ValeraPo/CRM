CREATE PROCEDURE [dbo].[Account_SelectById]
	@Id int
AS
BEGIN
	SELECT
		a.Id,
		a.[Name], 
		a.CurrencyType,
		a.LeadId,
		a.LockDate,
		a.IsBlocked,
		l.Id,
		l.[Name], 
		l.LastName,
		l.BirthDate,
		l.Email,
		l.Phone,
		l.[Role], 
		l.IsBanned
	from dbo.[Account] a inner join dbo.[Lead] l ON a.LeadId = l.Id
	where a.Id = @Id
END