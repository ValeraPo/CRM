CREATE PROCEDURE [dbo].[Lead_ChangeRole]
	@Id int,
	@Role int
	
AS
BEGIN 
	update dbo.[Lead]
	SET 
		[Role] = @Role
	WHERE Id = @Id
END
