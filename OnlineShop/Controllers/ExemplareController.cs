using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Data;
using System.Data;

namespace OnlineShop.Controllers
{
	public class ExemplareController : Controller
	{
		private readonly ApplicationDbContext db;

		public ExemplareController(ApplicationDbContext context)
		{
			db = context;
		}

		[Authorize(Roles = "Colaborator,Administrator")]
		public ActionResult Index()
		{
			if (TempData.ContainsKey("message"))
				ViewBag.message = TempData["message"].ToString();

			var exemplare = from exemplar in db.Exemplare
							join produs in db.Produse on exemplar.ProdusId equals produs.Id
							orderby produs.Titlu, exemplar.Numar_Produs
							select exemplar;

			ViewBag.Exemplare = exemplare;
			return View();
		}

		[Authorize(Roles = "Colaborator,Administrator")]
		public ActionResult Show(int id_produs, int nr_produs)
		{
			Exemplar? exemplar = db.Exemplare.Find(id_produs, nr_produs);

			if (exemplar == null)
			{
				TempData["message"] = "Exemplarul cautat nu exista";
				return RedirectToAction("Index");
			}
			return View(exemplar);
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult New()
		{
			return View();
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public ActionResult New(Exemplar exemplar)
		{
			if (ModelState.IsValid)
			{
				db.Exemplare.Add(exemplar);
				db.SaveChanges();
				TempData["message"] = "Exemplarul a fost adaugat";
				return RedirectToAction("Index");
			}
			return View(exemplar);
		}

		[Authorize(Roles = "Colaborator,Administrator")]
		public ActionResult Edit(int id_produs, int nr_produs)
		{
			Exemplar? exemplar = db.Exemplare.Find(id_produs, nr_produs);

			if (exemplar == null)
			{
				TempData["message"] = "Exemplarul cautat nu exista";
				return RedirectToAction("Index");
			}
			return View(exemplar);
		}

		[Authorize(Roles = "Colaborator,Administrator")]
		[HttpPost]
		public ActionResult Edit(int id_produs, int nr_produs, Exemplar reqEx)
		{
			Exemplar? exemplar = db.Exemplare.Find(id_produs, nr_produs);

			if (exemplar == null)
			{
				TempData["message"] = "Exemplarul cautat nu exista";
				return RedirectToAction("Index");
			}
			if (ModelState.IsValid)
			{
				exemplar.Stare = reqEx.Stare;
				exemplar.ComandaId = reqEx.ComandaId;

				db.SaveChanges();
				TempData["message"] = "Exemplarul a fost modificat!";
				return RedirectToAction("Index");
			}
			return View(reqEx);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public ActionResult Delete(int id_produs, int nr_produs)
		{
			Exemplar? exemplar = db.Exemplare.Find(id_produs, nr_produs);

			if (exemplar == null)
			{
				TempData["message"] = "Exemplarul cautat nu exista";
				return RedirectToAction("Index");
			}
			db.Exemplare.Remove(exemplar);
			TempData["message"] = "Exemplarul a fost sters";
			db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}
