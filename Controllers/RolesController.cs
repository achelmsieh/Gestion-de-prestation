using aplication20_07_2019.Models;

using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aplication20_07_2019.Controllers
{
   
    public class RolesController : Controller
    {   
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Roles
        public ActionResult Index()
        { 
            return View(db.Roles.ToList());
        }

        // GET: Roles/Details/5
        public ActionResult Details(String id)
        {
            var role = db.Roles.Find(id);
            if(role==null)
            {
                HttpNotFound();
            }
            return View(role);
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        public ActionResult Create(IdentityRole role)
        {
            try
            {
                
                if(ModelState.IsValid)
                {
                    db.Roles.Add(role);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(role);
            }
            catch
            {
                return View();
            }
        }

        // GET: Roles/Edit/5
        public ActionResult Edit(string id)
        {
            var role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        
        public ActionResult Edit(IdentityRole role)
        {
            
            try
            {
                // TODO: Add update logic here
                if(ModelState.IsValid)
                {
                    db.Entry(role).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(role);
            }
            catch
            {
                return View(role);
            }
        }

        // GET: Roles/Delete/5
        public ActionResult Delete(string id)
        {

            var role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult Delete(IdentityRole myRole)
        {
            
                var role = db.Roles.Find(myRole.Id);
                db.Roles.Remove(role);
                db.SaveChanges();
                return RedirectToAction("Index");
               
            
        }
    }
}
