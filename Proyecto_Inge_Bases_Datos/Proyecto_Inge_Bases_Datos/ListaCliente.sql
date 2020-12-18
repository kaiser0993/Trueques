CREATE TABLE [dbo].[ListaCliente]
(
	[Nombre]		NVARCHAR (50)	NOT NULL,
	[Correo]		NVARCHAR (50)	NOT NULL,
	PRIMARY KEY ([Nombre], [Correo] ASC),
	CONSTRAINT [FK_dbo.Cliente_dbo.ListaCliente] FOREIGN KEY ([Correo])
		REFERENCES [dbo].Cliente ([Correo])
)
