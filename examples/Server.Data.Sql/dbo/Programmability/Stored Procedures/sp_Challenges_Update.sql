CREATE PROCEDURE [dbo].[sp_Challenges_Update]
(
	@Key UNIQUEIDENTIFIER,
	@OldVersion INT,
	@NewVersion INT,
	@Value NVARCHAR(MAX)
)
AS
BEGIN
	UPDATE	[dbo].[Challenges]
	SET		[Version] = @NewVersion,
			[Value] = @Value
	WHERE	[Key] = @Key
	AND		[Version] = @OldVersion
END
