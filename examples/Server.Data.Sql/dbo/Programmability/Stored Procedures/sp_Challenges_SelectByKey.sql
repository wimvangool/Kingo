CREATE PROCEDURE [dbo].[sp_Challenges_SelectByKey]
	@Key UNIQUEIDENTIFIER
AS
BEGIN
	SELECT	[Value]
	FROM	[dbo].[Challenges]
	WHERE	[Key] = @Key
END
