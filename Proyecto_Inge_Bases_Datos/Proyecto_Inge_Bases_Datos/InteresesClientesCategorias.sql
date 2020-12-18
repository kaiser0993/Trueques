CREATE TABLE [dbo].[InteresesClientesCategorias]
(
	[IDCategoria] INT NOT NULL,
	[CorreoCliente] NVARCHAR(50) NOT NULL,
	CONSTRAINT [FK_dbo.InteresesClientesCategorias_dbo.Categoria] FOREIGN KEY ([IDCategoria])
	 REFERENCES [dbo].[Categoria] ([CategoriaID]) ON DELETE CASCADE,
	CONSTRAINT [FK_dbo.InteresesClientesCategorias_dbo.Registrado] FOREIGN KEY ([CorreoCliente])
	 REFERENCES [dbo].[Cliente] ([Correo]) --ON DELETE CASCADE
)
