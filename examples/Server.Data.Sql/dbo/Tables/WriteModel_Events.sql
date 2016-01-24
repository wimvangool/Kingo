CREATE TABLE [dbo].[WriteModel_Events]
(
	[Key] UNIQUEIDENTIFIER NOT NULL, 
    [Version] INT NOT NULL, 
    [Value] NVARCHAR(MAX) NOT NULL, 
    [Type] NVARCHAR(256) NOT NULL, 
    CONSTRAINT [PK_Events] PRIMARY KEY ([Key], [Version])
)

GO

CREATE INDEX [IX_Events_Key] ON [dbo].[WriteModel_Events] ([Key])
