CREATE PROCEDURE [dbo].[sp_Players_SelectByKey]
	@Key uniqueidentifier
AS
BEGIN
	SELECT	[Value]
	FROM	[dbo].[Players]
	WHERE	[Key] = @Key
END
