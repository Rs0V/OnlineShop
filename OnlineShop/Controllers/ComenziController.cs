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
			ViewBag.Comenzi = comenzi;
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
			return RedirectToAction("Index");
		}

		[Authorize(Roles = "Utilizator,Colaborator,Administrator")]
		public ActionResult New()
		{
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
			return View(comanda);
		}

		public ActionResult Edit(int id)
		{
			Comanda? comanda = db.Comenzi.Find(id);

			if (comanda == null)
			{
				TempData["message"] = "Comanda cautata nu exista";
				return RedirectToAction("Index");
			}
			if (comanda.UtilizatorId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
				return View(comanda);

			TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei comenzi care nu va apartine";
			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult Edit(int id, Comanda reqComanda)
		{
			Comanda? comanda = db.Comenzi.Find(id);

			if (comanda == null)
			{
				TempData["message"] = "Comanda cautata nu exista";
				return RedirectToAction("Index");
			}
			if (comanda.UtilizatorId == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
			{
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
			TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei comenzi care nu va apartine";
			return RedirectToAction("Index");
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
	}
}
