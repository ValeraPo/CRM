CREATE PROCEDURE [dbo].[Invoice_Insert]
	@Id nvarchar(30),
	@AccountId int,
	@Amount decimal(9,2),
	@Status tinyint
AS
BEGIN
	INSERT INTO dbo.Invoice
		(Id,
		AccountId,
		Amount,
		Status)
	VALUES
		(@Id,
		@AccountId,
		@Amount,
		@Status)
END
