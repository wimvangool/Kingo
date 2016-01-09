CREATE PROCEDURE [dbo].[sp_Games_DeleteActiveGame]
(
	@Key UNIQUEIDENTIFIER
)
AS
BEGIN
	DELETE FROM [dbo].[ActiveGames]
	WHERE [GameKey] = @Key
END
