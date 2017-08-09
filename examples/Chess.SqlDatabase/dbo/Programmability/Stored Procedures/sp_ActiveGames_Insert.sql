CREATE PROCEDURE [dbo].[sp_ActiveGames_Insert]
(
	@GameKey UNIQUEIDENTIFIER,
	@WhitePlayerKey UNIQUEIDENTIFIER,
	@BlackPlayerKey UNIQUEIDENTIFIER
)
AS
BEGIN
	INSERT INTO [dbo].[ReadModel_ActiveGames]
	(
		[GameKey],
		[WhitePlayerKey],
		[BlackPlayerKey]
	)
	VALUES (
		@GameKey,
		@WhitePlayerKey,
		@BlackPlayerKey
	)	
END
