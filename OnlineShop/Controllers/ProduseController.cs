using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Data;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace OnlineShop.Controllers
{
	enum SearchFilter
	{
		Pret_Crescator, Pret_Descrescator,
		Rating_Crescator, Rating_Descrescator,
		Nume_Crescator, Nume_Descrescator
	}

	public class ProduseController : Controller
	{
		private readonly ApplicationDbContext db;

		public ProduseController(ApplicationDbContext context)
		{
			db = context;
		}

		public ActionResult Index(int? categ)
		{
			var perPagina = 4;

			if (TempData.ContainsKey("message"))
				ViewBag.message = TempData["message"].ToString();

			var produse = from produs in db.Produse
						  orderby produs.Titlu
						  select produs;


			if (categ != null)
				produse = (IOrderedQueryable<Produs>)produse.Where(p => p.CategorieId == categ);

			
			var search = Convert.ToString(HttpContext.Request.Query["search"]);
			if (search != null && search.Trim() != "")
			{
				search = search.Trim();
				produse = (IOrderedQueryable<Produs>)produse.Where(p => p.Titlu.Contains(search) || p.Descriere.Contains(search));
			}


			var sort = Convert.ToString(HttpContext.Request.Query["sort"]);
			if (sort == null)
				sort = Convert.ToString(HttpContext.Request.Query["sort-value"]);
			if (sort == null)
				sort = "titlu";

			var order = Convert.ToString(HttpContext.Request.Query["order"]);
			if (order == null)
				order = Convert.ToString(HttpContext.Request.Query["sort-order"]);
			if (order == null)
				order = "cresc";

			sort = sort.Trim();
			order = order.Trim();


			switch (order)
			{
				case "cresc":
					produse = produse.OrderBy(p =>
						(sort == "titlu") ? p.Titlu :
						(sort == "pret") ? p.Pret.ToString() :
						p.Rating.ToString());
					break;

				case "desc":
					produse = produse.OrderBy(p =>
						(sort == "titlu") ? p.Titlu :
						(sort == "pret") ? p.Pret.ToString() :
						p.Rating.ToString());
					break;
			}

			ViewBag.PaginationBaseUrl = "/Produse/Index/?categ=" + ((categ != null) ? categ.ToString() : "null") +
					"&search=" + ((search != null) ? search.ToString() : "null") +
					"&sort=" + ((sort != null) ? sort.ToString() : "null") +
					"&order=" + ((order != null) ? order.ToString() : "null") +
					"&page";


			ViewBag.SearchString = search;
			ViewBag.FilterValue = sort;
			ViewBag.FilterOrder = order;



            var prodCount = produse.Count();
			var pagCurenta = Convert.ToInt32(HttpContext.Request.Query["page"]);

			var offset = 0;
			if (pagCurenta != 0)
				offset = (pagCurenta - 1) * perPagina;

			var prodPag = produse.Skip(offset).Take(perPagina);

			ViewBag.lastPage = Math.Ceiling((float)prodCount / (float)perPagina);



			var categorii = from categorie in db.Categorii
							select categorie;


			ViewBag.Produse = prodPag;
			ViewBag.Categorii = categorii;
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

			var categorii = from categorie in db.Categorii
							select categorie;
			ViewBag.Categorii = categorii;

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
				//var categorii = from categorie in db.Categorii
				//				where categorie.Id == produs.CategorieId
				//				select categorie;

				//if (categorii.Count() == 0)
				//{
				//	TempData["message"] = "Categoria " + produs.CategorieId.ToString() + " nu exista";
				//}
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
						AuxProd = produs.ToString(),
						Info = "Cerere adaugare produs",
						Acceptat = Acceptare.In_Asteptare,
						//Respins = false,
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
					produs.CategorieId = reqProd.CategorieId;

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
					ProdusId = reqProd.Id,
					AuxProd = reqProd.ToString(),
					Info = "Cerere editare produs",
					Acceptat = Acceptare.In_Asteptare,
					//Respins = false,
					Data = DateTime.Now
				};
				db.Cereri.Add(cerere);
				db.SaveChanges();
				TempData["message"] = "Cererea de editare a produsului a fost trimisa";
			}
			return RedirectToAction("Index");
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
			}
			else
			{
				Cerere cerere = new Cerere
				{
					ProdusId = produs.Id,
					AuxProd = null,
					Info = "Cerere stergere produs",
					Acceptat = Acceptare.In_Asteptare,
					//Respins = false,
					Data = DateTime.Now
				};
				db.Cereri.Add(cerere);
				db.SaveChanges();
				TempData["message"] = "Cererea de stergere a produsului a fost trimisa";
			}
			return RedirectToAction("Index");
		}

		// Shopping Cart

		[HttpPost]
		public ActionResult AdaugaCos(int id) 
		{
            Produs? produs = db.Produse.Find(id);

            if (produs == null)
            {
                TempData["message"] = "Produsul cautat nu exista";
                return RedirectToAction("Index");
            }
			
			if (HttpContext.Session.GetString("cos") == null)
			{
				HttpContext.Session.SetString("cos", "");
            }
			
			HttpContext.Session.SetString("cos", HttpContext.Session.GetString("cos") + id.ToString() + " ");

			return RedirectToAction("Index");
        }



		[HttpPost]
		public ActionResult ScrieReview(int id)
		{
			Produs? produs = db.Produse.Find(id);

			if (produs == null)
			{
				TempData["message"] = "Produsul cautat nu exista";
				return RedirectToAction("Index");
			}

			HttpContext.Session.SetString("produs-review", produs.Id.ToString());
			return Redirect("/Reviewuri/New");
		}
	}
}