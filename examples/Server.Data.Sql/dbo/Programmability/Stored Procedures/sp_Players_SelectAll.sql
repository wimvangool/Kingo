CREATE PROCEDURE [dbo].[sp_Players_SelectAll]
AS
BEGIN
	SELECT  [Key],
		    [Name]
	FROM	[dbo].[ReadModel_Players];	
END
