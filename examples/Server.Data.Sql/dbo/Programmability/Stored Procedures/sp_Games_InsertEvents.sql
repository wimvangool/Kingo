CREATE PROCEDURE [dbo].[sp_Games_InsertEvents]
(
	@Key UNIQUEIDENTIFIER,
	@OldVersion INT,
	@Events dbo.Events READONLY
)
AS
BEGIN
	INSERT INTO [dbo].[Games] ([Key], [Version], [Value])
	SELECT	@Key, [Version], [Value]
	FROM	@Events
	WHERE	@OldVersion IS NULL
	OR		@OldVersion = (SELECT MAX([Version]) FROM [dbo].[Games])
END
