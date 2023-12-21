using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class ReviewuriController : Controller
    {
        private readonly ApplicationDbContext db;

        public ReviewuriController(ApplicationDbContext context)
        {
            db = context;
        }

        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
                ViewBag.message = TempData["message"].ToString();

            var reviewuri = from review in db.Reviewuri
                            join utilizator in db.Utilizatori on review.UtilizatorId equals utilizator.Id
                            join produs in db.Produse on review.ProdusId equals produs.Id
                            orderby produs.Titlu, produs.Rating descending
                            select review;

            ViewBag.Reviewuri = reviewuri;
            return View();
        }

        public ActionResult Show(int id)
        {
            Review review = db.Reviewuri.Find(id);
            return View(review);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Review review)
        {
            if (ModelState.IsValid)
            {
                db.Reviewuri.Add(review);
                db.SaveChanges();
                TempData["message"] = "Review-ul a fost adaugat!";
                return RedirectToAction("Index");
            }
            return View(review);
        }

        public ActionResult Edit(int id)
        {
            Review review = db.Reviewuri.Find(id);
            return View(review);
        }

        [HttpPost]
        public ActionResult Edit(int id, Review reqReview)
        {
            Review review = db.Reviewuri.Find(id);
            if (ModelState.IsValid)
            {
                review.Continut = reqReview.Continut;

                db.SaveChanges();
                TempData["message"] = "Review-ul a fost modificat!";
                return RedirectToAction("Index");
            }
            return View(reqReview);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Review review = db.Reviewuri.Find(id);
            db.Reviewuri.Remove(review);
            TempData["message"] = "Review-ul a fost sters!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
