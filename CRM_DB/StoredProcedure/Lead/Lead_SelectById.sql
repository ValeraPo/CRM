CREATE PROCEDURE [dbo].[Lead_SelectById]
	@Id int
AS
BEGIN
	SELECT
		l.Id,
		l.[Name], 
		l.LastName,
		l.BirthDate,
		l.Email,
		l.Phone,
		l.[Role], 
		l.IsBanned,
		l.[Password],
		a.Id,
		a.[Name],
		a.CurrencyType
	FROM dbo.[Lead] l inner join dbo.[Account] a ON a.LeadId = l.Id
	WHERE l.Id = @Id and a.IsBlocked = 0
END