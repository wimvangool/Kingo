CREATE PROCEDURE [dbo].[sp_Games_InsertActiveGame]
(
	@GameKey UNIQUEIDENTIFIER,
	@WhitePlayerKey UNIQUEIDENTIFIER,
	@BlackPlayerKey UNIQUEIDENTIFIER
)
AS
BEGIN
	INSERT INTO [dbo].[ActiveGames] ([GameKey], [WhitePlayerKey], [BlackPlayerKey])
	VALUES (@GameKey, @WhitePlayerKey, @BlackPlayerKey)
END
