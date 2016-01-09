CREATE PROCEDURE [dbo].[sp_Challenges_Insert]
(
	@Key UNIQUEIDENTIFIER,	
	@Version INT,
	@Value NVARCHAR(MAX)	
)
AS
BEGIN
	INSERT INTO [dbo].[Challenges] ([Key], [Version], [Value])
	VALUES						   (@Key,  @Version,  @Value)	
END
