CREATE PROCEDURE [dbo].[sp_PendingChallenges_SelectByReceiver]
(
	@ReceiverKey UNIQUEIDENTIFIER
)
AS
BEGIN
	SELECT	[ChallengeKey],
			[SenderName]
	FROM	[dbo].[ReadModel_PendingChallenges]
	WHERE	[ReceiverKey] = @ReceiverKey;
END
