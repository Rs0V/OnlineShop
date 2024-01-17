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

		[Authorize(Roles = "Colaborator,Administrator")]
		public ActionResult Index()
		{
			if (TempData.ContainsKey("message"))
				ViewBag.message = TempData["message"].ToString();

			var exemplare = from exemplar in db.Exemplare
							join produs in db.Produse on exemplar.ProdusId equals produs.Id
							orderby produs.Titlu, exemplar.Id
							select exemplar;

			var produse = from produs in db.Produse
						  select produs;

			ViewBag.Exemplare = exemplare; // Luam toate exemplarele, impreuna cu toate produsele corespunzatoare, ordonate crescator dupa titlul produsul, respectiv id-ul exemplarului

			ViewBag.Produse = produse; // Luam toate produsele din baza de date
			
			return View();
		}
		[Authorize(Roles = "Colaborator,Administrator")]
		public ActionResult Show(int nr_exemplar, int id_produs)
		{
			Exemplar? exemplar = db.Exemplare.Find(nr_exemplar, id_produs); // Gasim exemplarul dupa nr_exemplar si id_produs (Composite PK)

			if (exemplar == null)
			{
				TempData["message"] = "Exemplarul cautat nu exista !";
				return RedirectToAction("Index");
			}

			var produse = from produs in db.Produse
						  select produs;

			ViewBag.Produse = produse; // Luam toate produsele din baza de date

			return View(exemplar);
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult New()
		{
			return View();
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public ActionResult New(Exemplar exemplar)
		{
			if (ModelState.IsValid)
			{
				db.Exemplare.Add(exemplar); // Adaugam un nou exemplar in baza de date

				db.SaveChanges();

				TempData["message"] = "Exemplarul a fost adaugat!";
				return RedirectToAction("Index"); // Ne ducem pe metoda "Index" din "Exemplare"
			}
			return View(exemplar);
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult Edit(int nr_exemplar, int id_produs)
		{
			Exemplar? exemplar = db.Exemplare.Find(nr_exemplar, id_produs); // Gasim exemplarul dupa nr_exemplar si id_produs (Composite PK)

			if (exemplar == null)
			{
				TempData["message"] = "Exemplarul cautat nu exista!";
				return RedirectToAction("Index");
			}
			return View(exemplar);
		}
		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public ActionResult Edit(int nr_exemplar, int id_produs, Exemplar reqEx)
		{
			Exemplar? exemplar = db.Exemplare.Find(nr_exemplar, id_produs); // Gasim exemplarul dupa nr_exemplar si id_produs (Composite PK)

			if (exemplar == null)
			{
				TempData["message"] = "Exemplarul cautat nu exista!";
				return RedirectToAction("Index");
			}
			if (ModelState.IsValid)
			{
				// Editam exemplarul corespunzator
				exemplar.Stare = reqEx.Stare;
				exemplar.ComandaId = reqEx.ComandaId;

				db.SaveChanges();

				TempData["message"] = "Exemplarul a fost modificat!";
				return RedirectToAction("Index");
			}
			return View(reqEx);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public ActionResult Delete(int nr_exemplar, int id_produs)
		{
			Exemplar? exemplar = db.Exemplare.Find(nr_exemplar, id_produs); // Gasim exemplarul dupa nr_exemplar si id_produs (Composite PK)

			if (exemplar == null)
			{
				TempData["message"] = "Exemplarul cautat nu exista!";
				return RedirectToAction("Index");
			}

			db.Exemplare.Remove(exemplar); // Stergem exemplarul corespunzator din baa de date
			
			TempData["message"] = "Exemplarul a fost sters!";
			
			db.SaveChanges();
			
			return RedirectToAction("Index");
		}
	}
}