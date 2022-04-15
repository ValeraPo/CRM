CREATE PROCEDURE [dbo].[Invoice_UpdateStatus]
	@Id nvarchar(30),
	@Status tinyint
AS
BEGIN
	UPDATE Invoice 
	SET
		Status = @Status
	WHERE
		Id = @Id
END
