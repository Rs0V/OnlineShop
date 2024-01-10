using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
	public class UtilizatoriController : Controller
	{
		private readonly ApplicationDbContext db;
		private readonly UserManager<Utilizator> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		public UtilizatoriController(
		ApplicationDbContext context,
		UserManager<Utilizator> userManager,
		RoleManager<IdentityRole> roleManager
		)
		{
			db = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		[Authorize(Roles = "Administrator,Colaborator")]
		public ActionResult Index()
		{
			if (TempData.ContainsKey("message"))
				ViewBag.message = TempData["message"].ToString();

			var utilizatori = from utilizator in db.Utilizatori
							  orderby utilizator.Nume, utilizator.Prenume
							  select utilizator;

			ViewBag.Utilizatori = utilizatori;
			ViewBag.userManager = _userManager;
			return View();
		}

		public ActionResult Show(string id)
		{
			Utilizator? utilizator = db.Utilizatori.Find(id);
			ViewBag.goodlog = false;

			if (utilizator == null)
			{
				TempData["message"] = "Utilizatorul cautat nu exista";
				return RedirectToAction("Index");
			}
			if (utilizator.Id == _userManager.GetUserId(User))
			{
				ViewBag.goodlog = true;
				return View(utilizator);
			}
			else if (User.IsInRole("Utilizator") == false)
			{
				return View(utilizator);
			}
			TempData["message"] = "Nu aveti dreptul sa vizualizati un cont care nu va apartine";
			return RedirectToAction("Index");
		}

		public ActionResult New()
		{
			return View();
		}

		[HttpPost]
		public ActionResult New(Utilizator utilizator)
		{
			if (ModelState.IsValid)
			{
				db.Utilizatori.Add(utilizator);
				db.SaveChanges();
				TempData["message"] = "Utilizatorul a fost inregistrat";
				return RedirectToAction("Index");
			}
			return View(utilizator);
		}


		public ActionResult Edit(string id)
		{
			Utilizator? utilizator = db.Utilizatori.Find(id);

			if (utilizator == null)
			{
				TempData["message"] = "Utilizatorul cautat nu exista";
				return RedirectToAction("Index");
			}
			if (utilizator.Id == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
				return View(utilizator);

			TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui cont care nu va apartine";
			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult Edit(string id, Utilizator reqUtilizator)
		{
			Utilizator? utilizator = db.Utilizatori.Find(id);

			if (utilizator == null)
			{
				TempData["message"] = "Utilizatorul cautat nu exista";
				return RedirectToAction("Index");
			}
			if (utilizator.Id == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
			{
				if (ModelState.IsValid)
				{
					utilizator.Nume = reqUtilizator.Nume;
					utilizator.Prenume = reqUtilizator.Prenume;

					utilizator.UserName = reqUtilizator.UserName;
					utilizator.NormalizedUserName = reqUtilizator.NormalizedUserName;

					utilizator.Email = reqUtilizator.Email;
					utilizator.NormalizedEmail = reqUtilizator.NormalizedEmail;
					utilizator.EmailConfirmed = true;

					utilizator.Telefon = reqUtilizator.Telefon;

					// var hasher = new PasswordHasher<Utilizator>();
					utilizator.PasswordHash = reqUtilizator.PasswordHash;

					db.SaveChanges();
					TempData["message"] = "Utilizatorul a fost modificat!";
					return RedirectToAction("Index");
				}
				return View(reqUtilizator);
			}
			TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui cont care nu va apartine";
			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult Delete(string id)
		{
			Utilizator? utilizator = db.Utilizatori.Find(id);

			if (utilizator == null)
			{
				TempData["message"] = "Utilizatorul cautat nu exista";
				return RedirectToAction("Index");
			}
			if (utilizator.Id == _userManager.GetUserId(User) || User.IsInRole("Administrator"))
			{
				db.Utilizatori.Remove(utilizator);
				TempData["message"] = "Utilizatorul a fost sters!";
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			TempData["message"] = "Nu aveti dreptul sa stergeti un cont care nu va apartine";
			return RedirectToAction("Index");
		}
	}
}
