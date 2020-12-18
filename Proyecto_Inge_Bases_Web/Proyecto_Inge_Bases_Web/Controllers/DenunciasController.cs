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

    public class DenunciasController : Controller
    {
        private TempPIEntities db = new TempPIEntities();
        // GET: Dashboard
        public ActionResult Index()
        {
            //Para verificar que el correo sea de un administrador.
            string correo = (string)HttpContext.Session["Correiro"];
            Administrador admin = db.Administradors.Find(correo);
            List<Registrado> registrados = db.Registradoes.ToList();
            //ViewBag.usuarios(registrados);

            if (admin == null)
            {
                return RedirectToAction("IniciarSesion", "Registrado");
            }

            List<Denuncia> denuncias = db.Denuncias.ToList();
            return View(denuncias);

        }
        public ActionResult Realizar_Denuncia(string correo)
        {
            var usuario = db.Registradoes.Find(correo);
            var denuncia = new Denuncia();
            denuncia.Denunciado = usuario.Correo;
            string correo2 = (string)HttpContext.Session["Correiro"];
            denuncia.Denunciante = correo2;
            DateTime fechaHoy = DateTime.UtcNow;
            denuncia.Fecha = fechaHoy;
            ViewBag.Nombre = usuario.Nombre;
            return View(denuncia);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Realizar_Denuncia([Bind(Include = "Fecha,Comentarios,Denunciante,Denunciado,Tipo")] Denuncia denuncias)
        {
            if (ModelState.IsValid)
            {
                db.Denuncias.Add(denuncias);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(denuncias);
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



        /*  Metodos para Usuario */
        public ActionResult VerCategorias()
        {

            return RedirectToAction("Index", "Categorias");

        }
        public ActionResult EliminarDenuncia(DateTime fecha)
        {

            Denuncia adicional = db.Denuncias.Find(fecha);

            if (adicional != null)
            {
                db.Denuncias.Remove(adicional);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult BloquearUsuario(String denunciado)
        {

            Registrado registrados = db.Registradoes.Find(denunciado);
            var productos = from p in db.Productoes
                            where p.CorreoCliente == denunciado
                            select p;
            productos = productos.OrderBy(p => p.Nombre);
            var denuncias = from q in db.Denuncias
                            where q.Denunciado == denunciado
                            select q;
            denuncias = denuncias.OrderBy(q => q.Fecha);
            foreach (var producto in productos)
            {
                producto.FechaPublicado = null;
                producto.Publicado = false;
            }
            foreach (var denuncia in denuncias)
            {
                db.Denuncias.Remove(denuncia);
            }
            if (registrados != null)
            {
                registrados.Bloqueo = true;
                db.SaveChanges();

            }

            return RedirectToAction("Index");
        }

    }


}