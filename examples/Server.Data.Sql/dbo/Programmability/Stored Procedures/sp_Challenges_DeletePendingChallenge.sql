CREATE PROCEDURE [dbo].[sp_Challenges_DeletePendingChallenge]
(
	@ChallengeKey UNIQUEIDENTIFIER
)
AS
BEGIN
	DELETE FROM [dbo].[PendingChallenges]
	WHERE		[ChallengeKey] = @ChallengeKey
END
