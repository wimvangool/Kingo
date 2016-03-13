CREATE PROCEDURE [dbo].[sp_Events_Insert]
(
	@Key UNIQUEIDENTIFIER,
	@Version INT,
	@Value NVARCHAR(MAX),
	@Type NVARCHAR(256),
	@OriginalVersion INT,
	@Events dbo.Events READONLY
)
AS
BEGIN
    -- First, all events are inserted.
	-- The insert only takes place if its a new stream (@OriginalVersion IS NULL) or
	-- if the @OriginalVersion matches the expected version, meaning no concurrency-conflict has occurred.
	INSERT INTO [dbo].[WriteModel_Events]
	(
		[Key],
		[Version],
		[Value],
		[Type]
	)
	SELECT	@Key,
			[Version],
			[Value],
			[Type]
	FROM	@Events
	WHERE	@OriginalVersion IS NULL
	OR		@OriginalVersion = (SELECT MAX([Version]) FROM [dbo].[WriteModel_Events] WHERE [Key] = @Key)

	-- Second, the snapshot is inserted, if specified.
	-- Note that we do not insert the snapshot if no events were inserted.
	IF @Value IS NOT NULL AND @@ROWCOUNT > 0
	BEGIN
		INSERT INTO [dbo].[WriteModel_Snapshots]
		(
			[Key],
			[Version],
			[Value],
			[Type]
		)
		VALUES
		(
			@Key,
			@Version,
			@Value,
			@Type
		);
	END	
END
