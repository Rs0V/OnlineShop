using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Data;
using System.Data;

namespace OnlineShop.Controllers
{
	public class CategoriiController : Controller
	{
		private readonly ApplicationDbContext db;
		public CategoriiController(ApplicationDbContext context)
		{
			db = context;
		}

		public ActionResult Index()
		{
			if (TempData.ContainsKey("message"))
				ViewBag.message = TempData["message"].ToString();

			var categorii = from categorie in db.Categorii
							orderby categorie.Denumire
							select categorie;

			ViewBag.Categorii = categorii; // Toate categoriile in ordine crescatoare dupa denumirea acestora
			return View();
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult Show(int id)
		{
			Categorie? categorie = db.Categorii.Find(id); // Gasim categoria dupa "id"
			if (categorie == null)
			{
				TempData["message"] = "Categoria cautata nu exista !";

				return RedirectToAction("Index"); // Ajungem apoi in metoda "Index" din "Categorii"
			}
			return View(categorie); // returnam "@Model" in View-ul "Show" din "Categorii"
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult New()
		{
			return View();
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public ActionResult New(Categorie categ)
		{
			if (ModelState.IsValid) // We can successfully add a new category to our database (validation rules respected)
			{
				db.Categorii.Add(categ); // Adaugam noua categorie validata in baza de date
				
				db.SaveChanges(); // "commit" in Database (save all changes)
				
				TempData["message"] = "Categoria a fost adaugata";
				return RedirectToAction("Index");
			}
			return View(categ);
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult Edit(int id)
		{
			Categorie? categorie = db.Categorii.Find(id);

			if (categorie == null)
			{
				TempData["message"] = "Categoria cautata nu exista";
				return RedirectToAction("Index");
			}
			return View(categorie);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public ActionResult Edit(int id, Categorie reqCateg)
		{
			Categorie? categorie = db.Categorii.Find(id);

			if (categorie == null)
			{
				TempData["message"] = "Categoria cautata nu exista";
				return RedirectToAction("Index");
			}
			if (ModelState.IsValid)
			{ 
				// Editam toate atributele pentru categoria dorita
				categorie.Denumire = reqCateg.Denumire;
				
				db.SaveChanges();

				TempData["message"] = "Categoria a fost modificata!";
				return RedirectToAction("Index");
			}
			return View(reqCateg);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public ActionResult Delete(int id)
		{
			Categorie? categorie = db.Categorii.Find(id);

			if (categorie == null)
			{
				TempData["message"] = "Categoria cautata nu exista";
				return RedirectToAction("Index");
			}

			db.Categorii.Remove(categorie); // Stergem categoria dorita din baza de date (daca aceasta a fost gasita)

			TempData["message"] = "Categoria a fost stearsa";
			
			db.SaveChanges();
			
			return RedirectToAction("Index"); // Ne intoarcem apoi in metoda "Index" din "Categorii"
		}
	}
}