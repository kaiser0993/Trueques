CREATE PROCEDURE [dbo].[spFisico_Insert]
	@CorreoCliente NVARCHAR (50)
AS
BEGIN	
		DECLARE @ProductoFisicoID INT

		SET @ProductoFisicoID =  
		(
		SELECT TOP 1 ProductoID 
		FROM Producto 
		ORDER BY FechaRegistrado DESC
		);
		
		INSERT INTO [Fisico] ([ProductoID], [CorreoCliente])
		VALUES(@ProductoFisicoID, @CorreoCliente)
END
