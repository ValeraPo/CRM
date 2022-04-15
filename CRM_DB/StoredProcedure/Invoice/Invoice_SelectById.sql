CREATE PROCEDURE [dbo].[Invoice_SelectById]
	@Id nvarchar(30)
AS
BEGIN
	SELECT 
		inv.Id,
		Amount,
		Status,
		acc.Id,
		acc.CurrencyType,
		acc.LeadId
	FROM Invoice inv
	INNER JOIN Account acc	ON acc.Id = inv.AccountId
	WHERE inv.Id = @Id
END
