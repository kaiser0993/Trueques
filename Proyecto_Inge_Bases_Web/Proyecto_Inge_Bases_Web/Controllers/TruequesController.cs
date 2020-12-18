using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto_Inge_Bases_Web.Models;

namespace Proyecto_Inge_Bases_Web.Controllers
{
    public class TruequesController : Controller
    {
        private TempPIEntities db = new TempPIEntities();

        // GET: TruequesDummy
        public ActionResult Index()
        {
            var trueques = db.Trueques.Include(t => t.Producto).Include(t => t.Producto1);
            return View(trueques.ToList());
        }

        // GET: TruequesDummy/Details/5
        public ActionResult Details(int? p1, string c1, int p2, string c2)
        {
            if (p1 == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trueque trueque = db.Trueques.Find(p1, c1, p2, c2);
            if (trueque == null)
            {
                return HttpNotFound();
            }
            return View(trueque);
        }

        // GET: TruequesDummy/Create
        public ActionResult Create()
        {

            ViewBag.ProductoIDOfertante = new SelectList(db.Productoes, "ProductoID", "ProductoID");
            ViewBag.ProductoIDPublicador = new SelectList(db.Productoes, "ProductoID", "ProductoID");
            ViewBag.CorreoOfertante = new SelectList(db.Productoes, "CorreoCliente", "CorreoCliente");
            ViewBag.CorreoPublicador = new SelectList(db.Productoes, "CorreoCliente", "CorreoCliente");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Trueque trueque, int productoIDPublicado, string correoPublicado, int productoIDOfertado, string correoOfertante, DateTime date)
        {
            trueque.Inserte(productoIDPublicado, correoPublicado, productoIDOfertado, correoOfertante, date);
            return View(trueque);
        }

        // POST: TruequesDummy/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductoIDPublicador,CorreoPublicador,ProductoIDOfertante,CorreoOfertante,FechaInicio,Mensaje,Finalizado,Calificacion")] Trueque trueque, Producto publicacion, Producto oferta)
        {
            if (ModelState.IsValid)
            {
                trueque.CreeTrueque(publicacion, oferta);
            }

            ViewBag.ProductoIDOfertante = new SelectList(db.Productoes, "ProductoID", "ProductoID", publicacion.ProductoID);
            ViewBag.ProductoIDPublicador = new SelectList(db.Productoes, "ProductoID", "ProductoID", oferta.ProductoID);
            ViewBag.CorreoOfertante = new SelectList(db.Productoes, "CorreoCliente", "CorreoCliente", publicacion.CorreoCliente);
            ViewBag.CorreoPublicador = new SelectList(db.Productoes, "CorreoCliente", "CorreoCliente", oferta.CorreoCliente);
            return View(trueque);
        }

        // GET: TruequesDummy/Edit/5
        public ActionResult Edit(int? p1, string c1, int p2, string c2)
        {
            if (p1 == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trueque trueque = db.Trueques.Find(p1, c1, p2, c2);
            if (trueque == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductoIDOfertante = new SelectList(db.Productoes, "ProductoID", "ProductoID", trueque.ProductoIDOfertante);
            ViewBag.ProductoIDPublicador = new SelectList(db.Productoes, "ProductoID", "ProductoID", trueque.ProductoIDPublicador);
            ViewBag.CorreoOfertante = new SelectList(db.Productoes, "CorreoCliente", "CorreoCliente", trueque.CorreoOfertante);
            ViewBag.CorreoPublicador = new SelectList(db.Productoes, "CorreoCliente", "CorreoCliente", trueque.CorreoPublicador);
            return View(trueque);
        }

        // POST: TruequesDummy/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductoIDPublicador,CorreoPublicador,ProductoIDOfertante,CorreoOfertante,FechaInicio,Mensaje,Finalizado,Calificacion")] Trueque trueque)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trueque).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductoIDOfertante = new SelectList(db.Productoes, "ProductoID", "ProductoID", trueque.ProductoIDOfertante);
            ViewBag.ProductoIDPublicador = new SelectList(db.Productoes, "ProductoID", "ProductoID", trueque.ProductoIDPublicador);
            ViewBag.CorreoOfertante = new SelectList(db.Productoes, "CorreoCliente", "CorreoCliente", trueque.CorreoOfertante);
            ViewBag.CorreoPublicador = new SelectList(db.Productoes, "CorreoCliente", "CorreoCliente", trueque.CorreoPublicador);
            return View(trueque);
        }

        // GET: TruequesDummy/Delete/5
        public ActionResult Delete(int? p1, string c1, int p2, string c2)
        {
            if (p1 == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trueque trueque = db.Trueques.Find(p1, c1, p2, c2);
            if (trueque == null)
            {
                return HttpNotFound();
            }
            return View(trueque);
        }

        // POST: TruequesDummy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int p1, string c1, int p2, string c2)
        {
            Trueque trueque = db.Trueques.Find(p1, c1, p2, c2);
            db.Trueques.Remove(trueque);
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