CREATE PROCEDURE [dbo].[sp_Players_Insert]
(
	@Key UNIQUEIDENTIFIER,
	@Name NVARCHAR(20)
)
AS
BEGIN
	INSERT INTO [dbo].[ReadModel_Players]
	(
		[Key],
		[Name]
	)
	VALUES
	(
		@Key,
		@Name
	);
END