CREATE TABLE [dbo].[PendingChallenges]
(    
	[ChallengeKey] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
	[ReceiverKey] UNIQUEIDENTIFIER NOT NULL,
    [SenderName] NVARCHAR(20) NOT NULL
)

GO

CREATE INDEX [IX_PendingChallenges_ReceiverKey] ON [dbo].[PendingChallenges] ([ReceiverKey])
