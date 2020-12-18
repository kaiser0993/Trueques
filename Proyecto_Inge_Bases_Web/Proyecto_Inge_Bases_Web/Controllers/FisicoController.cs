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
    public class FisicoController : Controller
    {
        private TempPIEntities db = new TempPIEntities();

        // GET: Fisico
        public ActionResult Index()
        {
            var fisicoes = db.Fisicoes.Include(f => f.Producto);
            return View(fisicoes.ToList());
        }

        // GET: Fisico/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fisico fisico = db.Fisicoes.Find(id);
            if (fisico == null)
            {
                return HttpNotFound();
            }
            return View(fisico);
        }

        // GET: Fisico/Create
        public ActionResult Create()
        {
            ViewBag.FisicoID = new SelectList(db.Productoes, "ProductoID", "Nombre");
            return View();
        }

        // POST: Fisico/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FisicoID")] Fisico fisico)
        {
            if (ModelState.IsValid)
            {
                db.Fisicoes.Add(fisico);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FisicoID = new SelectList(db.Productoes, "ProductoID", "Nombre", fisico.ProductoID);
            return View(fisico);
        }

        // GET: Fisico/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fisico fisico = db.Fisicoes.Find(id);
            if (fisico == null)
            {
                return HttpNotFound();
            }
            ViewBag.FisicoID = new SelectList(db.Productoes, "ProductoID", "Nombre", fisico.ProductoID);
            return View(fisico);
        }

        // POST: Fisico/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FisicoID")] Fisico fisico)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fisico).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FisicoID = new SelectList(db.Productoes, "ProductoID", "Nombre", fisico.ProductoID);
            return View(fisico);
        }

        // GET: Fisico/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fisico fisico = db.Fisicoes.Find(id);
            if (fisico == null)
            {
                return HttpNotFound();
            }
            return View(fisico);
        }

        // POST: Fisico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Fisico fisico = db.Fisicoes.Find(id);
            db.Fisicoes.Remove(fisico);
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
