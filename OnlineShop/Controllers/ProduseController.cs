﻿using Microsoft.AspNetCore.Authorization;
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

		public ActionResult Index(int? id_categ)
		{
			if (TempData.ContainsKey("message"))
				ViewBag.message = TempData["message"].ToString();

			var produse = from produs in db.Produse
						  orderby produs.Titlu
						  select produs;

			if (id_categ != null)
			{
				produse = from produs in db.Produse
						  where produs.Id_Categorie == id_categ
						  orderby produs.Titlu
						  select produs;
			}
			ViewBag.Produse = produse;
            return View();
        }

        public ActionResult Show(int id)
		{
			Produs? produs = db.Produse.Find(id);

            if (produs == null)
            {
                TempData["message"] = "Produsul cautat nu exista";
                return RedirectToAction("Index");
            }
            return View(produs);
		}

		[Authorize(Roles = "Administrator,Colaborator")]
		public ActionResult New()
        {
			return View();
		}

		[Authorize(Roles = "Administrator,Colaborator")]
		[HttpPost]
		public ActionResult New(Produs produs)
		{
			if (ModelState.IsValid)
			{
				if (User.IsInRole("Administrator"))
				{
                    db.Produse.Add(produs);
                    db.SaveChanges();
                    TempData["message"] = "Produsul a fost adaugat";
                }
				else
				{
					Cerere cerere = new Cerere
					{
						ProdusId = null,
						Produs = produs,
						Info = "Cerere adaugare produs",
						Acceptat = false,
						Respins = false,
						Data = DateTime.Now
					};
					db.Cereri.Add(cerere);
					db.SaveChanges();
                    TempData["message"] = "Cererea de adaugare a produsului a fost trimisa";
                }
				return RedirectToAction("Index");
			}
			return View(produs);
		}

		[Authorize(Roles = "Administrator,Colaborator")]
		public ActionResult Edit(int id)
        {
			Produs? produs = db.Produse.Find(id);

            if (produs == null)
            {
                TempData["message"] = "Produsul cautat nu exista";
                return RedirectToAction("Index");
            }
            return View(produs);
		}

		[Authorize(Roles = "Administrator,Colaborator")]
		[HttpPost]
        public ActionResult Edit(int id, Produs reqProd)
		{
			Produs? produs = db.Produse.Find(id);

            if (produs == null)
            {
                TempData["message"] = "Produsul cautat nu exista";
                return RedirectToAction("Index");
            }
			if (User.IsInRole("Administrator"))
			{
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
			else
			{
				Cerere cerere = new Cerere
				{
					ProdusId = produs.Id,
					Produs = reqProd,
					Info = "Cerere editare produs",
					Acceptat = false,
					Respins = false,
					Data = DateTime.Now
				};
				db.Cereri.Add(cerere);
				db.SaveChanges();
				TempData["message"] = "Cererea de editare a produsului a fost trimisa";
			}
		}

		[Authorize(Roles = "Administrator,Colaborator")]
		[HttpPost]
        public ActionResult Delete(int id)
		{
			Produs? produs = db.Produse.Find(id);

            if (produs == null)
            {
                TempData["message"] = "Produsul cautat nu exista";
                return RedirectToAction("Index");
            }
			if (User.IsInRole("Administrator"))
			{
				db.Produse.Remove(produs);
				TempData["message"] = "Produsul a fost sters";
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			else
			{
				Cerere cerere = new Cerere
				{
					ProdusId = produs.Id,
					Produs = null,
					Info = "Cerere stergere produs",
					Acceptat = false,
					Respins = false,
					Data = DateTime.Now
				};
				db.Cereri.Add(cerere);
				db.SaveChanges();
				TempData["message"] = "Cererea de stergere a produsului a fost trimisa";
			}
		}
	}
}