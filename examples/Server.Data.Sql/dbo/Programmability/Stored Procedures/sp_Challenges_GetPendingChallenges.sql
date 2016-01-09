CREATE PROCEDURE [dbo].[sp_Challenges_GetPendingChallenges]
(
	@ReceiverKey UNIQUEIDENTIFIER
)
AS
BEGIN
	SELECT	[ChallengeKey], [SenderName]
	FROM	[dbo].[PendingChallenges]
	WHERE	[ReceiverKey] = @ReceiverKey;
END
