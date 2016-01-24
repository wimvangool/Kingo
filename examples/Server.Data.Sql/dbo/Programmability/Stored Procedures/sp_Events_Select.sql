CREATE PROCEDURE [dbo].[sp_Events_Select]
(
	@Key UNIQUEIDENTIFIER
)
AS
BEGIN
    -- First, we look for the latest snapshot of the aggregate.
    DECLARE @Version INT
	SELECT  @Version = MAX([Version])
	FROM	[dbo].[WriteModel_Snapshots]
	WHERE	[Key] = @Key;

	-- Then, as a first result set, we select all events with a higher version than the latest snapshot's version.
	SELECT	[Version],
			[Value],
			[Type]
	FROM	[dbo].[WriteModel_Events]
	WHERE	[Key] = @Key
	AND		
	(		 
			@Version IS NULL OR @Version < [Version]
	);

	-- Finally, as the second result set, we select the snapshot, if any.
	SELECT	[Version],
			[Value],
			[Type]
	FROM	[dbo].[WriteModel_Snapshots]
	WHERE	[Key] = @Key
	AND		[Version] = @Version;
		    
END
