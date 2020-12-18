using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto_Inge_Bases_Web.Models;
using PagedList;
using Microsoft.Ajax.Utilities;
using System.Data.Entity.Validation;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Runtime.Remoting.Contexts;
using System.Data.Entity.Core.Objects;

namespace Proyecto_Inge_Bases_Web.Controllers
{
    public class ProductoController : Controller
    {
        private TempPIEntities db = new TempPIEntities();


        // GET: Producto
        /**
            @Param: sortOrder. string que indica de que manera se va a ordenar la lista.
            @Param: currentFilter. Permite saber la configuracion de filtrado
            @Param: searchString. Recibe el string con lo que desea buscar.
            @Param: page. Permite saber cual es la pagina a mostrar
        */

        public ActionResult Index(string searchBy, string currentFilter, string searchString, int? page)
        {
            //var cookiecorreo = ControllerContext.HttpContext.Request.Cookies["user"];
            string correoActual = (string)HttpContext.Session["Correiro"];

            ViewBag.CurrentSort = searchBy;
            ViewBag.NameSortParm = String.IsNullOrEmpty(searchBy) ? "Name asc" : "";
            ViewBag.SortPriceParm = searchBy == "Price" ? "Price desc" : "Price";


            //Esta condición se utiliza por  que el filtro se debe mantener siempre; cuando alguien realiza una busqueda mediante el searchbox 
            //este se debe reiniciar, por eso la condicion not null

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var products = db.Productoes.Include(c => c.Cliente);
            var productos = from p in db.Productoes
                            select p;

            productos = productos.Where(p => p.Estado == true);

            // Es para que me salgan solo mis productos, (yo siendo el usuario logueado)
            if (correoActual != null)
            {
                productos = productos.Where(p => p.CorreoCliente == correoActual);
            }
            else
            {
                return RedirectToAction("IniciarSesion", "Registrado");
            }

            //Este if es el que se encarga de sacar los resultados que se vinculan a la busqueda realizada
            if (!String.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(p => p.Nombre.Contains(searchString));
            }

            //Primero la busqueda y luego el ordenamiento, este switch va a permitir agregar distintas opciones de ordenamiento
            switch (searchBy)
            {
                case "Name asc":
                    productos = productos.OrderBy(p => p.Nombre);
                    break;

                case "Price desc":
                    productos = productos.OrderByDescending(p => p.PrecioEstimado);
                    break;
                case "Price":
                    productos = productos.OrderBy(p => p.PrecioEstimado);
                    break;
                default:
                    productos = productos.OrderBy(p => p.Nombre);
                    break;
            }
            //Esto convierte una sola pagina en una colección para realizar la paginacion
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(productos.ToPagedList(pageNumber, pageSize));
        }

        // GET: Producto/Details/5
        public ActionResult Details(int? id, string correo)
        {
            var cookiecorreo = ControllerContext.HttpContext.Request.Cookies["user"];
            string correoActual = (string)HttpContext.Session["Correiro"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productoes.Find(id, correo);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // GET: Producto/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Categoria = new SelectList(db.Categorias, "CategoriaId", "Nombre");
            string correoActual = (string)HttpContext.Session["Correiro"];
            if (correoActual != null) //Esto se supone que saca la información de la cookie del correo 
                return View();
            else
                return RedirectToAction("IniciarSesion", "Registrado"); // Me devuelve a registrarme
        }

        // POST: Producto/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductoID,CorreoCliente,Nombre,PrecioEstimado,Condicion,Descripcion,Publicado,PathImagen1,PathImagen2,PathImagen3,FechaRegistrado")] Producto producto, HttpPostedFileBase Archivo_Imagen1, HttpPostedFileBase Archivo_Imagen2, HttpPostedFileBase Archivo_Imagen3, FormCollection form)
        {
            ViewBag.Categoria = new SelectList(db.Categorias, "CategoriaId", "Nombre");
            producto.FechaRegistrado = DateTime.Now;
            Debug.WriteLine("Create/productoID: " + producto.ProductoID);

            //Debug.WriteLine("Se crea: " + producto.FechaRegistrado);

            string derechosAutor = Convert.ToString(form["Derechos"].ToString());
            string tipoArchivo = Convert.ToString(form["TipoArchivo"].ToString());
            string fuentes = Convert.ToString(form["Fuentes"].ToString());

            string correoActual = (string)HttpContext.Session["Correiro"];
            if (correoActual != null) //Esto se supone que saca la información de la cookie del correo 
                producto.CorreoCliente = correoActual;
            else
                return RedirectToAction("IniciarSesion", "Registrado"); // Me devuelve a registrarme
            string Nombre_Archivo = "", Extension_Archivo = "";

            if (Archivo_Imagen1 != null)   //Bloque que guarda imagen 1 de producto 
            {
                Extension_Archivo = Path.GetExtension(Archivo_Imagen1.FileName);        //Obtiene el tipo de archivo (.jpg, .png, .jpeg)
                Nombre_Archivo = producto.CorreoCliente + producto.Nombre + "_1" + Extension_Archivo;   //Crea el nombre que va a tener la imagen del producto y se junta con el tipo de archivo
                producto.PathImagen1 = "~/Fotos/" + Nombre_Archivo;                     //Guarda el path en la tabla
                Nombre_Archivo = Path.Combine(Server.MapPath("~/Fotos/"), Nombre_Archivo);  //Combina el path donde se almacenan las fotos y nombre del archivo
                Archivo_Imagen1.SaveAs(Nombre_Archivo);     //Guarda la imagen dentro de la carpeta
            }
            if (Archivo_Imagen2 != null)   //Bloque que guarda imagen 2 de producto 
            {
                Nombre_Archivo = "";
                Extension_Archivo = "";
                Extension_Archivo = Path.GetExtension(Archivo_Imagen2.FileName);
                Nombre_Archivo = producto.CorreoCliente + producto.Nombre + "_2" + Extension_Archivo;
                producto.PathImagen2 = "~/Fotos/" + Nombre_Archivo;
                Nombre_Archivo = Path.Combine(Server.MapPath("~/Fotos/"), Nombre_Archivo);
                Archivo_Imagen2.SaveAs(Nombre_Archivo);
            }
            if (Archivo_Imagen3 != null)   //Bloque que guarda imagen 3 de producto 
            {
                Nombre_Archivo = "";
                Extension_Archivo = "";
                Extension_Archivo = Path.GetExtension(Archivo_Imagen3.FileName);
                Nombre_Archivo = producto.CorreoCliente + producto.Nombre + "_3" + Extension_Archivo;
                producto.PathImagen3 = "~/Fotos/" + Nombre_Archivo;
                Nombre_Archivo = Path.Combine(Server.MapPath("~/Fotos/"), Nombre_Archivo);
                Archivo_Imagen3.SaveAs(Nombre_Archivo);
            }
            if (ModelState.IsValid)
            {
                if (producto.InsertProducto(producto) == "ERROR")
                {
                    return RedirectToAction("Producto", "Index");

                }
            }

            int opcion = Convert.ToUInt16(form["TipoProducto"].ToString()); //UInt ya que el selector no tiene valores negativos. Con esta opción guardamos a la tabla físico o virtual.
            //1: Físico.    2: Virtual
            if (opcion == 1)
            {
                insertProductoFisico(correoActual);
            }
            else
            {
                insertProductoVirtual(correoActual);

            }


            return RedirectToAction("Index");

        }


        public void insertProductoFisico(string correoActual)
        {
            // Esta dando error de restricciones de llave foranea
            // Anti SQL Inyection, pruebelo

            // al no mapearse se pueden llamar a la libre

            db.spFisico_Insert(correoActual);

        }

        public void insertProductoVirtual(string correoActual)
        {



            //db.spVirtual_Insert(correoActual, "Prueba", "Prueba", "Prueba", DateTime.Now);

        }

        // GET: Producto/Edit/5
        public ActionResult Edit(int? id, string correo)
        {
            var cookiecorreo = ControllerContext.HttpContext.Request.Cookies["user"];
            string correoActual = (string)HttpContext.Session["Correiro"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productoes.Find(id, correo);
            if (producto == null)
            {
                return HttpNotFound();
            }
            ViewBag.correo = new SelectList(db.Clientes, "Correo", producto.CorreoCliente);
            return View(producto);
        }

        // POST: Producto/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductoID,CorreoCliente,Nombre,PrecioEstimado,Condicion,Descripcion,Publicado,PathImagen1,PathImagen2,PathImagen3,FechaRegistrado,FechaPublicado")] Producto producto, HttpPostedFileBase Archivo_Imagen1, HttpPostedFileBase Archivo_Imagen2, HttpPostedFileBase Archivo_Imagen3, string Publicado)
        {
            Debug.WriteLine("Se edita: " + producto.FechaRegistrado);

            var cookiecorreo = ControllerContext.HttpContext.Request.Cookies["user"];
            string correoActual = (string)HttpContext.Session["Correiro"];
            string Nombre_Archivo = "", Extension_Archivo = "";
            if (Publicado == "true")
            {
                producto.Publicado = true;
                producto.FechaPublicado = DateTime.Now;
            }

            if (Archivo_Imagen1 != null || Archivo_Imagen2 != null || Archivo_Imagen3 != null)
            {
                if (Archivo_Imagen1 != null)   //Bloque que guarda imagen 1 de producto
                {
                    Extension_Archivo = Path.GetExtension(Archivo_Imagen1.FileName);        //Obtiene el tipo de archivo (.jpg, .png, .jpeg)
                    Nombre_Archivo = producto.CorreoCliente + producto.Nombre + "_1" + Extension_Archivo;   //Crea el nombre que va a tener la imagen del producto y se junta con el tipo de archivo
                    producto.PathImagen1 = "~/Fotos/" + Nombre_Archivo;                     //Guarda el path en la tabla
                    Nombre_Archivo = Path.Combine(Server.MapPath("~/Fotos/"), Nombre_Archivo);  //Combina el path donde se almacenan las fotos y nombre del archivo
                    Archivo_Imagen1.SaveAs(Nombre_Archivo);     //Guarda la imagen dentro de la carpeta
                }
                if (Archivo_Imagen2 != null)   //Bloque que guarda imagen 2 de producto
                {
                    Nombre_Archivo = "";
                    Extension_Archivo = "";
                    Extension_Archivo = Path.GetExtension(Archivo_Imagen2.FileName);
                    Nombre_Archivo = producto.CorreoCliente + producto.Nombre + "_2" + Extension_Archivo;
                    producto.PathImagen2 = "~/Fotos/" + Nombre_Archivo;
                    Nombre_Archivo = Path.Combine(Server.MapPath("~/Fotos/"), Nombre_Archivo);
                    Archivo_Imagen2.SaveAs(Nombre_Archivo);
                }
                if (Archivo_Imagen3 != null)   //Bloque que guarda imagen 3 de producto
                {
                    Nombre_Archivo = "";
                    Extension_Archivo = "";
                    Extension_Archivo = Path.GetExtension(Archivo_Imagen3.FileName);
                    Nombre_Archivo = producto.CorreoCliente + producto.Nombre + "_3" + Extension_Archivo;
                    producto.PathImagen3 = "~/Fotos/" + Nombre_Archivo;
                    Nombre_Archivo = Path.Combine(Server.MapPath("~/Fotos/"), Nombre_Archivo);
                    Archivo_Imagen3.SaveAs(Nombre_Archivo);
                }
            }
            else
            {
                // Si no selecciona foto nueva pero anteriormente había una
                // entonces mantiene la información de la anterior
                producto.PathImagen1 = producto.PathImagen1;
                producto.PathImagen2 = producto.PathImagen2;
                producto.PathImagen3 = producto.PathImagen3;
            }

            if (ModelState.IsValid)
            {
                db.Entry(producto).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    throw;
                }
                return RedirectToAction("Index");
            }
            ViewBag.CorreoCliente = new SelectList(db.Clientes, "user", producto.CorreoCliente);
            return View(producto);
        }

        // GET: Producto/Delete/5
        public ActionResult Delete(int? id, string correo)
        {
            var cookiecorreo = ControllerContext.HttpContext.Request.Cookies["user"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productoes.Find(id, correo);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Producto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string correo)
        {
            var cookiecorreo = ControllerContext.HttpContext.Request.Cookies["user"];
            Producto producto = db.Productoes.Find(id, correo);
            producto.Estado = false;

            //db.Productoes.Remove(producto);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Insertar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Insertar(Producto obj)
        {
            Producto objreg = new Producto();
            //string result = objreg.InsertProducto(obj);
            // ViewData["result"] = result;
            ModelState.Clear();
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        /* Acciones a Usuario  */
        public ActionResult VerSignUp_Usuario()
        {
            return RedirectToAction("SignUp", "Usuario");
        }
        /* -------------------------------*/


        /*  Metodos para ProductoDelDia */
        public ActionResult VerIndex_ProductosDelDia()
        {
            return RedirectToAction("Index", "ProductosDelDia");
        }

        [Serializable]
        private class DbEntityValidationException : Exception
        {
            public DbEntityValidationException()
            {
            }

            public DbEntityValidationException(string message) : base(message)
            {
            }

            public DbEntityValidationException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected DbEntityValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

            public IEnumerable<object> EntityValidationErrors { get; internal set; }
        }



    }
}
