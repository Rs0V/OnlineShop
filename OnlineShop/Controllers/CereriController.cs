﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnlineShop.Models;
using OnlineShop.Data;
using System.Data;


namespace OnlineShop.Controllers
{
	public class CereriController : Controller
	{
		// Passed by reference, but "str" parameter cannot be modified( C++ - const string& str)
		public Produs String2Produs(in string str) 
		{
			var prod_params = str.Split("╚");
			Produs produs = new Produs
			{
				Id = int.Parse(prod_params[0]),
				Titlu = prod_params[1],
				Descriere = prod_params[2],
				Pret = float.Parse(prod_params[3]),
				Poza = prod_params[4],
				Rating = (prod_params[5] != "") ? int.Parse(prod_params[5]) : null,
				CategorieId = int.Parse(prod_params[6])
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

			var produse = from produs in db.Produse
						  select produs;

			ViewBag.Cereri = cereri; // Luam toate cererile in ordine inversa in functie de data crearii acestora

			ViewBag.Produse = produse; // Luam toate produsele din baza de date

			ViewBag.Categorii = categorii; // Luam toate categoriile din baza de date

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
			if (cerere.Acceptat != Acceptare.In_Asteptare) // Admin-ul poate edita o cerere o singura data. 
			{
				TempData["message"] = "Cererea nu mai poate fi modificata";
				return RedirectToAction("Index");
			}

			var categorii = from categorie in db.Categorii
							select categorie;

			var produse = from produs in db.Produse
						  select produs;

			ViewBag.Categorii = categorii; // Luam toate categoriile din baza de date

			ViewBag.Produse = produse; // Luam toate produsele din baza de date
			
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
			//cerere.Acceptat = request.Acceptat;
			if (ModelState.IsValid)
			{
				cerere.Acceptat = request.Acceptat;

				TempData["message"] = "Cererea a fost modificata!";

				if (cerere.Acceptat == Acceptare.Acceptat)
				{
					if (cerere.ProdusId == null && cerere.AuxProd != null)
					{
						var auxProd = String2Produs(cerere.AuxProd);

						db.Produse.Add(auxProd); // Cerere acceptata de admin, deci putem adauga produsul in baza de date (inca nu exista produsul din cerere)

						TempData["message"] += " Produsul a fost adaugat!";
					}
					else if (cerere.ProdusId != null && cerere.AuxProd != null)
					{
						var auxProd = String2Produs(cerere.AuxProd);

						var produs = (from p in db.Produse
									  where p.Id == cerere.ProdusId
									  select p).ToList().ElementAt(0); // Selectam primul produs care are id-ul identic cu "cerere.ProdusId"

						// Cerere acceptata de admin, deci putem edita produsul din baza de date
						produs.Titlu = auxProd.Titlu;
						produs.Descriere = auxProd.Descriere;
						produs.Pret = auxProd.Pret;
						produs.Poza = auxProd.Poza;
						produs.Rating = auxProd.Rating;
						produs.CategorieId = auxProd.CategorieId;

						TempData["message"] += " Produsul a fost editat!";
					}
					else if (cerere.ProdusId != null && cerere.Produs == null) // cerere.AuxProd == null (in loc de) cerere.Produs == null
					{
						var produs = (from p in db.Produse
									  where p.Id == cerere.ProdusId
									  select p).ToList().ElementAt(0); // Selectam primul produs care are id-ul identic cu "cerere.ProdusId"

						db.Produse.Remove(produs); // Cerere acceptata de admin, deci stergem produsul din baza de date

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

