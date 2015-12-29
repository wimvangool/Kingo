CREATE TABLE [dbo].[Challenges]
(
	[Key] UNIQUEIDENTIFIER NOT NULL, 
    [Version] INT NOT NULL, 
    [Value] NVARCHAR(MAX) NOT NULL,
	CONSTRAINT [PK_Challenges_Key] PRIMARY KEY CLUSTERED ([Key] ASC),
    CONSTRAINT [CK_Challenges_Version] CHECK ([Version]>(0))
)
