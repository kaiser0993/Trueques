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
    public class VirtualController : Controller
    {
        private TempPIEntities db = new TempPIEntities();

        // GET: Virtual
        public ActionResult Index()
        {
            var virtuals = db.Virtuals.Include(c => c.Producto);
            return View(virtuals.ToList());
        }

        // GET: Virtual/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Virtual @virtual = db.Virtuals.Find(id);
            if (@virtual == null)
            {
                return HttpNotFound();
            }
            return View(@virtual);
        }

        // GET: Virtual/Create
        public ActionResult Create()
        {
            ViewBag.VirtualID = new SelectList(db.Productoes, "ProductoID", "Nombre");
            return View();
        }

        // POST: Virtual/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VirtualID,TipoDeArchivo,DerechosDeProducto,Fuentes,FechaExpiracion")] Virtual @virtual)
        {
            if (ModelState.IsValid)
            {
                db.Virtuals.Add(@virtual);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.VirtualID = new SelectList(db.Productoes, "ProductoID", "Nombre", @virtual.ProductoID);
            return View(@virtual);
        }

        // GET: Virtual/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Virtual @virtual = db.Virtuals.Find(id);
            if (@virtual == null)
            {
                return HttpNotFound();
            }
            ViewBag.VirtualID = new SelectList(db.Productoes, "ProductoID", "Nombre", @virtual.ProductoID);
            return View(@virtual);
        }

        // POST: Virtual/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VirtualID,TipoDeArchivo,DerechosDeProducto,Fuentes,FechaExpiracion")] Virtual @virtual)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@virtual).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.VirtualID = new SelectList(db.Productoes, "ProductoID", "Nombre", @virtual.ProductoID);
            return View(@virtual);
        }

        // GET: Virtual/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Virtual @virtual = db.Virtuals.Find(id);
            if (@virtual == null)
            {
                return HttpNotFound();
            }
            return View(@virtual);
        }

        // POST: Virtual/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Virtual @virtual = db.Virtuals.Find(id);
            db.Virtuals.Remove(@virtual);
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
