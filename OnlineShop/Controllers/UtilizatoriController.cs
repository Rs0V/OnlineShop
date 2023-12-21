using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;
using System;

namespace OnlineShop.Controllers
{
    public class UtilizatoriController : Controller
    {
        private readonly ApplicationDbContext db;

        public UtilizatoriController(ApplicationDbContext context)
        {
            db = context;
        }

        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
                ViewBag.message = TempData["message"].ToString();

            var utilizatori = from utilizator in db.Utilizatori
                              orderby utilizator.Nume, utilizator.Prenume
                              select utilizator;

            ViewBag.Utilizatori = utilizatori;
            return View();
        }

        public ActionResult Show(int id)
        {
            Utilizator utilizatori = db.Utilizatori.Find(id);
            return View(utilizatori);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Utilizator utilizator)
        {
            if (ModelState.IsValid)
            {
                db.Utilizatori.Add(utilizator);
                db.SaveChanges();
                TempData["message"] = "Utilizatorul a fost inregistrat";
                return RedirectToAction("Index");
            }
            return View(utilizator);
        }

        public ActionResult Edit(int id)
        {
            Utilizator utilizator = db.Utilizatori.Find(id);
            return View(utilizator);
        }

        [HttpPost]
        public ActionResult Edit(int id, Utilizator reqUtilizator)
        {
            Utilizator utilizator = db.Utilizatori.Find(id);
            if (ModelState.IsValid)
            {
                // utilizator.Tip = reqUtilizator.Tip;
                utilizator.Nume = reqUtilizator.Nume;
                utilizator.Prenume = reqUtilizator.Prenume;
                utilizator.Email = reqUtilizator.Email;
                utilizator.Telefon = reqUtilizator.Telefon;
                utilizator.Parola = reqUtilizator.Parola;

                db.SaveChanges();
                TempData["message"] = "Utilizatorul a fost modificat!";
                return RedirectToAction("Index");
            }
            return View(reqUtilizator);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Utilizator utilizator = db.Utilizatori.Find(id);
            db.Utilizatori.Remove(utilizator);
            TempData["message"] = "Utilizatorul a fost sters!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
