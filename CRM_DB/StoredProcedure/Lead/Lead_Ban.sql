CREATE PROCEDURE [dbo].[Lead_Ban]
	@Id int,
	@IsBanned bit
	
AS
BEGIN 
	update dbo.[Lead]
	SET 
		IsBanned = @IsBanned
	WHERE Id = @Id
END
