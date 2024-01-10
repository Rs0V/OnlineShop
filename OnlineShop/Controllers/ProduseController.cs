using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Data;
using System.Data;
using System.Text.RegularExpressions;

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

		public ActionResult Index()
		{
			var perPagina = 9;

			if (TempData.ContainsKey("message"))
				ViewBag.message = TempData["message"].ToString();

			var produse = from produs in db.Produse
						  orderby produs.Titlu
						  select produs;



			var filter_value = Convert.ToString(HttpContext.Request.Query["sort-value"]);
			var filter_order = Convert.ToString(HttpContext.Request.Query["sort-order"]);
			if (filter_value != null && filter_order != null)
			{
				filter_value = filter_value.Trim();
				filter_order = filter_order.Trim();
			}
			else
			{
				filter_value = "titlu";
				filter_order = "cresc";
			}



			var search = Convert.ToString(HttpContext.Request.Query["search"]);
			if (search != null && search.Trim() != "")
			{
				search = search.Trim();

				var prodIds = (from produs in db.Produse
							   where produs.Titlu.Contains(search) ||
							   produs.Descriere.Contains(search)
							   select produs.Id).ToList();

				//var prodIdsFromReviews = (from produs in db.Produse
				//						  join review in db.Reviewuri on produs.Id equals review.ProdusId
				//						  where review.Continut.Contains(search)
				//						  select produs.Id).ToList();

				//var mergedIds = prodIds.Union(prodIdsFromReviews).ToList();

				produse = from produs in db.Produse
						  where prodIds.Contains(produs.Id)
						  orderby produs.Titlu
						  select produs;

				switch (filter_value)
				{
					case "titlu":
						switch (filter_order)
						{
							case "cresc":
								produse = from produs in db.Produse
										  where prodIds.Contains(produs.Id)
										  orderby produs.Titlu
										  select produs;
								break;

							case "desc":
								produse = from produs in db.Produse
										  where prodIds.Contains(produs.Id)
										  orderby produs.Titlu descending
										  select produs;
								break;
						}
						break;

					case "pret":
						switch (filter_order)
						{
							case "cresc":
								produse = from produs in db.Produse
										  where prodIds.Contains(produs.Id)
										  orderby produs.Pret
										  select produs;
								break;

							case "desc":
								produse = from produs in db.Produse
										  where prodIds.Contains(produs.Id)
										  orderby produs.Pret descending
										  select produs;
								break;
						}
						break;

					case "rating":
						switch (filter_order)
						{
							case "cresc":
								produse = from produs in db.Produse
										  where prodIds.Contains(produs.Id)
										  orderby produs.Rating
										  select produs;
								break;

							case "desc":
								produse = from produs in db.Produse
										  where prodIds.Contains(produs.Id)
										  orderby produs.Rating descending
										  select produs;
								break;
						}
						break;
				}
				ViewBag.PaginationBaseUrl = "/Produse/Index/?search=" + search + "&filter=" + filter_value + "&order=" + filter_order + "&page";
			}
			else
			{
				switch (filter_value)
				{
					case "titlu":
						switch (filter_order)
						{
							case "cresc":
								produse = from produs in db.Produse
										  orderby produs.Titlu
										  select produs;
								break;

							case "desc":
								produse = from produs in db.Produse
										  orderby produs.Titlu descending
										  select produs;
								break;
						}
						break;

					case "pret":
						switch (filter_order)
						{
							case "cresc":
								produse = from produs in db.Produse
										  orderby produs.Pret
										  select produs;
								break;

							case "desc":
								produse = from produs in db.Produse
										  orderby produs.Pret descending
										  select produs;
								break;
						}
						break;

					case "rating":
						switch (filter_order)
						{
							case "cresc":
								produse = from produs in db.Produse
										  orderby produs.Rating
										  select produs;
								break;

							case "desc":
								produse = from produs in db.Produse
										  orderby produs.Rating descending
										  select produs;
								break;
						}
						break;
				}
				ViewBag.PaginationBaseUrl = "/Produse/Index/?filter=" + filter_value + "&order=" + filter_order + "&page";
			}
			ViewBag.SearchString = search;
			ViewBag.FilterValue = filter_value;
			ViewBag.FilterOrder = filter_order;



            var prodCount = produse.Count();
			var pagCurenta = Convert.ToInt32(HttpContext.Request.Query["page"]);

			var offset = 0;
			if (pagCurenta != 0)
				offset = (pagCurenta - 1) * perPagina;

			var prodPag = produse.Skip(offset).Take(perPagina);

			ViewBag.lastPage = Math.Ceiling((float)prodCount / (float)perPagina);

			/*
			if (id_categ != null)
			{
				produse = from produs in db.Produse
						  where produs.CategorieId == id_categ
						  orderby produs.Titlu
						  select produs;
			}
			string? search = Convert.ToString(HttpContext.Request.Query["search"]);
			if (search != null)
			{
				search = search.Trim();
				produse = from produs in db.Produse
						  where produs.CategorieId == id_categ &&
						  (produs.Titlu.Contains(search) ||
						  produs.Descriere.Contains(search))
						  orderby produs.Titlu
						  select produs;

				ViewBag.SearchString = search;

				if (filter != null)
				{
					switch ((SearchFilter)filter)
					{
						case SearchFilter.Nume_Crescator:
							break;

						case SearchFilter.Nume_Descrescator:
							produse = from produs in db.Produse
									  where produs.CategorieId == id_categ
									  where produs.Titlu.Contains(search)
									  orderby produs.Titlu descending
									  select produs;
							break;

						case SearchFilter.Pret_Crescator:
							produse = from produs in db.Produse
									  where produs.CategorieId == id_categ
									  where produs.Titlu.Contains(search)
									  orderby produs.Pret
									  select produs;
							break;

						case SearchFilter.Pret_Descrescator:
							produse = from produs in db.Produse
									  where produs.CategorieId == id_categ
									  where produs.Titlu.Contains(search)
									  orderby produs.Pret descending
									  select produs;
							break;

						case SearchFilter.Rating_Crescator:
							produse = from produs in db.Produse
									  where produs.CategorieId == id_categ
									  where produs.Titlu.Contains(search)
									  orderby produs.Rating
									  select produs;
							break;

						case SearchFilter.Rating_Descrescator:
							produse = from produs in db.Produse
									  where produs.CategorieId == id_categ
									  where produs.Titlu.Contains(search)
									  orderby produs.Rating descending
									  select produs;
							break;
					}
				}
			}
			*/

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
	}
}