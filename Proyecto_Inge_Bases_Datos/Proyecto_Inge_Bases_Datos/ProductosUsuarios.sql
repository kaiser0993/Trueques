CREATE VIEW [dbo].[ProductosUsuarios]
	AS SELECT r.Nombre,r.Correo, r.Apellido1, r.Apellido2, count(p.ProductoID) as CantidadProductos
FROM [dbo].[Producto] p
INNER JOIN [dbo].[Registrado] r
ON p.CorreoCliente=r.Correo
GROUP BY r.Nombre,r.Correo, r.Apellido1, r.Apellido2, p.CorreoCliente;
	
