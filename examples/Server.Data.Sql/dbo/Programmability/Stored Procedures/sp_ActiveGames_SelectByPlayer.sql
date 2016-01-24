CREATE PROCEDURE [dbo].[sp_ActiveGames_SelectByPlayer]
(
	@PlayerKey UNIQUEIDENTIFIER
)
AS
BEGIN
	SELECT	games.[GameKey],
			white.[Key] AS WhitePlayerKey,
			white.[Name] AS WhitePlayerName,
			black.[Key] AS BlackPlayerKey,
			black.[Name] AS BlackPlayerName
	FROM	[dbo].[ReadModel_ActiveGames] games
	JOIN	[dbo].[ReadModel_Players] white
		ON	games.WhitePlayerKey = white.[Key]
	JOIN	[dbo].[ReadModel_Players] black
		ON	games.BlackPlayerKey = black.[Key]
	WHERE	@PlayerKey = games.[WhitePlayerKey]
	OR		@PlayerKey = games.[BlackPlayerKey]
END
