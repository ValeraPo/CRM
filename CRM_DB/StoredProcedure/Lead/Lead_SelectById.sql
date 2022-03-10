CREATE PROCEDURE [dbo].[Lead_SelectById]
	@Id int
AS
BEGIN
	select
		[Name], 
		LastName,
		BirthDate,
		Email,
		Phone,
		[Password],
		[Role]
	from dbo.[Lead] l
	inner join dbo.Account a on l.Id = a.LeadId
	where Id = @Id

END