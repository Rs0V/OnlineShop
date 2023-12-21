using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Data;
using System.Data;

namespace OnlineShop.Controllers
{
	public class ProduseController : Controller
	{
		private readonly ApplicationDbContext db;

		public ProduseController(ApplicationDbContext context)
		{
			db = context;
		}

		public ActionResult Index()
		{
			if (TempData.ContainsKey("message"))
				ViewBag.message = TempData["message"].ToString();

			var produse = from produs in db.Produse
						  orderby produs.Titlu
						  select produs;

			ViewBag.Produse = produse;
			return View();
		}

		public ActionResult Show(int id)
		{
			Produs produse = db.Produse.Find(id);
			return View(produse);
		}

		public ActionResult New()
		{
			return View();
		}

		[HttpPost]
		public ActionResult New(Produs produs)
		{
			if (ModelState.IsValid)
			{
				db.Produse.Add(produs);
				db.SaveChanges();
				TempData["message"] = "Produsul a fost adaugat";
				return RedirectToAction("Index");
			}
			return View(produs);
		}

		public ActionResult Edit(int id)
		{
			Produs produs = db.Produse.Find(id);
			return View(produs);
		}

		[HttpPost]
		public ActionResult Edit(int id, Produs reqProd)
		{
			Produs produs = db.Produse.Find(id);
			if (ModelState.IsValid)
			{
				produs.Titlu = reqProd.Titlu;
				produs.Descriere = reqProd.Descriere;
                produs.Pret = reqProd.Pret;
				produs.Poza = reqProd.Poza;
				produs.Rating = reqProd.Rating;
				produs.Id_Categorie = reqProd.Id_Categorie;

                db.SaveChanges();
				TempData["message"] = "Produsul a fost modificat!";
				return RedirectToAction("Index");
			}
			return View(reqProd);
		}

		[HttpPost]
		public ActionResult Delete(int id)
		{
			Produs produs = db.Produse.Find(id);
            db.Produse.Remove(produs);
			TempData["message"] = "Produsul a fost sters";
			db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}