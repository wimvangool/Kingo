CREATE TABLE [dbo].[Games]
(
	[Key] UNIQUEIDENTIFIER NOT NULL, 
    [Version] INT NOT NULL, 
    [Value] NVARCHAR(MAX) NOT NULL,
	CONSTRAINT [PK_Games_Key] PRIMARY KEY CLUSTERED ([Key] ASC),
    CONSTRAINT [CK_Games_Version] CHECK ([Version]>(0))
)
