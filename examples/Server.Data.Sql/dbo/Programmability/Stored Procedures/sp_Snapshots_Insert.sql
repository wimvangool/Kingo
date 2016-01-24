CREATE PROCEDURE [dbo].[sp_Snapshots_Insert]
(
	@Key UNIQUEIDENTIFIER,
	@Version INT,
	@Value NVARCHAR(MAX),
	@Type NVARCHAR(256)
)
AS
BEGIN
	INSERT INTO [dbo].[WriteModel_Snapshots]
	(
		[Key],
		[Version],
		[Value],
		[Type]
	)
	VALUES
	(
		@Key,
		@Version,
		@Value,
		@Type
	);
END
