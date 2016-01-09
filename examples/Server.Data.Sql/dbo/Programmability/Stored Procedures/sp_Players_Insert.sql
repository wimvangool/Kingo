CREATE PROCEDURE [dbo].[sp_Players_Insert]
(
	@Key UNIQUEIDENTIFIER,
	@Name NVARCHAR(20),
	@Version INT,
	@Value NVARCHAR(MAX)	
)
AS
BEGIN
	INSERT INTO [dbo].[Players] ([Key], [Name], [Version], [Value])
	VALUES						(@Key,  @Name,  @Version,  @Value)	
END
