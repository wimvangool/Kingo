CREATE PROCEDURE [dbo].[sp_Snapshots_Select]
(
	@Key UNIQUEIDENTIFIER
)
AS
BEGIN
	SELECT	[Value],
			[Type]
	FROM	[dbo].[WriteModel_Snapshots]
	WHERE	[Key] = @Key;
END
