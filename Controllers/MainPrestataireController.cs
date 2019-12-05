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
    public class MainPrestataireController : Controller
    {
        market_appEntities dbmetier = new market_appEntities();
        // GET: MainPrestataire
        [HttpGet]
        public ActionResult Index()
        {
            var id_prestataire = User.Identity.GetUserId();
            IList<Marché> liste_marche_propose = dbmetier.Participations.Where(a => a.id_prestataire_participation == id_prestataire && a.budget_prestation == null).Select(a => a.Marché).Where(a => a.id_prestataire_final == null).ToList();
            IList<Marché> liste_marche_enattente = dbmetier.Participations.Where(a => a.id_prestataire_participation == id_prestataire && a.budget_prestation != null).Select(a => a.Marché).Where(a => a.id_prestataire_final == null).ToList();
            IList<Marché> listMarche = dbmetier.Marché.Where(a => a.id_prestataire_final == id_prestataire).ToList();
            IList<Marché> listMarche_encour = new List<Marché>();
            bool istrue;
            foreach (var item in listMarche)
            {
                istrue = false;
                foreach (var item1 in item.Missions)
                {
                    if (item1.Ordre_service == null)
                        istrue = true;
                    else
                    {
                        if (item1.Ordre_service.etat_facturation == false)
                        {
                            istrue = true;
                        }
                    }
                }
                if (istrue == true)
                    listMarche_encour.Add(item);
            }



            ViewBag.marche_encour = listMarche_encour;
            ViewBag.marche_propose = liste_marche_propose;
            ViewBag.marche_enattent= liste_marche_enattente;
            
            return View();
        }
    
    }
}