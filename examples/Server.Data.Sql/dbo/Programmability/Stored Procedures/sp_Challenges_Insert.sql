CREATE PROCEDURE [dbo].[sp_Challenges_Insert]
	@Key uniqueidentifier,	
	@Version int,
	@Value nvarchar(MAX)	
AS
BEGIN
	INSERT INTO [dbo].[Challenges] ([Key], [Version], [Value])
	VALUES						   (@Key,  @Version,  @Value)	
END
