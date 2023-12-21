using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class ReviewuriController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<Utilizator> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ReviewuriController(
        ApplicationDbContext context,
        UserManager<Utilizator> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
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

        public ActionResult Show(string id_utilizator, int id_produs)
        {
            Review review = db.Reviewuri.Find(id_utilizator, id_produs);
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

        public ActionResult Edit(string id_utilizator, int id_produs)
        {
            Review review = db.Reviewuri.Find(id_utilizator, id_produs);
            return View(review);
        }

        [HttpPost]
        public ActionResult Edit(string id_utilizator, int id_produs, Review reqReview)
        {
            Review review = db.Reviewuri.Find(id_utilizator, id_produs);
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
        public ActionResult Delete(string id_utilizator, int id_produs)
        {
            Review review = db.Reviewuri.Find(id_utilizator, id_produs);
            db.Reviewuri.Remove(review);
            TempData["message"] = "Review-ul a fost sters!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
