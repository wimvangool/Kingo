CREATE PROCEDURE [dbo].[sp_Snapshots_Update]
(
	@Key UNIQUEIDENTIFIER,
	@Version INT,
	@Value NVARCHAR(MAX),
	@Type NVARCHAR(256),
	@OriginalVersion INT
)
AS
BEGIN
	UPDATE  [dbo].[WriteModel_Snapshots]
	SET		[Version] = @Version,
			[Value] = @Value,
			[Type] = @Type
	WHERE	[Key] = @Key
	AND		[Version] = @OriginalVersion;    		
END
