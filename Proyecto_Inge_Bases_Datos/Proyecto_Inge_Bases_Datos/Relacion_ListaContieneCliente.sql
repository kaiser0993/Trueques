CREATE TABLE [dbo].[Relacion_ListaContieneCliente]
(
	[Nombre_Lista]	NVARCHAR (50)	NOT NULL,
	[Correo]		NVARCHAR (50)	NOT NULL,
	[CorreoAmigo]	NVARCHAR (50)	NOT NULL,
	PRIMARY KEY ([Nombre_Lista], [Correo], [CorreoAmigo] ASC),
	CONSTRAINT [FK_dbo.ListaCliente_dbo.Relacion_ListaContieneCliente] FOREIGN KEY ([Nombre_Lista], [Correo])
		REFERENCES [dbo].ListaCliente ([Nombre], [Correo]),
	CONSTRAINT [FK_dbo.Cliente_dbo.Relacion_ListaContieneCliente] FOREIGN KEY ([CorreoAmigo])
		REFERENCES [dbo].Cliente ([Correo])
)
