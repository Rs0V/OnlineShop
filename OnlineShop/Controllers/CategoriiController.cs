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

			ViewBag.Categorii = categorii;
			return View();
		}

		public ActionResult Show(int id)
		{
			Categorie categorie = db.Categorii.Find(id);
			return View(categorie);
		}

		public ActionResult New()
		{
			return View();
		}

		[HttpPost]
		public ActionResult New(Categorie categ)
		{
			if (ModelState.IsValid)
			{
				db.Categorii.Add(categ);
				db.SaveChanges();
				TempData["message"] = "Categoria a fost adaugata";
				return RedirectToAction("Index");
			}
			return View(categ);
		}

		public ActionResult Edit(int id)
		{
            Categorie categorie = db.Categorii.Find(id);
			return View(categorie);
		}

		[HttpPost]
		public ActionResult Edit(int id, Categorie reqCateg)
		{
            Categorie categorie = db.Categorii.Find(id);
			if (ModelState.IsValid)
			{
                categorie.Denumire = reqCateg.Denumire;
				db.SaveChanges();
				TempData["message"] = "Categoria a fost modificata!";
				return RedirectToAction("Index");
			}
			return View(reqCateg);
		}

		[HttpPost]
		public ActionResult Delete(int id)
		{
            Categorie categorie = db.Categorii.Find(id);
			db.Categorii.Remove(categorie);
			TempData["message"] = "Categoria a fost stearsa";
			db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}