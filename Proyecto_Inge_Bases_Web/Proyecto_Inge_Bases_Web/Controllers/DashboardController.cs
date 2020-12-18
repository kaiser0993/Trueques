using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using Proyecto_Inge_Bases_Web.Models;
using System.Data.Entity.Validation;

namespace Proyecto_Inge_Bases_Web.Controllers
{
    
    public class DashboardController : Controller
    {
        private TempPIEntities db = new TempPIEntities();
        // GET: Dashboard
        public ActionResult Index()
        {
            //Para verificar que el correo sea de un administrador.
            string correo = (string)HttpContext.Session["Correiro"];
            Administrador admin = db.Administradors.Find(correo);

            if (admin == null)
            {
                return RedirectToAction("IniciarSesion", "Registrado");
            }

            var usuario = from u in db.Calificaciones_Cliente
                          select u;
            usuario = usuario.OrderBy(u => u.Nombre);

            //Datos de las categorias y sus productos
            var categoria = db.Categorias.Where(cat=>cat.CategoriaPadreID == 0);
            categoria = categoria.OrderBy(u => u.CategoriaID);

            ViewBag.ListaCategorias = categoria.ToList();

            var ListaCategoriaNombre = categoria.Select(c => c.Nombre).ToList();

            ViewBag.ListaNombreString = string.Join(",", ListaCategoriaNombre.Select(x => string.Format("'{0}'", x))).Trim();
           
            var ListaProductosCategoria = categoria.Select(c => c.Productoes).ToList();

           var  ListaCantidadProductos  = new List<int>();

            foreach (var ListaProductos in ListaProductosCategoria) {
                ListaCantidadProductos.Add(ListaProductos.Count());  
            }
            ViewBag.ListaCanProductosString = string.Join(",", ListaCantidadProductos).Trim();

            return View(usuario.ToList());
        }

        public ActionResult EdadesClientes() 
        {
            //Se encarga de la pagina del grafico de las edades de los clientes.

            //Para verificar que el correo sea de un administrador.
            string correo = (string)HttpContext.Session["Correiro"];
            Administrador admin = db.Administradors.Find(correo);

            if (admin == null)
            {
                return RedirectToAction("IniciarSesion", "Registrado");
            }

            var usuario = from u in db.Calificaciones_Cliente
                          select u;
            usuario = usuario.OrderBy(u => u.Nombre);

            return View(usuario.ToList());
        }
        public ActionResult CalificacionesUsuarios()
        {
            //Se encarga de la pagina del grafico de las edades de los clientes.

            //Para verificar que el correo sea de un administrador.
            string correo = (string)HttpContext.Session["Correiro"];
            Administrador admin = db.Administradors.Find(correo);

            if (admin == null)
            {
                return RedirectToAction("IniciarSesion", "Registrado");
            }

            var usuario = from u in db.Calificaciones_Cliente
                          select u;
            usuario = usuario.OrderBy(u => u.Nombre);

            return View(usuario.ToList());
        }

        public ActionResult ProductosPorCategoria()
        {
            //Se encarga de la pagina del grafico de la cantidad de productos por categoría.

            //Para verificar que el correo sea de un administrador.
            string correo = (string)HttpContext.Session["Correiro"];
            Administrador admin = db.Administradors.Find(correo);

            if (admin == null)
            {
                return RedirectToAction("IniciarSesion", "Registrado");
            }

            var categoria = db.Categorias.Where(cat => cat.CategoriaPadreID == 0);
            categoria = categoria.OrderBy(u => u.CategoriaID);

            ViewBag.ListaCategorias = categoria.ToList();

            var ListaCategoriaNombre = categoria.Select(c => c.Nombre).ToList();

            ViewBag.ListaNombreString = string.Join(",", ListaCategoriaNombre.Select(x => string.Format("'{0}'", x))).Trim();

            var ListaProductosCategoria = categoria.Select(c => c.Productoes).ToList();

            var ListaCantidadProductos = new List<int>();

            foreach (var ListaProductos in ListaProductosCategoria)
            {
                ListaCantidadProductos.Add(ListaProductos.Count());
            }
            ViewBag.ListaCanProductosString = string.Join(",", ListaCantidadProductos).Trim();

            return View();
        }



        public ActionResult VerIndex_ProductosDelDia()
        {
            return RedirectToAction("Index", "ProductosDelDia");
        }

        /* Acciones a Producto */
        public ActionResult Ver_Estadisticas()
        {
            return RedirectToAction("Index", "Estadisticas");
        }

        /*-------------------------*/

        /*  Metodos para Usuario */
        public ActionResult VerCategorias()
        {

            return RedirectToAction("Index", "Categorias");

        }
    }


}