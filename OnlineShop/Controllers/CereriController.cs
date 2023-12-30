using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnlineShop.Models;
using OnlineShop.Data;
using System.Data;


namespace OnlineShop.Controllers
{
	public class CereriController : Controller
	{
		public Produs String2Produs(in string str)
		{
			var prod_params = str.Split("╚");
			Produs produs = new Produs
			{
				Titlu = prod_params[0],
				Descriere = prod_params[1],
				Pret = float.Parse(prod_params[2]),
				Poza = prod_params[3],
				Rating = (prod_params[4] != "") ? int.Parse(prod_params[4]) : null,
				CategorieId = int.Parse(prod_params[5])
			};
			return produs;
		}



		private readonly ApplicationDbContext db;
		public CereriController(ApplicationDbContext context)
		{
			db = context;
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult Index()
		{
			if (TempData.ContainsKey("message"))
				ViewBag.message = TempData["message"].ToString();

			var cereri = from cerere in db.Cereri
						 orderby cerere.Data descending
						 select cerere;

			var categorii = from categorie in db.Categorii
							select categorie;

			ViewBag.Cereri = cereri;
			ViewBag.Categorii = categorii;
			return View();
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult Edit(int id)
		{
			Cerere? cerere = db.Cereri.Find(id);

			if (cerere == null)
			{
				TempData["message"] = "Cererea nu exista";
				return RedirectToAction("Index");
			}

			var categorii = from categorie in db.Categorii
							select categorie;
			ViewBag.Categorii = categorii;

			return View(cerere);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public ActionResult Edit(int id, Cerere request)
		{
			Cerere? cerere = db.Cereri.Find(id);

			if (cerere == null)
			{
				TempData["message"] = "Cererea nu exista";
				return RedirectToAction("Index");
			}
			cerere.Acceptat = request.Acceptat;
			//if (ModelState.IsValid)
			{
				//cerere.ProdusId = request.ProdusId;
				//cerere.AuxProd = request.AuxProd;
				//cerere.Acceptat = request.Acceptat;
				////cerere.Respins = request.Respins;
				//cerere.Data = request.Data;
				//cerere.Info = request.Info;

				TempData["message"] = "Cererea a fost modificata!";

				if (cerere.Acceptat == Acceptare.Acceptat)
				{
					if (cerere.ProdusId == null && cerere.AuxProd != null)
					{
						db.Produse.Add(String2Produs(cerere.AuxProd));
						TempData["message"] += " Produsul a fost adaugat";
					}
					else if (cerere.ProdusId != null && cerere.AuxProd != null)
					{
						var auxProd = String2Produs(cerere.AuxProd);

						cerere.Produs.Titlu = auxProd.Titlu;
						cerere.Produs.Descriere = auxProd.Descriere;
						cerere.Produs.Pret = auxProd.Pret;
						cerere.Produs.Poza = auxProd.Poza;
						cerere.Produs.Rating = auxProd.Rating;
						cerere.Produs.CategorieId = auxProd.CategorieId;

						TempData["message"] += " Produsul a fost editat!";
					}
					else if (cerere.ProdusId != null && cerere.Produs == null)
					{
						db.Produse.Remove(cerere.Produs);
						TempData["message"] = "Produsul a fost sters!";
					}
				}
				db.SaveChanges();
			}
			return RedirectToAction("Index");
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public ActionResult Delete(int id)
		{
			Cerere? cerere = db.Cereri.Find(id);

			if (cerere == null)
			{
				TempData["message"] = "Cererea nu exista";
				return RedirectToAction("Index");
			}
			
			db.Cereri.Remove(cerere);
			TempData["message"] = "Cererea a fost stearsa";
			db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}

