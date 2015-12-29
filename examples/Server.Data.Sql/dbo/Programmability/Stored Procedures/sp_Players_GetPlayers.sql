CREATE PROCEDURE [dbo].[sp_Players_GetPlayers]
AS
BEGIN
	SELECT  [Key], [Name]
	FROM	[dbo].[Players];
END
