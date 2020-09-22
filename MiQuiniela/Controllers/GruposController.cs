using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiQuiniela.Models;

namespace MiQuiniela.Controllers
{
    public class GruposController : Controller
    {
        private QuinielaEntities db = new QuinielaEntities();

        //
        // GET: /Grupos/

        public ActionResult Index()
        {
            var grupos = db.Grupos.Include(g => g.Usuario);
            return View(grupos.ToList());
        }

        //
        // GET: /Grupos/Details/5

        public ActionResult Details(int id = 0)
        {
            Grupos grupos = db.Grupos.Find(id);
            if (grupos == null)
            {
                return HttpNotFound();
            }
            return View(grupos);
        }

        //
        // GET: /Grupos/Create

        public ActionResult Create()
        {
            //ViewBag.ID_Usuario = new SelectList(db.Usuarios, "ID_Usuario", "Nickname");

            return View();
        }

        //
        // POST: /Grupos/Create

        [HttpPost]
        public ActionResult Create(Grupos grupos)
        {
            if (ModelState.IsValid)
            {
                if (db.Grupos.Count() > 0)
                    grupos.ID_Grupo = db.Grupos.OrderByDescending(g => g.ID_Grupo).FirstOrDefault().ID_Grupo + 1;
                else
                    grupos.ID_Grupo = 1;
                grupos.ID_Usuario = Int32.Parse(TempData["ID_Usuario"].ToString());
                grupos.FechaIns = DateTime.Now;
                db.Grupos.Add(grupos);
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }

            ViewBag.ID_Usuario = new SelectList(db.Usuarios, "ID_Usuario", "Nickname", grupos.ID_Usuario);
            return View(grupos);
        }

        //
        // GET: /Grupos/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Grupos grupos = db.Grupos.Find(id);
            if (grupos == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Usuario = new SelectList(db.Usuarios, "ID_Usuario", "Nickname", grupos.ID_Usuario);
            return View(grupos);
        }

        //
        // POST: /Grupos/Edit/5

        [HttpPost]
        public ActionResult Edit(Grupos grupos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grupos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Usuario = new SelectList(db.Usuarios, "ID_Usuario", "Nickname", grupos.ID_Usuario);
            return View(grupos);
        }

        //
        // GET: /Grupos/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Grupos grupos = db.Grupos.Find(id);
            if (grupos == null)
            {
                return HttpNotFound();
            }
            return View(grupos);
        }

        //
        // POST: /Grupos/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Grupos grupos = db.Grupos.Find(id);
            db.Grupos.Remove(grupos);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}