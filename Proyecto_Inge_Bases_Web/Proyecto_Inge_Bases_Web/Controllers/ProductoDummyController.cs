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
    public class ProductoDummyController : Controller
    {
        private TempPIEntities db = new TempPIEntities();

        // GET: ProductoDummy
        public ActionResult Index()
        {
            var productoes = db.Productoes.Include(p => p.Cliente);
            return View(productoes.ToList());
        }

        // GET: ProductoDummy/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productoes.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // GET: ProductoDummy/Create
        public ActionResult Create()
        {
            ViewBag.CorreoCliente = new SelectList(db.Clientes, "Correo", "Pais");
            return View();
        }

        // POST: ProductoDummy/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductoID,CorreoCliente,Nombre,PrecioEstimado,Condicion,Descripcion,NombreImagen,PathImagen,Publicado,FechaRegistrado,FechaPublicado,Calificacion")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                db.Productoes.Add(producto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CorreoCliente = new SelectList(db.Clientes, "Correo", "Pais", producto.CorreoCliente);
            return View(producto);
        }

        // GET: ProductoDummy/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productoes.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            ViewBag.CorreoCliente = new SelectList(db.Clientes, "Correo", "Pais", producto.CorreoCliente);
            return View(producto);
        }

        // POST: ProductoDummy/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductoID,CorreoCliente,Nombre,PrecioEstimado,Condicion,Descripcion,NombreImagen,PathImagen,Publicado,FechaRegistrado,FechaPublicado,Calificacion")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(producto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CorreoCliente = new SelectList(db.Clientes, "Correo", "Pais", producto.CorreoCliente);
            return View(producto);
        }

        // GET: ProductoDummy/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productoes.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: ProductoDummy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Producto producto = db.Productoes.Find(id);
            db.Productoes.Remove(producto);
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
