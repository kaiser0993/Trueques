using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto_Inge_Bases_Web.Models;

namespace Proyecto_Inge_Bases_Web.Controllers
{
    public class SubastasController : Controller
    {
        private TempPIEntities db = new TempPIEntities();

        // GET: Subastas
        public ActionResult Index()
        {
            return View(db.Subastas.ToList());
        }

        // GET: Subastas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subasta subasta = db.Subastas.Find(id);
            if (subasta == null)
            {
                return HttpNotFound();
            }
            return View(subasta);
        }

        // GET: Subastas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Subastas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Puede que se necesiten recibir parámetros para guardar las fechas de Inicio y Fin de la subasta ya que en el Create se guardan desde el calendario de Bootstrap
        public ActionResult Create([Bind(Include = "Id,FechaInicio,FechaFin,Calificacion")] Subasta subasta, FormCollection form)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            string format = "dd/mm/yyyy hh tt";
            string FechaIForm = form["FechaI"].ToString();
            string FechaFForm = form["FechaF"].ToString();
            DateTime FechaI = DateTime.ParseExact(FechaIForm, format, provider);
            DateTime FechaF = DateTime.ParseExact(FechaFForm, format, provider);

            subasta.FechaInicio = FechaI;
            subasta.FechaFin = FechaF;

            if (ModelState.IsValid)
            {
                db.Subastas.Add(subasta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(subasta);
        }

        // GET: Subastas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subasta subasta = db.Subastas.Find(id);
            if (subasta == null)
            {
                return HttpNotFound();
            }
            return View(subasta);
        }

        // POST: Subastas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FechaInicio,FechaFin,Calificacion")] Subasta subasta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subasta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(subasta);
        }

        // GET: Subastas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subasta subasta = db.Subastas.Find(id);
            if (subasta == null)
            {
                return HttpNotFound();
            }
            return View(subasta);
        }

        // POST: Subastas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Subasta subasta = db.Subastas.Find(id);
            db.Subastas.Remove(subasta);
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
