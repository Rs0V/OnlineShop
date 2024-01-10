using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
	public class ComenziController : Controller
	{
		private readonly ApplicationDbContext db;
		private readonly UserManager<Utilizator> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		public ComenziController(
		ApplicationDbContext context,
		UserManager<Utilizator> userManager,
		RoleManager<IdentityRole> roleManager
		)
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

			ViewBag.Comenzi = comenzi;
			ViewBag.Utilizatori = utilizatori;
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
			if (comanda.UtilizatorId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
				return View(comanda);

			TempData["message"] = "Nu aveti dreptul sa vizualizati o comanda care nu va apartine";

			var utilizatori = from utilizator in db.Utilizatori
							  select utilizator;
			ViewBag.Utilizatori = utilizatori;

			return RedirectToAction("Index");
		}

		[Authorize(Roles = "Utilizator,Colaborator,Administrator")]
		[HttpGet]
		public ActionResult New()
		{
			var produse = from produs in db.Produse
						  where 1 == 2
						  select produs; // Query Vid
			var cosStr = HttpContext.Session.GetString("cos");
			if (cosStr != null)
			{
				List<int> cos = new List<int>();
				// var a = cosStr.Split(" ").ToList();
				foreach (var str in cosStr.Split(" ").ToList())
				{
					if(str != null && str != "" && str != " ")
						cos.Add(int.Parse(str));
				}

				produse = from produs in db.Produse
						  where cos.Contains(produs.Id)
						  select produs;
			}

			ViewBag.Produse = produse;

			return View();
		}

		[Authorize(Roles = "Utilizator,Colaborator,Administrator")]
		[HttpPost]
		public ActionResult New(Comanda comanda)
		{
			if (ModelState.IsValid)
			{
				db.Comenzi.Add(comanda);
				db.SaveChanges();
				TempData["message"] = "Comanda a fost adaugata!";
				return RedirectToAction("Index");
			}

			///

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
				comanda.Data = reqComanda.Data;
				comanda.Stare = reqComanda.Stare;
				comanda.Valoare = reqComanda.Valoare;
				comanda.UtilizatorId = reqComanda.UtilizatorId;

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
			if (comanda.UtilizatorId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
			{
				db.Comenzi.Remove(comanda);
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
			var cosStr = HttpContext.Session.GetString("cos");
			if (cosStr != null)
			{
				List<int> cos = new List<int>();
				foreach (var str in cosStr.Split(" ").ToList())
				{
					if (str != null && str != "" && str != " ")
					{
						if (int.Parse(str) == id)
							continue;

						cos.Add(int.Parse(str));
					}
				}

				string newCos = "";
				foreach(int id_ in cos)
				{
					newCos += id_.ToString() + " ";
				}
				HttpContext.Session.SetString("cos", newCos);
			}

			return RedirectToAction("New");
		}
	}
}
