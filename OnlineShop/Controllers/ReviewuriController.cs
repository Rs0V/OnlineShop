using Microsoft.AspNetCore.Authorization;
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

		public ActionResult Index(object? id_bonus, string? tip_idb)
		{
			if (TempData.ContainsKey("message"))
				ViewBag.message = TempData["message"].ToString();

			var reviewuri = from review in db.Reviewuri
							join utilizator in db.Utilizatori on review.UtilizatorId equals utilizator.Id
							join produs in db.Produse on review.ProdusId equals produs.Id
							orderby produs.Titlu, produs.Rating descending
							select review;

			if (id_bonus != null)
			{
				if (tip_idb == null)
				{
					ViewBag.message = "Tipul id-ului cautat trebuie specificat";
					reviewuri = null;
				}
				else if (tip_idb == "produs")
				{
					reviewuri = from review in db.Reviewuri
								join utilizator in db.Utilizatori on review.UtilizatorId equals utilizator.Id
								join produs in db.Produse on review.ProdusId equals produs.Id
								where review.ProdusId == (int)id_bonus
								orderby produs.Titlu, produs.Rating descending
								select review;
				}
				else if (tip_idb == "utilizator")
				{
					reviewuri = from review in db.Reviewuri
								join utilizator in db.Utilizatori on review.UtilizatorId equals utilizator.Id
								join produs in db.Produse on review.ProdusId equals produs.Id
								where review.UtilizatorId == (string)id_bonus
								orderby produs.Titlu, produs.Rating descending
								select review;
				}
			}
			ViewBag.Reviewuri = reviewuri;
			return View();
		}

		public ActionResult Show(string id_utilizator, int id_produs)
		{
			Review? review = db.Reviewuri.Find(id_utilizator, id_produs);
			ViewBag.canedit = false;

			if (review == null)
			{
				TempData["message"] = "Reviewul cautat nu exista";
				return RedirectToAction("Index");
			}
			if (review.UtilizatorId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
                ViewBag.canedit = true;
            
			return View(review);
		}

		// Le specificam pe toate pt ca doar userii neautentificati nu au voie sa lase reviewuri
		[Authorize(Roles = "Utilizator,Colaborator,Administrator")]
		public ActionResult New()
		{
			return View();
		}

		[Authorize(Roles = "Utilizator,Colaborator,Administrator")]
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
			Review? review = db.Reviewuri.Find(id_utilizator, id_produs);

			if (review == null)
			{
				TempData["message"] = "Reviewul cautat nu exista";
				return RedirectToAction("Index");
			}
			if (review.UtilizatorId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
				return View(review);

            TempData["message"] = "Nu aveti dreptul sa editati un review care nu va apartine";
            return RedirectToAction("Index");
        }

		[HttpPost]
		public ActionResult Edit(string id_utilizator, int id_produs, Review reqReview)
		{
			Review? review = db.Reviewuri.Find(id_utilizator, id_produs);

			if (review == null)
			{
				TempData["message"] = "Reviewul cautat nu exista";
				return RedirectToAction("Index");
			}
            if (review.UtilizatorId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
			{
                if (ModelState.IsValid)
                {
                    review.Continut = reqReview.Continut;

                    db.SaveChanges();
                    TempData["message"] = "Review-ul a fost modificat!";
                    return RedirectToAction("Index");
                }
                return View(reqReview);
            }
            TempData["message"] = "Nu aveti dreptul sa editati un review care nu va apartine";
            return RedirectToAction("Index");
        }

		[HttpPost]
		public ActionResult Delete(string id_utilizator, int id_produs)
		{
			Review? review = db.Reviewuri.Find(id_utilizator, id_produs);

			if (review == null)
			{
				TempData["message"] = "Reviewul cautat nu exista";
				return RedirectToAction("Index");
			}
            if (review.UtilizatorId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
			{
                db.Reviewuri.Remove(review);
                TempData["message"] = "Review-ul a fost sters!";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            TempData["message"] = "Nu aveti dreptul sa stergeti un review care nu va apartine";
            return RedirectToAction("Index");
		}
	}
}
