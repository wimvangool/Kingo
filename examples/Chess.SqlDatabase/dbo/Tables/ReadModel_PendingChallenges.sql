CREATE TABLE [dbo].[ReadModel_PendingChallenges]
(    
	[ChallengeKey] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
	[ReceiverKey] UNIQUEIDENTIFIER NOT NULL,
    [SenderName] NVARCHAR(20) NOT NULL
)

GO

CREATE INDEX [IX_PendingChallenges_ReceiverKey] ON [dbo].[ReadModel_PendingChallenges] ([ReceiverKey])
