CREATE TABLE [dbo].[Players] (
    [Key]      UNIQUEIDENTIFIER NOT NULL,
    [Name]    NVARCHAR (20)    NOT NULL,
    [Version] INT              NOT NULL,
    [Value]    NVARCHAR (MAX)   NOT NULL,
    CONSTRAINT [PK_Players_Id] PRIMARY KEY CLUSTERED ([Key] ASC),
    CONSTRAINT [CK_Players_Version] CHECK ([Version]>(0))
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Players_Name]
    ON [dbo].[Players]([Name] ASC);

