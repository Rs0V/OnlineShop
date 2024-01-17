using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;
using System.Threading;
using System;

namespace OnlineShop.Controllers
{
	public class ComenziController : Controller
	{
		private readonly ApplicationDbContext db;
		private readonly UserManager<Utilizator> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		public ComenziController(ApplicationDbContext context, UserManager<Utilizator> userManager, RoleManager<IdentityRole> roleManager)
		{
			db = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public ActionResult Index()
		{
			if (TempData.ContainsKey("message"))
				ViewBag.message = TempData["message"].ToString();

			var comenzi = from comanda in db.Comenzi
						  join utilizator in db.Utilizatori on comanda.UtilizatorId equals utilizator.Id
						  where comanda.UtilizatorId == _userManager.GetUserId(User)
						  orderby utilizator.Nume, utilizator.Prenume, comanda.Data descending
						  select comanda;

			if (User.IsInRole("Administrator"))
			{
				comenzi = from comanda in db.Comenzi
						  join utilizator in db.Utilizatori on comanda.UtilizatorId equals utilizator.Id
						  orderby utilizator.Nume, utilizator.Prenume, comanda.Data descending
						  select comanda;
			}

			var utilizatori = from utilizator in db.Utilizatori
							  select utilizator;

			ViewBag.Comenzi = comenzi; // Luam toate comenzile, impreuna cu toti utilizatorii (daca suntem admin), altfel doar toate comenzile plasate din propriul cont de utilizator

			ViewBag.Utilizatori = utilizatori; // Luam toti utilizatorii din baza de date
			
			return View();
		}

		public ActionResult Show(int id)
		{
			Comanda? comanda = db.Comenzi.Find(id);

			if (comanda == null)
			{
				TempData["message"] = "Comanda cautata nu exista";
				return RedirectToAction("Index");
			}
			if (comanda.UtilizatorId == _userManager.GetUserId(User) || User.IsInRole("Administrator")) // Putem vizualiza doar comenzile efectuate din propriul cont sau daca suntem admin
			{
				var utilizatori = from utilizator in db.Utilizatori
				select utilizator;

                ViewBag.Utilizatori = utilizatori; // Luam toti utilizatorii din baza de date

				return View(comanda);
            }

            TempData["message"] = "Nu aveti dreptul sa vizualizati o comanda care nu va apartine";
			
			return RedirectToAction("Index");
		}

		[Authorize(Roles = "Utilizator,Colaborator,Administrator")]
		[HttpGet]
		public ActionResult New()
		{
			var produse = from produs in db.Produse
						  where 1 == 2
						  select produs; // Query Vid

			var cosStr = HttpContext.Session.GetString("cos"); // Get string value with key "cos" from session state

			if (cosStr != null)
			{
				List<int> cos = new List<int>();
				// var a = cosStr.Split(" ").ToList();
				foreach (var str in cosStr.Split(" ").ToList())
				{
					if(str != null && str != "" && str != " ")
						cos.Add(int.Parse(str)); // Punem in lista toate id-urile produselor pe care dorim sa le adaugam in cos
				}

				produse = from produs in db.Produse
						  where cos.Contains(produs.Id)
						  select produs;
			}

			ViewBag.Produse = produse; // Luam toate produsele care au fost incluse deja in cos (sau query-ul vid daca nu avem niciun produs in cos)
			
			ViewBag.userManager = _userManager; // Sa avem acces la id-ul curent al utilizatorului si-n View-uri
			
			return View();
		}

		[Authorize(Roles = "Utilizator,Colaborator,Administrator")]
		[HttpPost]
		public ActionResult New(Comanda comanda)
		{
			if (ModelState.IsValid)
			{
				var exCom = new List<Exemplar>();

				var exemplare = from exemplar in db.Exemplare
								where exemplar.Stare == StareExemplar.Disponibil
								select exemplar; // Luam toate exemplarele disponibile din baza de date

				var cosStr = HttpContext.Session.GetString("cos");

				if (cosStr != null)
				{
					List<int> cos = new List<int>();
					foreach (var str in cosStr.Split(" ").ToList())
					{
						if (str != null && str != "" && str != " ")
							cos.Add(int.Parse(str));
					}

					foreach(var produsId in cos)
					{
						var ex = exemplare.Where(e => e.ProdusId == produsId).ToList(); // Punem intr-o lista toate exemplarele ale caror produse apar in lista ce reprezinta cosul de cumparaturi
						if (ex.Count() > 0)
						{
							ex[0].Stare = StareExemplar.Comandat; // Exemplarul devine "comandat"
							exCom.Add(ex[0]); // Adaugam in lista de exemplare comandate
						}
						else
						{
							TempData["message"] = "Produsul " +
								(from produs in db.Produse
								 where produs.Id == produsId
								 select produs).ToList()[0].Titlu +
								" nu este disponibil momentan"; // Produsul respectiv din cos nu poate fi comandat (indisponibil)

							return RedirectToAction("Index");
						}
					}
				}

				db.Comenzi.Add(comanda); // Adaugam noua comanda in baza de date
				
				db.SaveChanges();

				foreach(var ex in exCom)
					ex.ComandaId = comanda.Id; // Setam exemplarele comandate cu id-ul noii comenzi adaugate

				HttpContext.Session.SetString("cos", ""); // Save string key "cos" with value "" in session state

				TempData["message"] = "Comanda a fost adaugata!";
				return RedirectToAction("Index");
			}
			return View(comanda);
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult Edit(int id)
		{
			Comanda? comanda = db.Comenzi.Find(id);

			if (comanda == null)
			{
				TempData["message"] = "Comanda cautata nu exista";
				return RedirectToAction("Index");
			}

			ViewBag.userManager = _userManager;

			return View(comanda);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public ActionResult Edit(int id, Comanda reqComanda)
		{
			Comanda? comanda = db.Comenzi.Find(id);

			if (comanda == null)
			{
				TempData["message"] = "Comanda cautata nu exista";
				return RedirectToAction("Index");
			}

			if (ModelState.IsValid)
			{
				// Putem edita comanda doar daca suntem admin si toate validarile au fost respectate
				comanda.Data = reqComanda.Data;
				comanda.Stare = reqComanda.Stare;
				comanda.Valoare = reqComanda.Valoare;

				db.SaveChanges();

				TempData["message"] = "Comanda a fost modificata!";
				return RedirectToAction("Index");
			}
			return View(reqComanda);
		}

		[HttpPost]
		public ActionResult Delete(int id)
		{
			Comanda? comanda = db.Comenzi.Find(id);

			if (comanda == null)
			{
				TempData["message"] = "Comanda cautata nu exista";
				return RedirectToAction("Index");
			}
			if (comanda.UtilizatorId == _userManager.GetUserId(User) || User.IsInRole("Administrator")) // Putem sterge doar comenzile efectuate din propriul cont sau daca suntem admin
			{
				db.Comenzi.Remove(comanda); // Stergem comanda din baza de date
				
				TempData["message"] = "Comanda a fost stearsa!";
				
				db.SaveChanges();

				return RedirectToAction("Index");
			}
			TempData["message"] = "Nu aveti dreptul sa stergeti o comanda care nu va apartine";
			
			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult StergeProdusCos(int id)
		{
			var cosStr = HttpContext.Session.GetString("cos"); // Get string value with key "cos" from session state

			if (cosStr != null)
			{
				List<int> cos = new List<int>();
				foreach (var str in cosStr.Split(" ").ToList())
				{
					if (str != null && str != "" && str != " ")
					{
						if (int.Parse(str) == id)
							continue;

						// In lista vor fi toate id-urile produselor, cu exceptia celui pe care dorim sa-l stergem din cos.
						cos.Add(int.Parse(str)); 
					}
				}

				string newCos = "";
				foreach(int id_ in cos)
				{
					newCos += id_.ToString() + " ";
				}

				HttpContext.Session.SetString("cos", newCos); // Save string key "cos" with string value newCos (id-urile produselor din cos) in session state
			}

			return RedirectToAction("New"); // Ne intoarcem apoi in "New" cu GET din "Comenzi"
		}
	}
}
