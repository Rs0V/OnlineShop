using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnlineShop.Models;
using OnlineShop.Data;
using System.Data;


namespace OnlineShop.Controllers
{
    public class CereriController : Controller
    {
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

            ViewBag.Cereri = cereri;
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
           
            if (ModelState.IsValid)
            {
                cerere.ProdusId = request.ProdusId;
                cerere.Produs = request.Produs;
                cerere.Acceptat = request.Acceptat;
                cerere.Respins = request.Respins;
                cerere.Data = request.Data;
                cerere.Info = request.Info;

                TempData["message"] = "Cererea a fost modificata!";
                if (cerere.ProdusId == null && cerere.Produs != null)
                {
                    db.Produse.Add(cerere.Produs);
                    TempData["message"] += " Produsul a fost adaugat";
                }
                else
                if (cerere.ProdusId != null && cerere.Produs != null)
                {
                    var produs = from altprodus in db.Produse
                                 where altprodus.Id == cerere.ProdusId
                                 select altprodus;
                    
                    produs.ElementAt(0).Titlu = cerere.Produs.Titlu;
                    produs.ElementAt(0).Descriere = cerere.Produs.Descriere;
                    produs.ElementAt(0).Pret = cerere.Produs.Pret;
                    produs.ElementAt(0).Poza = cerere.Produs.Poza;
                    produs.ElementAt(0).Rating = cerere.Produs.Rating;
                    produs.ElementAt(0).Id_Categorie = cerere.Produs.Id_Categorie;
                    TempData["message"] += " Produsul a fost editat!";
                }
                else
                if(cerere.ProdusId != null && cerere.Produs == null)
                {
                    var produs = from altprodus in db.Produse
                                 where altprodus.Id == cerere.ProdusId
                                 select altprodus;

                    db.Produse.Remove(produs.ElementAt(0));
                    TempData["message"] = "Produsul a fost sters!";
                    
                }


                db.SaveChanges();
                
                return RedirectToAction("Index");
            }
            return View(request);
            
               
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

