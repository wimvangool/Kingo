CREATE PROCEDURE [dbo].[sp_Games_GetActiveGames]
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
	FROM	[dbo].[ActiveGames] games
	JOIN	[dbo].[Players] white
		ON	games.WhitePlayerKey = white.[Key]
	JOIN	[dbo].[Players] black
		ON	games.BlackPlayerKey = black.[Key]
	WHERE	@PlayerKey = games.[WhitePlayerKey]
	OR		@PlayerKey = games.[BlackPlayerKey]
END
