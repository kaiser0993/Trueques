CREATE PROCEDURE [dbo].[spVirtual_Insert]
	@CorreoCliente NVARCHAR(50),
	@TipoDeArchivo NVARCHAR(50),
	@DerechosDeProducto NVARCHAR(50) NULL,
	@Fuentes NVARCHAR(50) NULL,
	@FechaExpiracion DATETIME NULL
	

AS
BEGIN 

		DECLARE @ProductoVirtualID INT
		-- Este select es porque los ID son autoFill, entonces se busca el ultimo que se ingreso
		SET @ProductoVirtualID =  
		(
			SELECT TOP 1 ProductoID 
			FROM Producto 
			ORDER BY FechaRegistrado DESC
		);



	INSERT INTO [Virtual] ([ProductoID], [CorreoCliente], [TipoDeArchivo], [DerechosDeProducto], [Fuentes], [FechaExpiracion])
		VALUES(@ProductoVirtualID, @CorreoCliente, @TipoDeArchivo, @DerechosDeProducto, @Fuentes, @FechaExpiracion)
END