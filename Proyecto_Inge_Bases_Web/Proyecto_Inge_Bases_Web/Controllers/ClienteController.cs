using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Antlr.Runtime;
using ICSharpCode.AvalonEdit.Utils;
using System.Dynamic;
using Proyecto_Inge_Bases_Web.Models;
using Newtonsoft.Json;
using System.Data.Entity.Core.Mapping;

namespace Proyecto_Inge_Bases_Web.Controllers
{
  
    public class ClienteController : Controller
    {
        private TempPIEntities db = new TempPIEntities();

        // GET: Cliente
        public ActionResult Index()
        {
            var clientes = db.Clientes.Include(c => c.Registrado);
            return View(clientes.ToList());
        }

        public ActionResult ListaDeAmigosSubListas()
        {
            var categoria = db.Categorias.Where(p => p.CategoriaID != 0)
            .Select(i => i.Nombre)
            .Distinct();
            ViewBag.Categoria = new SelectList(categoria);
            return View();
        }

        //Método para usar en imagen
        public byte[] ConvertToByte(HttpPostedFileBase file2)
        {
            byte[] imageByte = null;
            BinaryReader rdr = new BinaryReader(file2.InputStream);
            imageByte = rdr.ReadBytes((int)file2.ContentLength);
            return imageByte;
        }

        // Guarda datos del user
        [HttpPost]
        public ActionResult EdicionPerfil(FormCollection form)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            string format = "MM/dd/yyyy";
            string nombre = form["Nombre"].ToString();
            string apellido1 = form["Apellido1"].ToString();
            string apellido2 = form["Apellido2"].ToString();
            string edad = form["Edad"].ToString();
            string direccion = form["Direccion"].ToString();
            DateTime edad2 = DateTime.ParseExact(edad, format, provider);
            // Actividad supervisada , ID de historia = OC- 1.6 , Daniel Sancho B66676, Joshua Ramirez, Se lee la categoría seleccionada(Aún solo lee una) y se procede a guardar en la base 
            // de datos(Hay que hacer algunas modificaciones en la base de datos por lo tanto no se logra guardar aún, solo captarla.)
            var categoria = form["Categoria"]; // Se lee la categoria desde la vista
            Registrado usuario = db.Registradoes.Find(HttpContext.Session["Correiro"]);
            Cliente cliente1 = db.Clientes.Find(HttpContext.Session["Correiro"]);
            //Actividad Supervisada Pair programming, HISTORIA= OC-1.6, DANIEL SANCHO Y JOSHUA RAMIREZ, se obtiene la info del canton de la vista y se guarda en la base de datos
            //se define la variable canton para guardar los datos obtenidos del form
            var canton = form["Canton"];
            // se hace una consulta a la base de datos para comparar el nombre guardado la variable con el nombre del canton en la base
            Canton canton1 = db.Cantons.SingleOrDefault(cantonx => cantonx.Nombre == canton);
            // se guarda el canton id en la base de datos del cliente actual
            cliente1.Canton = canton1.id;

            HttpPostedFileBase file = Request.Files["file2"];//Para leer imagen de vista
            byte[] imagen7 = ConvertToByte(file);
            if (imagen7.Length == 0)//if para que no se guarde una imagen vacia despues de que el usuario ya ingresó una imagen o no ha ingresado y decide ingresar de nuevo a la página de editar perfil
            { }
            else
            { cliente1.FotoPerfil = imagen7; }

            string checkResp = form["checkbox1"]; //lee el valor del checkbox
            bool checkRespB = Convert.ToBoolean(checkResp);
            cliente1.BloquearNotificaciones = checkRespB;// Guarda en base true si el user checkea la opción de no resivir notif



            //Aqui empieza el código donde se guardan las categorías que elija el user.
            string checkResp3 = form["Entretenimiento"]; //lee el valor del checkbox
            bool checkRespB3 = Convert.ToBoolean(checkResp3);
            string checkResp4 = form["Deportes"]; //lee el valor del checkbox
            bool checkRespB4 = Convert.ToBoolean(checkResp4);

            //Aqui termina el código categorias

            cliente1.FechaNacimiento = edad2;
            cliente1.DireccionExacta = direccion;
            usuario.Nombre = nombre;
            usuario.Apellido1 = apellido1;
            usuario.Apellido2 = apellido2;
            db.SaveChanges();
            @Session["Username"] = nombre;// Ver para que servia no recuerdo
            return RedirectToAction("PerfilCompleto", "Registrado");
        }

        public ActionResult EditarPerfil(string ChangeCantonId)
        {
            Cliente cliente1 = db.Clientes.Find(HttpContext.Session["Correiro"]);
            Registrado registrado2 = db.Registradoes.Find(HttpContext.Session["Correiro"]);

            if (cliente1.Canton != null)
            {
                var cantonid = cliente1.Canton;
                Canton cantonx = db.Cantons.Find(cantonid);
                var cantoname = cantonx.Nombre;
                var cantonprovincia = cantonx.NombreProvincia;

                var pais = db.Pais.Where(p => p.Nombre != null)
                  .Select(i => i.Nombre);
                ViewBag.Pais = new SelectList(pais);

                var provincia = db.Provincias.Where(p => p.Nombre == cantonprovincia).Concat(db.Provincias.Where(p => p.Nombre != cantonprovincia))// Filtrar provincia del canton seleccionado y mostrar las demás sin repetir
                    .Select(i => i.Nombre);
                ViewBag.Provincia = new SelectList(provincia);

                var canton = db.Cantons.Where(p => p.Nombre == cantoname).Concat(db.Cantons.Where(p => (p.NombreProvincia == cantonprovincia && p.Nombre != cantoname)))// FIltrar canton seleccionado y mostrar las demás sin repetir.
                    .Select(i => i.Nombre);
                ViewBag.Canton = new SelectList(canton);
            }
            else
            { // Muestra ubicaciones para usuario recien registrado ...
                var pais = db.Pais.Where(p => p.Nombre != null)
                   .Select(i => i.Nombre);
                ViewBag.Pais = new SelectList(pais);

                var provincia = db.Provincias.Where(p => p.Nombre != null)
                    .Select(i => i.Nombre);
                ViewBag.Provincia = new SelectList(provincia);

                var canton = db.Cantons.Where(p => p.NombreProvincia == "Alajuela")
                    .Select(i => i.Nombre);
                ViewBag.Canton = new SelectList(canton);

            }

            var categoria = db.Categorias.Where(p => p.CategoriaID != 0)
              .Select(i => i.Nombre)
              .Distinct();
            ViewBag.Categoria = new SelectList(categoria);

            // Aqui se muestran los datos del usuario en págnia de perifl
            //Este if es para poder mostrar la imagen del perfil de usuario, si el user no ha cargado imagen que se cargue la que está por default.
            if (cliente1.FotoPerfil == null)
            {
                byte[] image = new WebClient().DownloadData("https://ucrindex.ucr.ac.cr/wp-content/plugins/all-in-one-seo-pack/images/default-user-image.png");
                var base64Image = Convert.ToBase64String(image);
                TempData["img"] = base64Image;
            }
            else
            {
                var base64Image = Convert.ToBase64String(cliente1.FotoPerfil);
                TempData["img"] = base64Image;
            }

            // Para checkear box en vista si ya fue checkeadoa nteriormente
            if (cliente1.BloquearNotificaciones == true)
            {
                TempData["checked"] = "checked";
            }
            else
            {
                TempData["checked"] = "";
            }

            TempData["NombreUsuario"] = registrado2.Nombre;
            TempData["Apellido1"] = registrado2.Apellido1;
            TempData["Apellido2"] = registrado2.Apellido2;
            TempData["Direccion"] = cliente1.DireccionExacta;
            TempData["Notificaciones"] = cliente1.BloquearNotificaciones; // ....................
            string edadx = cliente1.FechaNacimiento.ToString();
            if (edadx != "")
            {
                string edadtemp1 = "";
                string edadtemp2 = "";
                string edadtemp3 = "";
                string edadfinal = "";
                int x = edadx.Length;
                if (edadx.Length == 18)
                {
                    if (edadx.Substring(1, 1) == "/")
                    {
                        edadx = "0" + edadx;
                    }
                    else
                    {
                        edadtemp1 = edadx.Substring(0, 2);
                        edadtemp2 = edadx.Substring(3, 1);
                        edadtemp2 = "0" + edadtemp2;
                        edadtemp3 = edadx.Substring(4, edadx.Length - 4);
                        edadx = edadtemp1 + "/" + edadtemp2 + edadtemp3;
                    }
                }
                else if (edadx.Length == 17)
                {
                    edadx = "0" + edadx;
                    edadtemp1 = edadx.Substring(0, 2);
                    edadtemp2 = edadx.Substring(3, 1);
                    edadtemp2 = "0" + edadtemp2;
                    edadtemp3 = edadx.Substring(4, edadx.Length - 4);
                    edadx = edadtemp1 + "/" + edadtemp2 + edadtemp3;
                }
                edadtemp1 = edadx.Substring(0, 2);
                edadtemp2 = edadx.Substring(3, 2);
                edadtemp3 = edadx.Substring(5, edadx.Length - 5);
                edadfinal = edadtemp2 + "/" + edadtemp1 + edadtemp3;

                TempData["Edad"] = edadfinal;
                return View();
            }
            else
            {
                return View();
            }
        }

        public ActionResult GetProvincia(string nombrePais)
        {
            var provincia = db.Provincias.Where(p => p.NombrePais == nombrePais.Trim())
            .Select(i => i.Nombre);
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (string pr in provincia)
            {
                items.Add(new SelectListItem() { Text = pr, Value = pr });
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCanton(string nombreProvincia)
        {
            var canton = db.Cantons.Where(p => p.NombreProvincia == nombreProvincia.Trim())
            .Select(i => i.Nombre);
            List<SelectListItem> items = new List<SelectListItem>();
            Console.WriteLine(canton);
            Console.WriteLine(nombreProvincia);

            foreach (string c in canton)
            {
                Console.WriteLine(c);
                items.Add(new SelectListItem() { Text = c, Value = c });
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // Método que se ejecuta cuando se registra el usuario, activa la cuenta y retorna vista.
        public ActionResult EditarPerfilRegistrandose()
        {
            // RegistradoController cliente = new RegistradoController();
            Registrado usuario = db.Registradoes.Find(HttpContext.Session["Username2"]);
            usuario.EstadoCuenta = true;
            db.SaveChanges();
            //cliente.Login(usuario);
            return RedirectToAction("IniciarSesion", "Registrado");
        }
        public ActionResult PerfilCliente()
        {

            return View();
        }

        public ActionResult ListaAmigos()
        {

            var correoU = HttpContext.Session["Correiro"];
            string correoUser = correoU.ToString();
           List<VistaAmigo> lista_A = db.VistaAmigos.Where(c=>c.CorreoDueno==correoUser).ToList();
            List<VistaNoAmigo> lista_B = db.VistaNoAmigoes.Where(c => c.dueno == correoUser).Distinct().ToList();

            MostrarListas listas = new MostrarListas();
            listas.Amigos = lista_A;
            listas.NoAmigos = lista_B;


            return View(listas);
        }

        /* public PartialViewResult Amigos()
         {
             return PartialView(db.ListaContieneClienteRegistadoes.ToList());
         }

         public PartialViewResult NoAmigos()
         {
             return PartialView(db.ListaNoAmigos.ToList());
         }
         */

        // GET: Cliente/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // GET: Cliente/Create
        public ActionResult Create()
        {
            ViewBag.Correo = new SelectList(db.Registradoes, "Correo", "Contrasena");
            return View();
        }

        // POST: Cliente/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Correo,FechaNacimiento,Pais,Provincia,Canton,DireccionExacta,FotoPerfil,EstadoCuenta,FechaCierre,CalificacionPromedio,ClienteID")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                db.Clientes.Add(cliente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Correo = new SelectList(db.Registradoes, "Correo", "Contrasena", cliente.Correo);
            return View(cliente);
        }

        // GET: Cliente/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            ViewBag.Correo = new SelectList(db.Registradoes, "Correo", "Contrasena", cliente.Correo);
            return View(cliente);
        }

        // POST: Cliente/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Correo,FechaNacimiento,Pais,Provincia,Canton,DireccionExacta,FotoPerfil,EstadoCuenta,FechaCierre,CalificacionPromedio,ClienteID")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Correo = new SelectList(db.Registradoes, "Correo", "Contrasena", cliente.Correo);
            return View(cliente);
        }

        // GET: Cliente/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Cliente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Cliente cliente = db.Clientes.Find(id);
            db.Clientes.Remove(cliente);
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
    }
}
