CREATE PROCEDURE [dbo].[Lead_SelectById]
	@Id int
AS
BEGIN
	SELECT
		l.[Name], 
		l.LastName,
		l.BirthDate,
		l.Email,
		l.Phone,
		l.[Role], 
		a.Id,
		a.[Name],
		a.CurrencyType
	FROM dbo.[Lead] l inner join dbo.[Account] a ON l.Id = a.LeadId
	WHERE l.Id = @Id
END