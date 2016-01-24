CREATE TABLE [dbo].[WriteModel_Snapshots]
(
	[Key] UNIQUEIDENTIFIER NOT NULL, 
    [Version] INT NOT NULL, 
    [Value] NVARCHAR(MAX) NOT NULL, 
    [Type] NVARCHAR(256) NOT NULL, 
    CONSTRAINT [PK_Snapshots] PRIMARY KEY ([Key], [Version]) 
)

GO

CREATE INDEX [IX_Snapshots_Key] ON [dbo].[WriteModel_Snapshots] ([Key])
