using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Proyecto_Inge_Bases_Web.Models;
using PagedList;
using System.Runtime.Remoting.Messaging;
using Proyecto_Inge_Bases_Web.Views.TruequeViewModels;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;

namespace Proyecto_Inge_Bases_Web.Controllers
{
   
    public class NotificationsController : Controller
    {
        private TempPIEntities db = new TempPIEntities();        

       /* public ActionResult IndexTruequeModels()
        {
            TruequeModels tmodel = new TruequeModels();
            var trueques = db.Trueques.Include(t => t.Producto).Include(t => t.Producto1); //Revisar si esto ya incluye los correos
            tmodel.Trueques = trueques.ToList();

            var products = db.Productoes.Include(c => c.Cliente);
            tmodel.Productos = products.ToList();

            return View(tmodel);

        }*/



        // GET: Trueque
        /**
            @Param: sortOrder. string que indica de que manera se va a ordenar la lista.
            @Param: currentFilter. Permite saber la configuracion de filtrado
            @Param: searchString. Recibe el string con lo que desea buscar.
            @Param: page. Permite saber cual es la pagina a mostrar
        */
        public ActionResult Index()
        {
            //Hola Maria
            // Marco Dice: Ahora se usa el session, y se redirige a Iniciar Sesion
            //var cookiecorreo = ControllerContext.HttpContext.Request.Cookies["user"];
            string correoActual = (string)HttpContext.Session["Correiro"];
           
            //Agarro los modelos para la vista con más de 1 modelo
            TruequeModels tmodel = new TruequeModels(); //Tiene la informacion del producto, y del trueque
            var trueques = db.Trueques.Include(t => t.Producto).Include(t => t.Producto1);
            var truequesB = from p in db.Trueques //Atributos relacionados a trueque
                            select p;



            if (correoActual != null) //Solo agarro los trueques donde yo soy publicador
            {
                truequesB = truequesB.Where(p => p.CorreoPublicador == correoActual);
            }
            else
            {
                return RedirectToAction("IniciarSesion", "Registrado"); // Me devuelve a registrarme
            }

      
            tmodel.Trueques = truequesB.ToList();
            //Agarro productos
            var products = db.Productoes.Include(c => c.Cliente);
            var productos = from ps in db.Productoes //Para agarrar el nombre de los productos y la imagen
                            select ps;
           

            tmodel.Productos = products.ToList();

            return View(tmodel);
        }

        public ActionResult Recibe(string searchBy, string currentFilter, string searchString, int? page)
        {
            var cookiecorreo = ControllerContext.HttpContext.Request.Cookies["user"];

            //   HttpCookie cookiecorreo = Request.Cookies["users"];

            ViewBag.CurrentSort = searchBy;
            ViewBag.NameSortParm = String.IsNullOrEmpty(searchBy) ? "Name asc" : "";
            ViewBag.SortPriceParm = searchBy == "Price" ? "Price desc" : "Price";

            //int uno;

            // Esto tiene que arreglarse, que venga de la rama de marco
            //ViewBag.CurrentSort = sortOrder;
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_asc" : "";

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

            // Es para que me salgan solo mis productos, (yo siendo el usuario logueado)
            if (cookiecorreo != null)
            {
                productos = productos.Where(p => p.CorreoCliente == cookiecorreo.Value);
            }
            else
            {
                // Arreglar esto return View("Index", "Home");
                //uno = 1;
                return View("Index", "Home");

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
     
        public ActionResult Solicitud(int p1)
        {
            string correoActual = (string)HttpContext.Session["Correiro"];

            int producto1 = p1;
            //Agarro los modelos para la vista con más de 1 modelo
            TruequeModels tmodel = new TruequeModels(); //Tiene la informacion del producto, y del trueque
            var trueques = db.Trueques.Include(t => t.Producto).Include(t => t.Producto1);
            var truequesB = from p in db.Trueques //Atributos relacionados a trueque
                            select p;
            var truequesC = from m in db.Trueques //Atributos relacionados a trueque
                            select m;


            if (correoActual != null) //Solo agarro los trueques donde yo soy publicador
            {
                truequesB = truequesB.Where(p => p.CorreoPublicador == correoActual && p.ProductoIDPublicador == p1);
            }
            else
            {
                // Arreglar esto return View("Index", "Home");
                return RedirectToAction("IniciarSesion", "Registrado");

            }

            tmodel.Trueques = truequesB.ToList();
            //Agarro productos
            var products = db.Productoes.Include(c => c.Cliente);
            var productos = from ps in db.Productoes //Para agarrar el nombre de los productos y la imagen
                            select ps;


            tmodel.Productos = products.ToList();

            //Agarro clientes 
            var clientes = db.Clientes.Include(c => c.Registrado);
            tmodel.Clientes = clientes.ToList();

            return View(tmodel);
        }


        public ActionResult DetallesProductosOfertados(int p1, string correo)
        {
            string correoActual = (string)HttpContext.Session["Correiro"];

            //int producto1 = p1;
            //Agarro los modelos para la vista con más de 1 modelo
            TruequeModels tmodel = new TruequeModels(); //Tiene la informacion del producto, y del trueque
            var trueques = db.Trueques.Include(t => t.Producto).Include(t => t.Producto1);
            var truequesB = from p in db.Trueques //Atributos relacionados a trueque
                            select p;
            var truequesC = from m in db.Trueques //Atributos relacionados a trueque
                            select m;


            if (correoActual != null) //Solo agarro los trueques donde yo soy publicador
            {
                truequesB = truequesB.Where(p => p.CorreoPublicador == correoActual && p.ProductoIDPublicador == p1 && p.CorreoOfertante == correo);
            }
            else
            {
                // Arreglar esto return View("Index", "Home");
                return RedirectToAction("IniciarSesion", "Registrado");

            }



            tmodel.Trueques = truequesB.ToList();
            //Agarro productos
            var products = db.Productoes.Include(c => c.Cliente);
            var productos = from ps in db.Productoes //Para agarrar el nombre de los productos y la imagen
                            select ps;


            tmodel.Productos = products.ToList();

            //Agarro clientes 
            var clientes = db.Clientes.Include(c => c.Registrado);
            tmodel.Clientes = clientes.ToList();

            return View(tmodel);
        }

        // GET: Notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trueque trueque = db.Trueques.Find(id);
            if (trueque == null)
            {
                return HttpNotFound();
            }
            return View(trueque);
        }

        // GET: Notifications/Create
        public ActionResult Create()
        {
            ViewBag.ProductoIDOfertante = new SelectList(db.Productoes, "ProductoID", "Nombre");
            ViewBag.ProductoIDPublicador = new SelectList(db.Productoes, "ProductoID", "Nombre");
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductoIDPublicador,CorreoPublicador,ProductoIDOfertante,CorreoOfertante,Mensaje,Finalizado,Calificacion")] Trueque trueque)
        {
            if (ModelState.IsValid)
            {
                db.Trueques.Add(trueque);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductoIDOfertante = new SelectList(db.Productoes, "ProductoID", "Nombre", trueque.ProductoIDOfertante);
            ViewBag.ProductoIDPublicador = new SelectList(db.Productoes, "ProductoID", "Nombre", trueque.ProductoIDPublicador);
            return View(trueque);
        }

        // GET: Notifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trueque trueque = db.Trueques.Find(id);
            if (trueque == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductoIDOfertante = new SelectList(db.Productoes, "ProductoID", "Nombre", trueque.ProductoIDOfertante);
            ViewBag.ProductoIDPublicador = new SelectList(db.Productoes, "ProductoID", "Nombre", trueque.ProductoIDPublicador);
            return View(trueque);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductoIDPublicador,CorreoPublicador,ProductoIDOfertante,CorreoOfertante,Mensaje,Finalizado,Calificacion")] Trueque trueque)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trueque).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductoIDOfertante = new SelectList(db.Productoes, "ProductoID", "Nombre", trueque.ProductoIDOfertante);
            ViewBag.ProductoIDPublicador = new SelectList(db.Productoes, "ProductoID", "Nombre", trueque.ProductoIDPublicador);
            return View(trueque);
        }

        // GET: Notifications/Delete/5
        public ActionResult Delete(int p1,  string correo)
        {
            string correoActual = (string)HttpContext.Session["Correiro"];
            
            TruequeModels tmodel = new TruequeModels(); //Tiene la informacion del producto, y del trueque
            
            var trueques = db.Trueques.Include(t => t.Producto).Include(t => t.Producto1);
            var truequesB = from p in db.Trueques //Atributos relacionados a trueque
                            select p;
            
            
            
            /*
            var productosA = from pA in db.Productoes
                            select pA;

            var productosB = from pB in db.Productoes
                             select pB ;*/

            

            if (correoActual != null) //Solo agarro los trueques donde yo soy publicador
            {
                truequesB = truequesB.Where(p => p.CorreoPublicador == correoActual && p.ProductoIDPublicador == p1 && p.CorreoOfertante == correo);
            }
            else
            {
                // Arreglar esto return View("Index", "Home");
                return RedirectToAction("IniciarSesion", "Registrado");
            }


            //truequesB.

            // OJO; SE TIENE QUE TENER CUIDADO, HAY QUE BUSCAQR EL ID EL SEGUNDO PRODUCTO PARA JALAR LOS NOMBRE DEL PRODUCTO
            // PREGUNTAR DONDE ES QUE SE TIENE ESE ID, O COMO SACARLO, LO COMENTADO SE CAE.


            //Trueque truequeActual = db.Trueques.Find(correoActual, p1, correo);
            //Producto productoOfertado = db.Productoes.Find(truequeActual.ProductoIDOfertante);
            //Producto productoPublicado = db.Productoes.Find(truequeActual.ProductoIDPublicador);

            var body1 = $"Truques@UCR informa: \n El usuario {correo} ha rechazado su oferta de truque que contiene los siguientes productos: \n *NULL \n *NULL";
            body1.Replace("\n", Environment.NewLine);
            var body2 = $"Truques@UCR informa: \n El usuario {correoActual} ha rechazado su oferta de truque que contiene los siguientes productos: \n *NULL \n *NULL";
            body2.Replace("\n", Environment.NewLine);
            
            EnviarCorreo(correoActual, body1);
            EnviarCorreo(correo, body2);

            //remueve todos los elementos
            db.Trueques.RemoveRange(truequesB);
            db.SaveChanges();
            return RedirectToAction("Recibe");
        }

 

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult MisTrueques()
        {
            string correoActual = (string)HttpContext.Session["Correiro"];

            //Agarro los modelos para la vista con más de 1 modelo
            TruequeModels tmodel = new TruequeModels(); //Tiene la informacion del producto, y del trueque
            var trueques = db.Trueques.Include(t => t.Producto).Include(t => t.Producto1);
            var truequesB = from p in db.Trueques //Atributos relacionados a trueque
                            select p;



            if (correoActual != null) //Solo agarro los trueques donde yo soy publicador
            {
                truequesB = truequesB.Where(p => p.CorreoPublicador == correoActual);
            }
            else
            {
                return RedirectToAction("IniciarSesion", "Registrado"); // Me devuelve a registrarme
            }


            tmodel.Trueques = truequesB.ToList();
            //Agarro productos
            var products = db.Productoes.Include(c => c.Cliente);
            var productos = from ps in db.Productoes //Para agarrar el nombre de los productos y la imagen
                            select ps;


            tmodel.Productos = products.ToList();

            return View(tmodel);
        }

        /* Acciones a Usuario  */
        public ActionResult VerSignUp_Usuario()
        {
            return RedirectToAction("SignUp", "Usuario");
        }
        /* -------------------------------*/


        /*Estar monitoreando este metodo tambien, el asunto del correo esta hecho solo para notificaar cuendo se elimine el truque, mejorarlo con un SWITCH -> el subject, 
         * el body viene del metodo */
        [HttpPost]
        public ActionResult EnviarCorreo(string correo, string bodyMessage)
        {
            var fromAddress = new MailAddress("truequesecciucr@gmail.com", "Trueques UCR");
            var toAddress = new MailAddress(correo, "Estimado Usuario");
            const string fromPassword = "Admin.9876";
            const string subject = "Su oferta ha sido rechazada";
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
