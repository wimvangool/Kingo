CREATE PROCEDURE [dbo].[sp_Challenges_InsertPendingChallenge]
(
	@ChallengeKey UNIQUEIDENTIFIER,
	@SenderKey UNIQUEIDENTIFIER,
	@ReceiverKey UNIQUEIDENTIFIER	
)
AS
BEGIN
	INSERT INTO [dbo].[PendingChallenges] ([ChallengeKey], [ReceiverKey], [SenderName])
	SELECT	@ChallengeKey, @ReceiverKey, [Name]
	FROM	[dbo].Players
	WHERE	[Key] = @SenderKey
END
