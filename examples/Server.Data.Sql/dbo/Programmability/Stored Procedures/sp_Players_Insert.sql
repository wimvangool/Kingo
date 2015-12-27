CREATE PROCEDURE [dbo].[sp_Players_Insert]
	@Key uniqueidentifier,
	@Name nvarchar(20),
	@Version int,
	@Value nvarchar(MAX)	
AS
BEGIN
	INSERT INTO [dbo].[Players] ([Key], [Name], [Version], [Value])
	VALUES						(@Key,  @Name,  @Version,  @Value)	
END
