CREATE PROCEDURE [dbo].[Invoice_SelectAll]
AS
BEGIN
	SELECT 
		Id,
		AccountId,
		Amount,
		Status
	FROM Invoice
END
