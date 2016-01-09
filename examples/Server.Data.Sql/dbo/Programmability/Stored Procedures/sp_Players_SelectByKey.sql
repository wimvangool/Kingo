CREATE PROCEDURE [dbo].[sp_Players_SelectByKey]
(
	@Key UNIQUEIDENTIFIER
)
AS
BEGIN
	SELECT	[Value]
	FROM	[dbo].[Players]
	WHERE	[Key] = @Key
END
