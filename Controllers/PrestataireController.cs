using aplication20_07_2019.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aplication20_07_2019.Controllers
{
    [Authorize(Roles = "prestataire, responsable_marche, admin")]
    public class PrestataireController : Controller
    {
        market_appEntities dbmetier =new market_appEntities();
        // GET: Prestataire
        //cet methode sera vu que par le responsable marche
        [Authorize(Roles = " responsable_marche,admin")]
        public ActionResult Index()
        {     
            return View(dbmetier.prestataires.ToList());
        }
        [HttpPost]
        public ActionResult Modifier_image_profil(HttpPostedFileBase file)
        {
            if (file != null)
            {
                prestataire pre = dbmetier.prestataires.Find(User.Identity.GetUserId());
                //get the bytes from the uploaded file
                byte[] data = GetBytesFromFile(file);
                pre.image_prestataire= data ;
                dbmetier.SaveChanges();
                
            }

            return RedirectToAction("Information_prstataire", "Prestataire");
        }
        private byte[] GetBytesFromFile(HttpPostedFileBase file)
        {
            using (Stream inputStream = file.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                return memoryStream.ToArray();
            }
        }


        // GET: Prestataire/Details/5
        [Authorize(Roles = " responsable_marche,admin")]
        public ActionResult Details(string id_prestataire)
        {   
            var pres = dbmetier.prestataires.Find(id_prestataire);
            if (pres == null)
            {
                HttpNotFound();
            }
            return View(pres); 
        }

        // utlisliser la vue ci dessos
        // show the information about le prestataire
        private static Image ConvertToImage(byte[] arrayBinary)
        {
            Image rImage = null;

            using (MemoryStream ms = new MemoryStream(arrayBinary))
            {
                rImage = Image.FromStream(ms);
            }
            return rImage;
        }
        [Authorize(Roles = "prestataire")]
        [HttpGet]
        public ActionResult Information_prstataire()
        { 
            var pres = dbmetier.prestataires.Find(User.Identity.GetUserId());
            if (pres == null)
            {
                HttpNotFound();
            }
            ViewBag.image = Convert.ToBase64String(pres.image_prestataire);
            Convert.ToBase64String(pres.image_prestataire);
            return View(pres);
        }
        

        // GET: Prestataire/Create
        [Authorize(Roles = " admin")]
        public ActionResult Create()
        {
            return View();
            // on doit cree un utilisateur
        }

        // POST: Prestataire/Create
        [HttpPost]
        [Authorize(Roles = " admin")]
        public ActionResult Create(prestataire pres)
        {
            // TODO: Add insert logic here
            if (ModelState.IsValid)
            {
                dbmetier.prestataires.Add(pres);
                dbmetier.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pres);
        
            
    
        }

        // GET: Prestataire/Edit/5

        [Authorize(Roles = " admin")]
        public ActionResult Edit(int id)
        {
                return View();
        }

        // POST: Prestataire/Edit/5
        [HttpPost]
        [Authorize(Roles = " admin")]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Prestataire/Delete/5
        [Authorize(Roles = " admin")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Prestataire/Delete/5
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
