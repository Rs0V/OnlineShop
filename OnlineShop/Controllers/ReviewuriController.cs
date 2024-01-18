using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
	public class ReviewuriController : Controller
	{

		// Pasul 10 - useri si roluri (Configurarea managerului de useri si roluri)
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


			/*
			 
			if (id_filtru != null && tip_id != null)
			{
				if (tip_id == "produs")
					reviewuri = reviewuri.Where(r => r.ProdusId == int.Parse(id_filtru));
				else
					reviewuri = reviewuri.Where(r => r.UtilizatorId == id_filtru);
			}

			*/
		
			var utilizatori = from utilizator in db.Utilizatori
							  select utilizator;

			var produse = from produs in db.Produse
						  select produs;


			ViewBag.Reviewuri = reviewuri; // Luam toate review-urile, impreuna cu produsul si utilizatorul care a lasat review-ul, ordonate dupa "order by" din query-ul de mai sus

			ViewBag.Utilizatori = utilizatori; // Luam toti utilizatorii din baza de date
			
			ViewBag.Produse = produse; // Luam toate produsele din baza de date
			
			ViewBag.userManager = _userManager; // Sa avem acces la id-ul curent al utilizatorului si-n View-uri

			return View();
		}

		public ActionResult Show(string id_utilizator, int id_produs)
		{
			var id = (from r in db.Reviewuri
					 where r.UtilizatorId == id_utilizator &&
					 r.ProdusId == id_produs
					 select r).ToList().ElementAt(0).Id; // Luam id-ul primului review in functie de "UtilizatorId" si "ProdusId" din Show

			Review? review = db.Reviewuri.Find(id, id_utilizator, id_produs); // Gasim review-ul dupa "id", "UtilizatorId", "ProdusId"

			ViewBag.canedit = false;

			if (review == null)
			{
				TempData["message"] = "Reviewul cautat nu exista!";
				return RedirectToAction("Index");
			}

			if (review.UtilizatorId == _userManager.GetUserId(User) || User.IsInRole("Administrator")) // Putem edita un review doar din propriul cont de user sau daca suntem admin (Folosit in View)
				ViewBag.canedit = true;

			var utilizatori = from utilizator in db.Utilizatori
							  select utilizator;

			var produse = from produs in db.Produse
						  select produs;

			ViewBag.Utilizatori = utilizatori; // Luam toti utilizatorii din baza de date

			ViewBag.Produse = produse; // Luam toate produsele din baza de date
			
			return View(review);
		}

		// Le specificam pe toate pt ca doar userii neautentificati nu au voie sa lase review-uri
		[Authorize(Roles = "Utilizator,Colaborator,Administrator")]
		public ActionResult New()
		{
			ViewBag.produsReview = HttpContext.Session.GetString("produs-review"); // Luam valoarea (id-ul produsului) din session state pentru string key-ul "produs-review"
			ViewBag.userReview = _userManager.GetUserId(User); // Adaugam in ViewBag id-ul utilizatorului curent care acceseaza pagina de "New"
			
			return View();
		}

		[Authorize(Roles = "Utilizator,Colaborator,Administrator")]
		[HttpPost]
		public ActionResult New(Review review)
		{
			if (ModelState.IsValid)
			{
				db.Reviewuri.Add(review); // Adaugam review-ul in baza de date

				db.SaveChanges();

				HttpContext.Session.SetString("produs-review", ""); // Save string key "produs-review" with value "" in session state

				TempData["message"] = "Review-ul a fost adaugat!";
				
				return RedirectToAction("Index"); // Ne ducem in final pe metoda "Index" din "Reviewuri"
			}
			return View(review);
		}

		public ActionResult Edit(string id_utilizator, int id_produs)
		{
			var id = (from r in db.Reviewuri
					  where r.UtilizatorId == id_utilizator &&
					  r.ProdusId == id_produs
					  select r).ToList().ElementAt(0).Id; // Luam id-ul primului review in functie de "UtilizatorId" si "ProdusId" din Edit cu GET

			Review? review = db.Reviewuri.Find(id, id_utilizator, id_produs); // Gasim review-ul dupa "id", "UtilizatorId", "ProdusId"

			if (review == null)
			{
				TempData["message"] = "Reviewul cautat nu exista!";
				return RedirectToAction("Index");
			}

			// Putem edita un review doar din propriul cont de user sau daca suntem admin
			if (review.UtilizatorId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
				return View(review);

			TempData["message"] = "Nu aveti dreptul sa editati un review care nu va apartine!";
			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult Edit(int id, string id_utilizator, int id_produs, Review reqReview)
		{
			Review? review = db.Reviewuri.Find(id, id_utilizator, id_produs); // Gasim review-ul dupa "id", "UtilizatorId", "ProdusId"

			if (review == null)
			{
				TempData["message"] = "Reviewul cautat nu exista!";
				return RedirectToAction("Index");
			}

			// Putem edita un review doar din propriul cont de user sau daca suntem admin
			if (review.UtilizatorId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
			{
				if (ModelState.IsValid)
				{
					// Editam review-ul corespunzator
					review.Rating = reqReview.Rating;
					review.Continut = reqReview.Continut;

					db.SaveChanges();

					TempData["message"] = "Review-ul a fost modificat!";

					return RedirectToAction("Index");
				}
				return View(reqReview);
			}

			TempData["message"] = "Nu aveti dreptul sa editati un review care nu va apartine!";
			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult Delete(string id_utilizator, int id_produs)
		{
			var id = (from r in db.Reviewuri
					  where r.UtilizatorId == id_utilizator &&
					  r.ProdusId == id_produs
					  select r).ToList().ElementAt(0).Id; // Luam id-ul primului review in functie de "UtilizatorId" si "ProdusId" din Delete

			Review? review = db.Reviewuri.Find(id, id_utilizator, id_produs); // Gasim review-ul dupa "id", "UtilizatorId", "ProdusId"

			if (review == null)
			{
				TempData["message"] = "Reviewul cautat nu exista!";
				return RedirectToAction("Index");
			}

			// Putem sterge un review doar din propriul cont de user sau daca suntem admin
			if (review.UtilizatorId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
			{
				db.Reviewuri.Remove(review); // Stergem review-ul din baza de date
				
				TempData["message"] = "Review-ul a fost sters!";
				
				db.SaveChanges();
				
				return RedirectToAction("Index");
			}
			
			TempData["message"] = "Nu aveti dreptul sa stergeti un review care nu va apartine!";
			return RedirectToAction("Index");
		}
	}
}
