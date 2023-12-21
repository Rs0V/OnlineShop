using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Data;
using System.Data;

namespace OnlineShop.Controllers
{
    public class ExemplareController : Controller
    {
        private readonly ApplicationDbContext db;

        public ExemplareController(ApplicationDbContext context)
        {
            db = context;
        }

        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
                ViewBag.message = TempData["message"].ToString();

            var exemplare = from exemplar in db.Exemplare
                            join produs in db.Produse on exemplar.Id_Produs equals produs.Id
                            orderby produs.Titlu
                            select exemplar;

            ViewBag.Exemplare = exemplare;
            return View();
        }

        public ActionResult Show(int id)
        {
            Exemplar exemplar = db.Exemplare.Find(id);
            return View(exemplar);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Exemplar exemplar)
        {
            if (ModelState.IsValid)
            {
                db.Exemplare.Add(exemplar);
                db.SaveChanges();
                TempData["message"] = "Exemplarul a fost adaugat";
                return RedirectToAction("Index");
            }
            return View(exemplar);
        }

        public ActionResult Edit(int id)
        {
            Exemplar exemplar = db.Exemplare.Find(id);
            return View(exemplar);
        }

        [HttpPost]
        public ActionResult Edit(int id, Exemplar reqEx)
        {
            Exemplar exemplar = db.Exemplare.Find(id);
            if (ModelState.IsValid)
            {
                exemplar.Stare = reqEx.Stare;
                exemplar.Id_Comanda = reqEx.Id_Comanda;

                db.SaveChanges();
                TempData["message"] = "Exemplarul a fost modificat!";
                return RedirectToAction("Index");
            }
            return View(reqEx);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Exemplar exemplar = db.Exemplare.Find(id);
            db.Exemplare.Remove(exemplar);
            TempData["message"] = "Exemplarul a fost sters";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
