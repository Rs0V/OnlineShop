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
			if (TempData.ContainsKey("message"))
				ViewBag.message = TempData["message"].ToString();

			var produse = from produs in db.Produse
						  orderby produs.Titlu
						  select produs; // Luam toate produsele din baza de date ordonate crescator in functie de titlul acestora


			if (categ != null)
				produse = (IOrderedQueryable<Produs>)produse.Where(p => p.CategorieId == categ);

			
			var search = Convert.ToString(HttpContext.Request.Query["search"]);
			if (search != null && search.Trim() != "" && search.Trim() != "null")
			{
				search = search.Trim();
				produse = (IOrderedQueryable<Produs>)produse.Where(p => p.Titlu.Contains(search) || p.Descriere.Contains(search));
			}


			var sort = Convert.ToString(HttpContext.Request.Query["sort"]);
			if (sort == null || sort.Trim() == "" || sort.Trim() == "null")
				sort = Convert.ToString(HttpContext.Request.Query["sort-value"]);
			if (sort == null || sort.Trim() == "" || sort.Trim() == "null")
				sort = "titlu";

			var order = Convert.ToString(HttpContext.Request.Query["order"]);
			if (order == null || order.Trim() == "" || order.Trim() == "null")
				order = Convert.ToString(HttpContext.Request.Query["sort-order"]);
			if (order == null || order.Trim() == "" || order.Trim() == "null")
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
					produse = produse.OrderByDescending(p =>
						(sort == "titlu") ? p.Titlu :
						(sort == "pret") ? p.Pret.ToString() :
						p.Rating.ToString());
					break;
			}

			ViewBag.PaginationBaseUrl = "/Produse/Index?categ=" + ((categ != null) ? categ.ToString() : "null") +
					"&search=" + ((search != null) ? search.ToString() : "null") +
					"&sort=" + ((sort != null) ? sort.ToString() : "null") +
					"&order=" + ((order != null) ? order.ToString() : "null") +
					"&page";


			ViewBag.SearchString = (search != null && search == "null") ? null : search;
			ViewBag.FilterValue = sort;
			ViewBag.FilterOrder = order;


			var perPagina = 4; // Maxim 4 produse afisate per pagina

			var prodCount = produse.Count();
			var pagCurenta = Convert.ToInt32(HttpContext.Request.Query["page"]);
			if (pagCurenta == 0)
				pagCurenta = 1;

			var offset = (pagCurenta - 1) * perPagina;

			///
			var prodPag = produse.Skip(offset).Take(perPagina);
			///

			ViewBag.lastPage = Math.Ceiling((float)prodCount / (float)perPagina);


			var categorii = from categorie in db.Categorii
							select categorie;


			ViewBag.Produse = prodPag; // Luam toate produsele de pe pagina respectiva si le punem in ViewBag pentru afisare in View

			ViewBag.Categorii = categorii; // Luam toate categoriile din baza de date
			
			return View();
		}

		public ActionResult Show(int id)
		{
			Produs? produs = db.Produse.Find(id);

			if (produs == null)
			{
				TempData["message"] = "Produsul cautat nu exista!";
				return RedirectToAction("Index");
			}

			var categorii = from categorie in db.Categorii
							select categorie;

			ViewBag.Categorii = categorii; // Luam toate categoriile din baza de date

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
					db.Produse.Add(produs); // Adaugam noul produs in baza de date, doar daca suntem admin, iar regulile de validare sunt respectate
					
					db.SaveChanges();
					
					TempData["message"] = "Produsul a fost adaugat!";
				}
				else
				{
					Cerere cerere = new Cerere // Cream o cerere de adaugare produs, daca suntem colab1
					{
						ProdusId = null,
						AuxProd = produs.ToString(),
						Info = "Cerere adaugare produs",
						Acceptat = Acceptare.In_Asteptare, // Cererea este initial "In_asteptare", urmand sa fie evaluata de admin
						Data = DateTime.Now
					};

					db.Cereri.Add(cerere); // Adaugam cererea de adaugare a produsului in baza de date, pt. a fi evaluata de admin
					
					db.SaveChanges();

					TempData["message"] = "Cererea de adaugare a produsului a fost trimisa!";
				}
				return RedirectToAction("Index"); // Ne ducem pe metoda "Index" din "Produse"
			}
			return View(produs);
		}

		[Authorize(Roles = "Administrator,Colaborator")]
		public ActionResult Edit(int id)
		{
			Produs? produs = db.Produse.Find(id);

			if (produs == null)
			{
				TempData["message"] = "Produsul cautat nu exista!";
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
				TempData["message"] = "Produsul cautat nu exista!";
				return RedirectToAction("Index");
			}
			if (User.IsInRole("Administrator"))
			{
				if (ModelState.IsValid)
				{
					// Editam produsul corespunzator, doar daca suntem admin, iar regulile de validare sunt respectate
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
				Cerere cerere = new Cerere // Cream o cerere de editare produs, daca suntem colab1
				{
					ProdusId = reqProd.Id,
					AuxProd = reqProd.ToString(),
					Info = "Cerere editare produs",
					Acceptat = Acceptare.In_Asteptare, // Cererea este initial "In_asteptare", urmand sa fie evaluata de admin
					Data = DateTime.Now
				};

				db.Cereri.Add(cerere); // Adaugam cererea de editare a produsului in baza de date, pt. a fi evaluata de admin

				db.SaveChanges();
				
				TempData["message"] = "Cererea de editare a produsului a fost trimisa!";
			}
			return RedirectToAction("Index"); // Ne ducem in final pe metoda "Index" din "Produse"
		}

		[Authorize(Roles = "Administrator,Colaborator")]
		[HttpPost]
		public ActionResult Delete(int id)
		{
			Produs? produs = db.Produse.Find(id);

			if (produs == null)
			{
				TempData["message"] = "Produsul cautat nu exista!";
				return RedirectToAction("Index");
			}
			if (User.IsInRole("Administrator"))
			{
				db.Produse.Remove(produs); // Stergem produsul din baza de date, daca suntem admin

				TempData["message"] = "Produsul a fost sters!";
				db.SaveChanges();
			}
			else
			{
				Cerere cerere = new Cerere	// Cream o cerere de stergere produs, daca suntem colab1
				{
					ProdusId = produs.Id,
					AuxProd = null,
					Info = "Cerere stergere produs",
					Acceptat = Acceptare.In_Asteptare, // Cererea este initial "In_asteptare", urmand sa fie evaluata de admin
					Data = DateTime.Now
				};
				
				db.Cereri.Add(cerere); // Adaugam cererea de stergere a produsului in baza de date, pt. a fi evaluata de admin

				db.SaveChanges();
				
				TempData["message"] = "Cererea de stergere a produsului a fost trimisa!";
			}
			return RedirectToAction("Index"); // Ne ducem in final pe metoda "Index" din "Produse"
		}

		// Shopping Cart

		[HttpPost]
		public ActionResult AdaugaCos(int id) 
		{
            Produs? produs = db.Produse.Find(id); // Gasim produsul dupa "id"

			if (produs == null)
            {
                TempData["message"] = "Produsul cautat nu exista!";
                return RedirectToAction("Index");
            }
			
			if (HttpContext.Session.GetString("cos") == null)
			{
				HttpContext.Session.SetString("cos", ""); // Save string key "cos" with value "" in session state (if key does not exist)
			}

			// Save string key "cos" with value (value + id + " ") in session state
			// (Adaugam la valoarea cosului din session state, id-ul produsului (ca string) pe care dorim sa-l punem
			// + inca un spatiu pt. delimitare)
			HttpContext.Session.SetString("cos", HttpContext.Session.GetString("cos") + id.ToString() + " ");

			return RedirectToAction("Index");
        }



		[HttpPost]
		public ActionResult ScrieReview(int id)
		{
			Produs? produs = db.Produse.Find(id); // Gasim produsul dupa "id"

			if (produs == null)
			{
				TempData["message"] = "Produsul cautat nu exista!";
				return RedirectToAction("Index");
			}

			// Save string key "produs-review" with value (produs.Id) in session state
			// (Adaugam la valoarea review-ului pt. produs din session state, id-ul produsului (ca string) pe care dorim sa-l punem
			HttpContext.Session.SetString("produs-review", produs.Id.ToString());

			return Redirect("/Reviewuri/New"); // Ne ducem pe "New" cu GET din controller-ul "Reviewuri"
		}
	}
}