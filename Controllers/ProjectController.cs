using aplication20_07_2019.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aplication20_07_2019.Controllers
{
    [Authorize(Roles = "responsable_marche")]
    public class ProjectController : Controller
    {
        market_appEntities dbmetier = new market_appEntities();
        // GET: Project
        public ActionResult Index()
        {
            List<Projet> list = dbmetier.Projets.ToList();
            return View(list);
        }

        // GET: Project/Details/5
        public ActionResult Details(int id_project)
        {
            Projet projet = dbmetier.Projets.Find(id_project);
            
            return View(projet);
        }

        // GET: Project/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Project/Create
        [HttpPost]
        public ActionResult Create(Projet project)
        {
            try
            {
                dbmetier.Projets.Add(project);
                dbmetier.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Project/Edit/5
        public ActionResult Edit(int id_project)
        {
            
            return View(dbmetier.Projets.Find(id_project));
        }

        // POST: Project/Edit/5
        [HttpPost]
        public ActionResult Edit( Projet projet)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    dbmetier.Entry(projet).State = EntityState.Modified;
                    dbmetier.SaveChanges();
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Project/Delete/5
        public ActionResult Delete(int id_projet)
        {
            
            return View(dbmetier.Projets.Find(id_projet));
        }

        // POST: Project/Delete/5
        [HttpPost]
        public ActionResult Delete(Projet projet)
        {

            
            Projet project = dbmetier.Projets.Find(projet.id_projet);
            ICollection <Marché> listmarche = projet.Marché;
            if (listmarche != null)
            {
                foreach (var ite in listmarche)
                {
                    Marché marche = dbmetier.Marché.Find(ite.id_marché);
                    //les cle de la liste
                    List<int> listmissimonid = marche.Missions.Select(a => a.id_mission).ToList();
                    foreach (var item in listmissimonid)
                    {
                        Mission mission = dbmetier.Missions.Find(item);
                        //les cle de la liste 
                        List<int> listidaff = mission.Affectation_profils.Select(a => a.id_affectation).ToList();
                        foreach (var item1 in listidaff)
                        {

                            dbmetier.Affectation_profils.Remove(dbmetier.Affectation_profils.Find(item1));
                            dbmetier.SaveChanges();
                        }

                        dbmetier.Missions.Remove(mission);
                        dbmetier.SaveChanges();
                    }
                    if (marche.Participations != null)
                    {
                        List<int> liscleparticipation = marche.Participations.Select(a => a.id_participation).ToList();
                        foreach (var item in liscleparticipation)
                        {
                            dbmetier.Participations.Remove(dbmetier.Participations.Find(item));
                        }

                    }
                    dbmetier.Marché.Remove(marche);
                    dbmetier.SaveChanges();
                }
            }
            
            dbmetier.Projets.Remove(project);
            dbmetier.SaveChanges();
                return RedirectToAction("Index");
            
            
        }
        // une fonction qui permet d'ajoute un nv. profil 
        public ActionResult ajouter_profils(string liblle_profil)
        {
            int nombre = dbmetier.Profils.Where(a => a.libelle_profil == liblle_profil).Count();
            if (nombre == 0 && liblle_profil!= null)
                dbmetier.Profils.Add(new Profil
                {
                    libelle_profil = liblle_profil
                });
            dbmetier.SaveChanges();
            return RedirectToAction("Create","Marche"); }
    }
}
