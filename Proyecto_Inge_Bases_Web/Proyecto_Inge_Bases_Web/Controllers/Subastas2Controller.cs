using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Globalization;
using System.Web;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Services;
using Proyecto_Inge_Bases_Web.Models;
using PagedList;
using Microsoft.Ajax.Utilities;
using System.Data.Entity.Validation;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Data.Entity.Migrations;

namespace Proyecto_Inge_Bases_Web.Controllers
{
    public class Subastas2Controller : Controller
    {
        private TempPIEntities db = new TempPIEntities();

        // GET: Subastas2
        public ActionResult Index()
        {
            var cookiecorreo = ControllerContext.HttpContext.Request.Cookies["user"];
            string correoActual = (string)HttpContext.Session["Correiro"];
            var subastas = db.Subastas.Include(s => s.Producto);
            if (correoActual != null) //Esto se supone que saca la información de la cookie del correo 
                return View(subastas.ToList());
            else
                return RedirectToAction("IniciarSesion", "Registrado");
            //return View(subastas.ToList());
        }

        // GET: Subastas2/Details/5
        public ActionResult Details(int? idProducto, string correo, DateTime fecha) //probar dejando datetime en lugar de string
        {
            //var fechaM = DateTimeOffset.Parse(fecha);
            //CultureInfo provider = CultureInfo.InvariantCulture;
           // string format = "MM/dd/yyyy hh:mm:ss.fff";
           // DateTime FechaI = DateTime.ParseExact(fecha, format, provider);


            var cookiecorreo = ControllerContext.HttpContext.Request.Cookies["user"];
            string correoActual = (string)HttpContext.Session["Correiro"];
            if (idProducto == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subasta subasta = db.Subastas.Find(idProducto, correo, fecha);
            //Subasta subasta = db.Subastas.Find(idProducto, correo, fecha);
            if (subasta == null)
            {
                return HttpNotFound();
            }
            return View(subasta);
        }


        // GET: Subastas2/Create

        public ActionResult Create(int idProducto, string correo, string imagen , string producto,string condicion, float precio )
        {
            
            Subasta subasta = new Subasta();
            subasta.ProductoIDSubastado = idProducto;
            subasta.CorreoSubastador = correo;
            

            ViewBag.PathImagen1 = imagen;
            ViewBag.Nombre = producto;
            ViewBag.Condicion = condicion;
            ViewBag.PrecioEstimado = precio;
            string correoActual = (string)HttpContext.Session["Correiro"];
            //var insertar = db.Productoes.SqlQuery("[dbo].[Producto_InsertarProducto]").ToList();
            if (correoActual != null) //Esto se supone que saca la información de la cookie del correo 
                return View(subasta);
            else
                return RedirectToAction("IniciarSesion", "Registrado");

            //ViewBag.ProductoIDSubastado = new SelectList(db.Productoes, "ProductoID", "Nombre");
            //return RedirectToAction("Create", "Subastas2", new { });
            //return View();
        }

        // POST: Subastas2/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductoIDSubastado,CorreoSubastador,FechaPublicado,PrecioMinimo,FechaInicio,FechaFin,Calificacion")] Subasta subasta)
        {
            string fecha = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
            CultureInfo provider = CultureInfo.InvariantCulture;
            string format = "MM/dd/yyyy hh:mm:ss";
            DateTime FechaI = DateTime.ParseExact(fecha, format, provider);

            // string fecha = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
            subasta.FechaPublicado = FechaI;
            //subasta.FechaPublicado = DateTime.Now;
            string correoActual = (string)HttpContext.Session["Correiro"];
            if (correoActual != null) //Esto se supone que saca la información de la cookie del correo 
                subasta.CorreoSubastador = correoActual;
            else
                return RedirectToAction("IniciarSesion", "Registrado");

            Producto pro = db.Productoes.Find(subasta.ProductoIDSubastado, subasta.CorreoSubastador);
            ViewBag.Nombre = pro.Nombre;
            ViewBag.Condicion = pro.Condicion;
            ViewBag.PrecioEstimado = pro.PrecioEstimado;
            int dif = DateTime.Compare(subasta.FechaFin, DateTime.Now);
            int difI = DateTime.Compare(subasta.FechaInicio, DateTime.Now);
            int dif2 = DateTime.Compare(subasta.FechaFin, subasta.FechaInicio);
            
            if (dif < 0)   
            {
                ViewBag.AvisoFecFin = "La fecha es invalida";
                return View(subasta);
            }
            if (difI < 0)
            {
                ViewBag.AvisoFecIn = "La fecha es invalida";
                return View(subasta);
            }
            if (dif2 < 0)
            {
                ViewBag.Aviso = "La fecha final no puede ser anterior a la fecha inicial";
                return View(subasta);
            }


            var subastas = from p in db.Subastas
                            select p;

            // Es para que me salgan solo mis productos, (yo siendo el usuario logueado)
            
            subastas = subastas.Where(p => p.CorreoSubastador == correoActual && p.ProductoIDSubastado == subasta.ProductoIDSubastado);
            /*foreach (var sub in subastas)
            {
                
            }*/ //Hacer validacion de que si hay una subasta con el mismo idproducto y usuario revisar que haya finalizado para poder insertar otra

            if (ModelState.IsValid)
            {
                db.Subastas.Add(subasta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductoIDSubastado = new SelectList(db.Productoes, "ProductoID", "Nombre", subasta.ProductoIDSubastado);
            return View(subasta);
        }

        public ActionResult misSubastas()
        {
            var cookiecorreo = ControllerContext.HttpContext.Request.Cookies["user"];
            string correoActual = (string)HttpContext.Session["Correiro"];
            var subastas = db.Subastas.Include(s => s.Producto);
            if (correoActual != null) //Esto se supone que saca la información de la cookie del correo 
                return View(subastas.ToList());
            else
                return RedirectToAction("IniciarSesion", "Registrado");
        }

        public ActionResult OfertarEnSubasta(int idProducto,string correo,DateTime fechaPublicado, string producto, string condicion,string descripcion, float precio, DateTime fechaInicio, DateTime fechaFin)
        {
            string fecha = fechaPublicado.ToString("MM/dd/yyyy hh:mm:ss");
            CultureInfo provider = CultureInfo.InvariantCulture;
            string format = "MM/dd/yyyy hh:mm:ss";
            DateTime FechaI = DateTime.ParseExact(fecha, format, provider);

            Relacion_ClienteOfertaEnSubasta ofertarSubasta = new Relacion_ClienteOfertaEnSubasta();
            ofertarSubasta.ProductoIDSubastado = idProducto;
            ofertarSubasta.CorreoSubastador = correo;
            ofertarSubasta.FechaPublicado = FechaI;
            ViewBag.Nombre = producto;
            ViewBag.Condicion = condicion;
            ViewBag.PrecioMinimo = precio;
            ViewBag.Descripcion = descripcion;
            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;

            var ofertas = db.Relacion_ClienteOfertaEnSubasta.Include(c => c.Subasta);
            var oferta = from p in db.Relacion_ClienteOfertaEnSubasta
                         select p;
            oferta = oferta.Where(p => p.ProductoIDSubastado == idProducto);
            
            //double? maximo = oferta.Max(o => o.PrecioOfrecido);
            if (oferta.Any())
            {
                ViewBag.OfertaMaxima = oferta.Max(o => o.PrecioOfrecido);
            }
            else
            {
                ViewBag.OfertaMaxima = 0;
            }
            //float precioMaximo = 0;
           // Relacion_ClienteOfertaEnSubasta ofertaMayor = db.Relacion_ClienteOfertaEnSubastas.Max(s => s.PrecioOfrecido && s.ProductoIDSubastado == idProducto);

           // string correoActual = (string)HttpContext.Session["Correiro"];
            //var insertar = db.Productoes.SqlQuery("[dbo].[Producto_InsertarProducto]").ToList();
           // if (correoActual != null) //Esto se supone que saca la información de la cookie del correo 
                return View(ofertarSubasta);
           // else
          //      return RedirectToAction("IniciarSesion", "Registrado");
            //db.Relacion_ClienteOfertaEnSubastas =
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OfertarEnSubasta([Bind(Include = "CorreoOfertador, ProductoIDSubastado, CorreoSubastador, FechaPublicado, PrecioOfrecido")] Relacion_ClienteOfertaEnSubasta ofertaSubasta)
        {
            DateTime var = ofertaSubasta.FechaPublicado;
            string fecha = ofertaSubasta.FechaPublicado.ToString("MM/dd/yyyy hh:mm:ss");
            CultureInfo provider = CultureInfo.InvariantCulture;
            string format = "MM/dd/yyyy hh:mm:ss";
            DateTime FechaI = DateTime.ParseExact(fecha, format, provider);
            ofertaSubasta.FechaPublicado = FechaI;

            string correoActual = (string)HttpContext.Session["Correiro"];
            ofertaSubasta.CorreoOfertador = correoActual;
            var oferta = from p in db.Relacion_ClienteOfertaEnSubasta
                         select p;
            oferta = oferta.Where(p => p.ProductoIDSubastado == ofertaSubasta.ProductoIDSubastado);
            Relacion_ClienteOfertaEnSubasta ofertarSubastaParam = new Relacion_ClienteOfertaEnSubasta();
            ofertarSubastaParam.ProductoIDSubastado = ofertaSubasta.ProductoIDSubastado;
            ofertarSubastaParam.CorreoSubastador = ofertaSubasta.CorreoSubastador;
            ofertarSubastaParam.FechaPublicado = ofertaSubasta.FechaPublicado;
            Producto prod = db.Productoes.Find(ofertaSubasta.ProductoIDSubastado, ofertaSubasta.CorreoSubastador);
            Subasta sub = db.Subastas.Find(ofertaSubasta.ProductoIDSubastado, ofertaSubasta.CorreoSubastador, ofertaSubasta.FechaPublicado);
            ViewBag.Nombre = prod.Nombre;
            ViewBag.Condicion = prod.Condicion;
            ViewBag.PrecioMinimo = sub.PrecioMinimo;
            ViewBag.Descripcion = prod.Descripcion;
            ViewBag.FechaInicio = sub.FechaInicio;
            ViewBag.FechaFin = sub.FechaFin;
            var body1 = $"Truques@UCR informa: \n El usuario {ofertaSubasta.CorreoSubastador} ha enviado una oferta por una subasta";
            body1.Replace("\n", Environment.NewLine);
            var body2 = $"Truques@UCR informa: \n El usuario {correoActual} ha enviado una oferta por una subasta";
            body2.Replace("\n", Environment.NewLine);

            

            //que busque si el usuario que esta ofertando ya habia ofertado y si sí que se modifique
             

            if (oferta.Any())
            {
                if (ofertaSubasta.PrecioOfrecido < oferta.Max(o => o.PrecioOfrecido))
                {
                    ViewBag.OfertaMaxima = oferta.Max(o => o.PrecioOfrecido);
                    ViewBag.Aviso = "No puede ofrecer un precio menor al precio mayor ya ofrecido";
                    return View(ofertarSubastaParam);
                }
            }
            if (db.Relacion_ClienteOfertaEnSubasta.Find(ofertaSubasta.CorreoOfertador, ofertaSubasta.ProductoIDSubastado, ofertaSubasta.CorreoSubastador, ofertaSubasta.FechaPublicado) != null)
            {
                editarOferta(ofertaSubasta);
                return RedirectToAction("Index");
            }
            else 
            if (ModelState.IsValid)
            {
                db.Relacion_ClienteOfertaEnSubasta.Add(ofertaSubasta);
                db.SaveChanges();
                EnviarCorreo(correoActual, body1);
                EnviarCorreo(ofertaSubasta.CorreoSubastador, body2);
                return RedirectToAction("Index");
            }
            else
            {
                return HttpNotFound();
            }

            //ViewBag.ProductoIDSubastado = new SelectList(db.Relacion_ClienteOfertaEnSubastas, "ProductoID", "Nombre", ofertaSubasta.ProductoIDSubastado);
            //return View(ofertaSubasta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editarOferta([Bind(Include = "CorreoOfertador, ProductoIDSubastado, CorreoSubastador, FechaPublicado, PrecioOfrecido")] Relacion_ClienteOfertaEnSubasta ofertaSubasta)
        {
            var body1 = $"Truques@UCR informa: \n El usuario {ofertaSubasta.CorreoSubastador} ha enviado una oferta por una subasta";
            body1.Replace("\n", Environment.NewLine);
            var body2 = $"Truques@UCR informa: \n El usuario {ofertaSubasta.CorreoOfertador} ha enviado una oferta por una subasta";
            body2.Replace("\n", Environment.NewLine);
            if (ModelState.IsValid)
            {
               
                db.Set<Relacion_ClienteOfertaEnSubasta>().AddOrUpdate(ofertaSubasta);
                EnviarCorreo(ofertaSubasta.CorreoOfertador, body1);
                EnviarCorreo(ofertaSubasta.CorreoSubastador, body2);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            else
            {
                return HttpNotFound();
            }
        }

            // GET: Subastas2/Edit/5
        public ActionResult Edit(int? idProducto, string correo, DateTime fecha)
        {
            if (idProducto == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subasta subasta = db.Subastas.Find(idProducto, correo, fecha);
            if (subasta == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductoIDSubastado = new SelectList(db.Productoes, "ProductoID", "Nombre", subasta.ProductoIDSubastado);
            return View(subasta);
        }

        // POST: Subastas2/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductoIDSubastado,CorreoSubastador,FechaPublicado,PrecioMinimo,FechaInicio,FechaFin,Calificacion")] Subasta subasta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subasta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductoIDSubastado = new SelectList(db.Productoes, "ProductoID", "Nombre", subasta.ProductoIDSubastado);
            return View(subasta);
        }

        // GET: Subastas2/Delete/5
        public ActionResult Delete(int? idProducto, string correo, DateTime fecha)
        {
            if (idProducto == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subasta subasta = db.Subastas.Find( idProducto, correo,fecha);
            if (subasta == null)
            {
                return HttpNotFound();
            }
            return View(subasta);
        }

        // POST: Subastas2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? idProducto, string correo, DateTime fecha)
        {
            var cookiecorreo = ControllerContext.HttpContext.Request.Cookies["user"];
            Subasta subasta = db.Subastas.Find(idProducto, correo, fecha);
            subasta.Estado = false;
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult ListaProductos(string searchBy, string currentFilter, string searchString, int? page)
        {
            //var cookiecorreo = ControllerContext.HttpContext.Request.Cookies["user"];
            string correoActual = (string)HttpContext.Session["Correiro"];
            ViewBag.CurrentSort = searchBy;
            ViewBag.NameSortParm = String.IsNullOrEmpty(searchBy) ? "Name desc" : "";
            ViewBag.SortPriceParm = searchBy == "Price" ? "Price desc" : "Price";

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
                case "Name desc":
                    productos = productos.OrderByDescending(p => p.Nombre);
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
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            return View(productos.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult EnviarCorreo(string correo, string bodyMessage)
        {
            var fromAddress = new MailAddress("truequesecciucr@gmail.com", "Trueques UCR");
            var toAddress = new MailAddress(correo, "Estimado Usuario");
            const string fromPassword = "Admin.9876";
            const string subject = "Ha recibido una nueva oferta por la subasta";
            string body = bodyMessage;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),

                Timeout = 20000
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
            return View();
        }

    }
}
