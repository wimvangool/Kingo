CREATE PROCEDURE [dbo].[sp_ActiveGames_Delete]
(
	@Key UNIQUEIDENTIFIER
)
AS
BEGIN
	DELETE FROM [dbo].[ReadModel_ActiveGames]
	WHERE		[GameKey] = @Key
END
