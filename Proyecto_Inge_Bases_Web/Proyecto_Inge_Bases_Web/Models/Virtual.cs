//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Proyecto_Inge_Bases_Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Virtual
    {
        public int ProductoID { get; set; }
        public string CorreoCliente { get; set; }
        public string TipoDeArchivo { get; set; }
        public string DerechosDeProducto { get; set; }
        public string Fuentes { get; set; }
        public Nullable<System.DateTime> FechaExpiracion { get; set; }
    
        public virtual Producto Producto { get; set; }
    }
}