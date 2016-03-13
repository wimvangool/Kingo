CREATE PROCEDURE [dbo].[sp_PendingChallenges_Delete]
(
	@ChallengeKey UNIQUEIDENTIFIER
)
AS
BEGIN
	DELETE FROM [dbo].[ReadModel_PendingChallenges]
	WHERE		[ChallengeKey] = @ChallengeKey
END
