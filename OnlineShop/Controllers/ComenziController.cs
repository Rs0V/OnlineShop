using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class ComenziController : Controller
    {
        private readonly ApplicationDbContext db;

        public ComenziController(ApplicationDbContext context)
        {
            db = context;
        }

        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
                ViewBag.message = TempData["message"].ToString();

            var comenzi = from comanda in db.Comenzi
                          orderby comanda.Data descending
                          select comanda;

            ViewBag.Comenzi = comenzi;
            return View();
        }

        public ActionResult Show(int id)
        {
            Comanda comenzi = db.Comenzi.Find(id);
            return View(comenzi);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Comanda comanda)
        {
            if (ModelState.IsValid)
            {
                db.Comenzi.Add(comanda);
                db.SaveChanges();
                TempData["message"] = "Comanda a fost adaugata!";
                return RedirectToAction("Index");
            }
            return View(comanda);
        }

        public ActionResult Edit(int id)
        {
            Comanda comanda = db.Comenzi.Find(id);
            return View(comanda);
        }

        [HttpPost]
        public ActionResult Edit(int id, Comanda reqComanda)
        {
            Comanda comanda = db.Comenzi.Find(id);
            if (ModelState.IsValid)
            {
                comanda.Data = reqComanda.Data;
                comanda.Stare = reqComanda.Stare;
                comanda.Valoare = reqComanda.Valoare;
                comanda.UtilizatorId = reqComanda.UtilizatorId;

                db.SaveChanges();
                TempData["message"] = "Comanda a fost modificata!";
                return RedirectToAction("Index");
            }
            return View(reqComanda);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Comanda comanda = db.Comenzi.Find(id);
            db.Comenzi.Remove(comanda);
            TempData["message"] = "Comanda a fost stearsa!";
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
