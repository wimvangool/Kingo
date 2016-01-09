CREATE TYPE [dbo].[Events] AS TABLE
(	
	[Version] INT UNIQUE,
	[Value] NVARCHAR(MAX) NOT NULL
)
