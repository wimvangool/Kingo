CREATE PROCEDURE [dbo].[sp_Players_GetPlayers]
AS
BEGIN
	SELECT  [Name]
	FROM	[dbo].[Players];
END
