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
		public ReviewuriController(ApplicationDbContext context, UserManager<Utilizator> userManager, RoleManager<IdentityRole> roleManager)
		{
			db = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public ActionResult Index(string? id_filtru, string? tip_id)
		{
			if (TempData.ContainsKey("message"))
				ViewBag.message = TempData["message"].ToString();

			var reviewuri = from review in db.Reviewuri
							join utilizator in db.Utilizatori on review.UtilizatorId equals utilizator.Id
							join produs in db.Produse on review.ProdusId equals produs.Id
							orderby produs.Titlu, review.Rating descending, utilizator.Nume, utilizator.Prenume
							select review;


			if (id_filtru != null && tip_id != null)
			{
				if (tip_id == "produs")
					reviewuri = reviewuri.Where(r => r.ProdusId == int.Parse(id_filtru));
				else
					reviewuri = reviewuri.Where(r => r.UtilizatorId == id_filtru);
			}
		
			var utilizatori = from utilizator in db.Utilizatori
							  select utilizator;

			var produse = from produs in db.Produse
						  select produs;


			ViewBag.Reviewuri = reviewuri;
			ViewBag.Utilizatori = utilizatori;
			ViewBag.Produse = produse;
			ViewBag.userManager = _userManager;
			return View();
		}

		public ActionResult Show(string id_utilizator, int id_produs)
		{
			var id = (from r in db.Reviewuri
					 where r.UtilizatorId == id_utilizator &&
					 r.ProdusId == id_produs
					 select r).ToList().ElementAt(0).Id;

			Review? review = db.Reviewuri.Find(id, id_utilizator, id_produs);
			ViewBag.canedit = false;

			if (review == null)
			{
				TempData["message"] = "Reviewul cautat nu exista";
				return RedirectToAction("Index");
			}
			if (review.UtilizatorId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
				ViewBag.canedit = true;

			var utilizatori = from utilizator in db.Utilizatori
							  select utilizator;

			var produse = from produs in db.Produse
						  select produs;

			ViewBag.Utilizatori = utilizatori;
			ViewBag.Produse = produse;
			return View(review);
		}

		// Le specificam pe toate pt ca doar userii neautentificati nu au voie sa lase reviewuri
		[Authorize(Roles = "Utilizator,Colaborator,Administrator")]
		public ActionResult New()
		{
			ViewBag.produsReview = HttpContext.Session.GetString("produs-review");
			ViewBag.userReview = _userManager.GetUserId(User);
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

				HttpContext.Session.SetString("produs-review", ""); // Save string key "produs-review" with value "" in session state. 

				TempData["message"] = "Review-ul a fost adaugat!";
				return RedirectToAction("Index");
			}
			return View(review);
		}

		public ActionResult Edit(string id_utilizator, int id_produs)
		{
			var id = (from r in db.Reviewuri
					  where r.UtilizatorId == id_utilizator &&
					  r.ProdusId == id_produs
					  select r).ToList().ElementAt(0).Id;

			Review? review = db.Reviewuri.Find(id, id_utilizator, id_produs);

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
		public ActionResult Edit(int id, string id_utilizator, int id_produs, Review reqReview)
		{
			Review? review = db.Reviewuri.Find(id, id_utilizator, id_produs);

			if (review == null)
			{
				TempData["message"] = "Reviewul cautat nu exista";
				return RedirectToAction("Index");
			}
			if (review.UtilizatorId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
			{
				if (ModelState.IsValid)
				{
					review.Rating = reqReview.Rating;
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
			var id = (from r in db.Reviewuri
					  where r.UtilizatorId == id_utilizator &&
					  r.ProdusId == id_produs
					  select r).ToList().ElementAt(0).Id;

			Review? review = db.Reviewuri.Find(id, id_utilizator, id_produs);

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
