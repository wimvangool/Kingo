CREATE PROCEDURE [dbo].[sp_Players_HasBeenRegistered]
	@Name nvarchar(20)
AS
BEGIN
	SELECT	COUNT([Key]) AS HasBeenRegistered
	FROM	[dbo].[Players]
	WHERE	Name = @Name
END
