CREATE TABLE [dbo].[SublistasAmigos]
(
	[CorreoDueno]		NVARCHAR (50)	NOT NULL,
	[CorreoAmigo]		NVARCHAR (50)	NOT NULL,
	[NombreSublista]	NVARCHAR (50)	NOT NULL,
	PRIMARY KEY ([NombreSublista],[CorreoDueno], [CorreoAmigo] ASC),
	CONSTRAINT [FK_dbo.SublistasAmigos_dbo.ListaAmigos] FOREIGN KEY ([CorreoDueno],[CorreoAmigo])
		REFERENCES [dbo].[ListaAmigos] ([CorreoDueno],[CorreoAmigo])
)
