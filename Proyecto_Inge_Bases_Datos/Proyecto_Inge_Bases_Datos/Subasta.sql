CREATE TABLE [dbo].[Subasta]
(
	
    [ProductoIDSubastado]	INT		NOT NULL ,
    [CorreoSubastador]		NVARCHAR(50)	NOT NULL,
    [FechaPublicado]    DATETIME NOT NULL,
    [PrecioMinimo] FLOAT NOT NULL,
    [FechaInicio] DATETIME2 NOT NULL DEFAULT NULL, 
    [FechaFin] DATETIME2 NOT NULL DEFAULT NULL, 
    [Calificacion] SMALLINT NULL DEFAULT NULL,
    [Estado] BIT NULL DEFAULT 1,

    PRIMARY KEY CLUSTERED ([ProductoIDSubastado],[CorreoSubastador], [FechaPublicado] ),
    CONSTRAINT [FK_dbo.Subasta_dbo.Producto] FOREIGN KEY (ProductoIDSubastado, CorreoSubastador) 
    REFERENCES [dbo].[Producto] (ProductoID,CorreoCliente),
)

