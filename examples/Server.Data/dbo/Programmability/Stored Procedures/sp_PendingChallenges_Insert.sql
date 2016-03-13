CREATE PROCEDURE [dbo].[sp_PendingChallenges_Insert]
(
	@ChallengeKey UNIQUEIDENTIFIER,
	@SenderKey UNIQUEIDENTIFIER,
	@ReceiverKey UNIQUEIDENTIFIER	
)
AS
BEGIN
	INSERT INTO [dbo].[ReadModel_PendingChallenges]
	(
		[ChallengeKey],
		[ReceiverKey],
		[SenderName]
	)
	SELECT	@ChallengeKey,
			@ReceiverKey,
			[Name]
	FROM	[dbo].[ReadModel_Players]
	WHERE	[Key] = @SenderKey
END
