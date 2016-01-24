CREATE PROCEDURE [dbo].[sp_Players_HasBeenRegistered]
(
	@Name NVARCHAR(20)
)
AS
BEGIN
	SELECT	COUNT([Key]) AS HasBeenRegistered
	FROM	[dbo].[ReadModel_Players]
	WHERE	Name = @Name	
END
