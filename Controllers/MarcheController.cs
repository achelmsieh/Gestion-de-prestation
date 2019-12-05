using aplication20_07_2019.Models;
using aplication20_07_2019.Models.viewmodel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace aplication20_07_2019.Controllers
{
    [Authorize(Roles = "prestataire,responsable_marche")]
    public class MarcheController : Controller
    {
        // GET: Marche
        market_appEntities dbmetier = new market_appEntities();


        [Authorize(Roles = "responsable_marche ")]
        public ActionResult Mes_Prestatire()
        {
           var list = dbmetier.Marché.Where(a => a.id_responsable_fk == User.Identity.GetUserId()).GroupBy(a=>a.prestataire).ToList();

            return null;
        }
        
        public ActionResult dashboard()
        {
            return View();
        }

        // cette fonction doit etre la couche metier de mission 
        public bool? etat_ordre_service( int id)
        {
            Ordre_service ordre= dbmetier.Ordre_service.Find(id);
            
            return ordre.etat_reception;
        }

        [Authorize(Roles = "responsable_marche ,prestataire")]
        public ActionResult Dashboardresp(int id_marche)
        {
            int totale_mission;
            IList<Mission> list_desmission_encour = new List<Mission>();
            IList<Mission> list_desmission_nonlances = new List<Mission>();
            IList<Mission> list_desmission_complete = new List<Mission>();
            IList<Mission> list_desmission_annule = new List<Mission>();
            IList<Mission> list_desmission = dbmetier.Missions.Where(a => a.id_marche_mission == id_marche).ToList();
            foreach (var item in list_desmission)
            {
                if (item.Ordre_service == null)
                {
                    list_desmission_nonlances.Add(item);
                }
                else
                {   if (item.Affectation_profils.Count == 0)
                        list_desmission_annule.Add(item);
                    if (etat_ordre_service(item.id_ordreservice_fk) == true && item.Affectation_profils.Count != 0)
                    {
                        list_desmission_complete.Add(item);
                    }
                    if (etat_ordre_service(item.id_ordreservice_fk) == false && item.Affectation_profils.Count != 0)
                    {
                        list_desmission_encour.Add(item);
                    }
                }
                ViewBag.list_desmission_nonlances = list_desmission_nonlances;
                ViewBag.list_desmission_complete = list_desmission_complete;
                ViewBag.list_desmission_encour = list_desmission_encour;
                ViewBag.list_desmission_annule = list_desmission_annule;
            }
            totale_mission = list_desmission.Count;
            if (totale_mission != 0)
            {
               
            ViewBag.pourcentage_nonlances = ((double)list_desmission_nonlances.Count / totale_mission)* 100;
            ViewBag.pourcentage_complete = ((double )list_desmission_complete.Count / totale_mission) * 100;
            ViewBag.pourcentage_encour = ((double)list_desmission_encour.Count / totale_mission) * 100;
            ViewBag.pourcentage_annulle = ((double)list_desmission_annule.Count / totale_mission) * 100;
            }
            int nombre_res=0, nombre_non=0;

            IList<Ordre_service> liste_ordre = dbmetier.Marché.Find(id_marche).Projet.Ordre_service.ToList();
            foreach (var item in liste_ordre)
            {
                if (item.etat_reception == true)
                    nombre_res++;
                else
                    nombre_non++;
            }
            ViewBag.pourcentage_ordre_recep =nombre_res;
            ViewBag.pourcentage_cordre_nonrecep =nombre_non;
        // ordre de service
        IList<Ordre_service> list_ordre = new List<Ordre_service>() ;
            //listmision view model
            List<MVM> listMissionVM = new List<MVM>();
            MVM element = new MVM();
            foreach (var item in list_desmission)
            {
                Ordre_service ordre = dbmetier.Ordre_service.Where(a => a.id_ordre_service == item.id_ordreservice_fk).FirstOrDefault();
                if (!list_ordre.Contains(ordre)&& ordre!=null)
                    list_ordre.Add(ordre);
                
           
            }
            Marché marche = dbmetier.Marché.Find(id_marche);
            int id_participation = dbmetier.Participations.Where(a => a.id_prestataire_participation == marche.id_prestataire_final && a.id_marché_participation == marche.id_marché).Select(a=>a.id_participation).FirstOrDefault();
            // on peut creer un liste de misiion non affecte a un ordre de service ici.
            foreach (var item in list_desmission)
            { int cout ,cout_totale=0;
                List<Affectation_profils> listaffectations = dbmetier.Affectation_profils.Where(a => a.id_mission_affectation == item.id_mission).ToList();

                foreach (var item2 in listaffectations)
                {
                    cout = (int)dbmetier.Participation_profil.Where(a => a.id_profil_fk == item2.Profil.id_profil && a.id_participation_fk == id_participation).Select(a => a.cout_unit_ht).FirstOrDefault();
                    cout_totale += cout * (int)item2.charge_profil;
                }
                element = new MVM { mission = item, cout = cout_totale };
                listMissionVM.Add(element);
            }
            ViewBag.listmission = listMissionVM;
            ViewBag.listeordre = list_ordre;
            ViewBag.Marche = marche;
            ViewBag.prestataire = dbmetier.prestataires.Find(marche.id_prestataire_final);
            var listedomaines = dbmetier.Domaines.ToList();
            ViewBag.listedomaines = new SelectList(listedomaines, "libelle_domaine", "libelle_domaine");
            var listProfils = dbmetier.Marché.Find(id_marche).Participations.Where(a => a.id_prestataire_participation == marche.id_prestataire_final).FirstOrDefault().Participation_profil.ToList().Select(a => a.Profil);
            ViewBag.listeProfils = new SelectList(listProfils, "libelle_profil", "libelle_profil");
            return View();
            

        }
        // ce quoi le role de cette dernier
        [HttpPost]
        [Authorize(Roles = "responsable_marche")]
        public ActionResult ajouteMission(int id_ordre, List<int>listmission,int id_marche)
        {

            if (User.Identity.GetUserId() == dbmetier.Marché.Where(a => a.id_marché == id_marche).Select(a => a.id_responsable_fk).FirstOrDefault())
            {
                Ordre_service ordre = dbmetier.Ordre_service.Find(id_ordre);
                
                foreach (var item in listmission)
                {
                    Mission mission = new Mission();
                    mission = dbmetier.Missions.Find(item);
                    mission.id_ordreservice_fk = ordre.id_ordre_service;
                }
                dbmetier.SaveChanges();
            }
            return RedirectToAction("Dashboardresp", new { id_marche = id_marche });
        }
        [HttpPost]
        [Authorize(Roles = "responsable_marche")]
        public ActionResult ajouterMission(MVM mission, int id_ordre, int id_marche  )
        {

            //ajout de domaine 
            int budget_mission=0,budget_restant=0;
            Participation par= dbmetier.Participations.Where(a => a.id_marché_participation == id_marche).First();
            ICollection<Participation_profil> listparti = par.Participation_profil;
            foreach (var item in mission.listeprofils)
            {
                budget_mission += item.charge * (int) listparti.Where(a => a.Profil.libelle_profil == item.libelle).Select(a => a.cout_unit_ht).FirstOrDefault();
            }

            Participation participation= dbmetier.Participations.Where(a => a.id_marché_participation == id_marche).FirstOrDefault();
            Marché marche = dbmetier.Marché.Find(id_marche);
            
            budget_restant = (int)participation.budget_prestation -(int) marche.montant_total;
            if (budget_mission <= budget_restant)
            {
                marche.montant_total +=budget_mission;
                Ordre_service ordre = dbmetier.Ordre_service.Find(id_ordre);
                ordre.date_modification= System.DateTime.Now;
                Domaine domaine = dbmetier.Domaines.Where(x => x.libelle_domaine == mission.mission.Domaine.libelle_domaine).FirstOrDefault();
                Mission mision = new Mission
                {
                    libelle_mission = mission.mission.libelle_mission,
                    id_domaine_mission_fk = domaine.id_domaine,
                    id_marche_mission = marche.id_marché
                    ,
                    id_ordreservice_fk = ordre.id_ordre_service
                };
                dbmetier.Missions.Add(mision);
                dbmetier.SaveChanges();
                foreach (var profillib in mission.listeprofils)
                {
                    Profil prof = dbmetier.Profils.Where(x => x.libelle_profil == profillib.libelle).FirstOrDefault();
                    Affectation_profils afectation = new Affectation_profils
                    {
                        id_mission_affectation = mision.id_mission,
                        id_profils_affectation = prof.id_profil,
                        charge_profil = profillib.charge
                    };
                    dbmetier.Affectation_profils.Add(afectation);
                    dbmetier.SaveChanges();

                }
            }
                  
            return null;
        }
        [HttpPost]
        [Authorize(Roles = "responsable_marche")]
        public ActionResult reception(int id_ordre)
        {
            Ordre_service ordre = dbmetier.Ordre_service.Find(id_ordre);
            ordre.etat_reception = true;
            ordre.date_fin_ordre = System.DateTime.Now;
            dbmetier.SaveChanges();
            return RedirectToAction("Dashboardresp" );
        }
        [HttpPost]
        [Authorize(Roles = "responsable_marche")]
        public ActionResult facturation(int id_ordre)
        {
            Ordre_service ordre = dbmetier.Ordre_service.Find(id_ordre);
            ordre.etat_facturation = true;
            dbmetier.SaveChanges();
            return null;
        }
        [HttpPost]
        [Authorize(Roles = "responsable_marche")]
        public ActionResult fin_Mission(int id_marche, int id_mission)
        {
            Mission mission=dbmetier.Missions.Find(id_mission);
            mission.date_fin_mission = System.DateTime.Now;
            Ordre_service ordre = dbmetier.Ordre_service.Find(mission.Ordre_service.id_ordre_service);
            bool isrecptione = true;
            
            foreach (var item in ordre.Missions)
            {
                if (item.date_fin_mission == null)
                isrecptione = false; 
            }
            if (isrecptione == true)
            {
                ordre.etat_reception = true;
                ordre.date_fin_ordre = mission.date_fin_mission;
            }
            dbmetier.SaveChanges();
            return RedirectToAction("Dashboardresp", new { id_marche = id_marche }); ;
        }
        [HttpPost]
        [Authorize(Roles = "responsable_marche")]
        public ActionResult annler_mision(int id_marche, int id_mission_annulée)
        {
            Marché mar= dbmetier.Marché.Find(id_marche);
            Affectation_profils aff;
            Mission mission = dbmetier.Missions.Where(a => a.id_mission == id_mission_annulée).FirstOrDefault();
            mission.date_fin_mission = System.DateTime.Now;
            ICollection<Affectation_profils> listaf = new List<Affectation_profils>();



            int budget_mission = 0;
            
            foreach (var item in mission.Affectation_profils)
            {

                Profil profil = item.Profil;
                Participation par= dbmetier.Participations.Where(a => a.id_marché_participation == id_marche).FirstOrDefault();
                Participation_profil particpro = dbmetier.Participation_profil.Where(a => a.id_profil_fk == profil.id_profil && a.id_participation_fk ==(int) par.id_participation).FirstOrDefault();
                budget_mission += (int) item.charge_profil * (int)particpro.cout_unit_ht;
            }
            mar.montant_total -= budget_mission;
            dbmetier.SaveChanges();


            foreach (var item in mission.Affectation_profils)
            {
                 aff = new Affectation_profils { charge_profil = item.charge_profil,id_affectation=item.id_affectation,id_mission_affectation=item.id_mission_affectation,id_profils_affectation=item.id_profils_affectation,Mission=item.Mission,Profil=item.Profil };
                
                listaf.Add(aff);
            }
            foreach (var item in listaf)
            {
                aff = dbmetier.Affectation_profils.Where(a => a.id_affectation == item.id_affectation).FirstOrDefault();
                dbmetier.Affectation_profils.Remove(aff);
            }  
            dbmetier.SaveChanges();
            return RedirectToAction("Dashboardresp", new { id_marche = id_marche }); ;
        }
        [HttpPost]
        [Authorize(Roles = "responsable_marche")]
        public ActionResult Creer_ordre(DateTime date_provisoire,List<int> listmission, int id_marche)
        {
            if (User.Identity.GetUserId() == dbmetier.Marché.Where(a => a.id_marché == id_marche).Select(a => a.id_responsable_fk).FirstOrDefault())
            {   Marché marche = dbmetier.Marché.Find(id_marche);
                Ordre_service ordre_service = new Ordre_service
                {
                    date_debut_ordre = System.DateTime.Now,
                    date_provisoire= date_provisoire,
                    id_projet_ordreservice=marche.Projet.id_projet,
                    etat_reception=false,
                    etat_facturation=false,
                    
                };
                dbmetier.Ordre_service.Add(ordre_service);
                dbmetier.SaveChanges();
                foreach (var item in listmission)
                {
                    Mission mission = new Mission();
                    mission = dbmetier.Missions.Find(item);
                    mission.date_debut_mission = System.DateTime.Now;
                    mission.id_ordreservice_fk = ordre_service.id_ordre_service;
                }
                dbmetier.SaveChanges();
            }
                return RedirectToAction("Dashboardresp", new { id_marche = id_marche });
        }

        private bool etat_ordre_service(int? id_ordreservice_fk)
        {

            return (bool) dbmetier.Ordre_service.Where(a => a.id_ordre_service == id_ordreservice_fk).Select(a => a.etat_reception).FirstOrDefault();
            
        }
        [Authorize(Roles = "responsable_marche")]
        public ActionResult MarcheencourRsp()
        {
            var userId = User.Identity.GetUserId();
            IList<Marché> listMarche = dbmetier.Marché.Where(a => a.id_responsable_fk == userId).ToList();
            IList<Marché> listMarche_encour = new List<Marché>();
            bool istrue;
            foreach (var item in listMarche)
            {   istrue = false;
                foreach (var item1 in item.Missions)
                {   if (item1.Ordre_service == null)
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
            return View(listMarche_encour); 

        }
        // GET: Marche/Details/5
        //prestataire
        public ActionResult Details_Participation(int id_marche)
        {   // fonction de vérification de presence du profil
            bool test(string str1, List<Models.viewmodel.Profil_cout_chargeVM> pc)
            {
                foreach (var it in pc)
                {
                    if (str1 == it.libelle)
                        return true;
                }
                return false;
            }
            var marche=dbmetier.Marché.Find(id_marche);
            var listeMission = dbmetier.Missions.Where(a => a.id_marche_mission == id_marche).ToList();
            var Projet = dbmetier.Projets.Where(a => a.id_projet == marche.id_projet_fk).FirstOrDefault();
            ViewBag.responsable = dbmetier.Responsable_marché.Where(r => r.id_responsable == marche.id_responsable_fk).FirstOrDefault();
            ViewBag.projet = Projet;
            List<MissionViewModel> listmissionvm= new List<MissionViewModel>();
            MissionViewModel element=new MissionViewModel(); ;
            List<Models.viewmodel.Profil_cout_chargeVM> profilschrgelist = new List<Models.viewmodel.Profil_cout_chargeVM>();
            Models.viewmodel.Profil_cout_chargeVM profilchar=new Models.viewmodel.Profil_cout_chargeVM();
            foreach (var item in listeMission)
            {
                element = new MissionViewModel();
                element.libelle_mission = item.libelle_mission;
                element.date_debut_mission = item.date_debut_mission;
                element.date_fin_mission = item.date_fin_mission;
                element.domaine = item.Domaine.libelle_domaine;

                List<Affectation_profils> liste_afectation = dbmetier.Affectation_profils.Where(a => a.id_mission_affectation == item.id_mission).ToList();
                profilschrgelist = new List<Models.viewmodel.Profil_cout_chargeVM>();
                foreach (var item2 in liste_afectation)
                {
                    profilchar = new Models.viewmodel.Profil_cout_chargeVM { charge =(int) item2.charge_profil, libelle = item2.Profil.libelle_profil };

                profilschrgelist.Add(profilchar);
                }
                element.listeprofils = profilschrgelist;
                listmissionvm.Add(element);
            }



            var marcheviewmodel = new MarcheViewModel
            { id_marché = marche.id_marché,
                date_debut = marche.date_debut,
                matricule= marche.matricule,
                date_fin= marche.date_fin,
                Délai= marche.Délai,
                budget_prevu= marche.budget_prevu,
                listMission= listmissionvm
                
            };
            // une liste pour remplir le budget de participation 
            List<Models.viewmodel.Profil_cout_chargeVM> liste_chargeparprofil = new List<Models.viewmodel.Profil_cout_chargeVM>();
            List<Models.viewmodel.Profil_cout_chargeVM> liste_chargeparprofilnontrie = new List<Models.viewmodel.Profil_cout_chargeVM>();
            foreach (MissionViewModel smiya in marcheviewmodel.listMission)
            {
                
                liste_chargeparprofilnontrie.AddRange(smiya.listeprofils);
            }
            foreach(Models.viewmodel.Profil_cout_chargeVM elem in liste_chargeparprofilnontrie)
            {
                if(!test (elem.libelle,liste_chargeparprofil) )
                {
                    liste_chargeparprofil.Add(new Models.viewmodel.Profil_cout_chargeVM { libelle = elem.libelle, charge = elem.charge });
                }
                else
                {foreach (Models.viewmodel.Profil_cout_chargeVM pc in liste_chargeparprofil)
                        if (elem.libelle == pc.libelle)
                             pc.charge = elem.charge+ pc.charge;
                }
            }
            ViewBag.liste_chargeparprofil = liste_chargeparprofil;




            return View(marcheviewmodel);
        }

        [Authorize(Roles = "responsable_marche")]
        public ActionResult MarcheENAttentRSP()
        {
            var userId = User.Identity.GetUserId();
            List<Marché> listmarche = dbmetier.Marché.Where(a=>a.id_responsable_fk==userId&&a.id_prestataire_final==null).ToList();
            List<Marche_nombreParticipation> listpar = new List<Marche_nombreParticipation>();
            foreach (Marché item in listmarche)
            {

                Marche_nombreParticipation var = new Marche_nombreParticipation
                {
                    marche = item,
                    nombre_patipation = dbmetier.Participations.Where(a => a.id_marché_participation == item.id_marché && a.budget_prestation != null).ToList().Count(),
                    nombre_prestataire_affecte = dbmetier.Participations.Where(a => a.id_marché_participation == item.id_marché ).ToList().Count()
                };
                if(var.nombre_patipation>0)
                    listpar.Add(var);
            }
            return View(listpar);
        }
        [Authorize(Roles = "responsable_marche")]
        [HttpGet]// javais ici un probleme de confusion entre le get et le pose d'ou cette solution 
        public ActionResult affect(String id_prestatire, int id_marche)
        {//securite
            if (User.Identity.GetUserId() == dbmetier.Marché.Where(a => a.id_marché == id_marche).Select(a => a.id_responsable_fk).FirstOrDefault())
            {
                Marché marche = new Marché();
                marche = dbmetier.Marché.Find(id_marche);
                Participation par = dbmetier.Participations.Where(a => a.id_prestataire_participation == id_prestatire && a.id_marché_participation == id_marche).FirstOrDefault();
                marche.montant_total = par.budget_prestation;
                marche.id_prestataire_final = id_prestatire;
                //SUprimer tous les autre participation 
                List<Participation> listparticipation = dbmetier.Participations.Where(a => a.id_marché_participation == id_marche).ToList();
                foreach (var Participation in listparticipation)
                {
                    if(Participation.id_prestataire_participation!=id_prestatire)
                    dbmetier.Participations.Remove(Participation);

                }
                dbmetier.SaveChanges();
            }

            return RedirectToAction("Dashboardresp", new { id_marche = id_marche });
        }
        [Authorize(Roles = "responsable_marche")]
        [HttpGet]
        public ActionResult affecte( int id_marche)
        {// reepet de delais 
            List<ParticipationRequest> listeoffre = new List<ParticipationRequest>();
            ParticipationRequest pr;
            Marché mar = dbmetier.Marché.Where(a => a.id_marché == id_marche).FirstOrDefault();
            List<Participation> listparticipation = dbmetier.Participations.Where(a => a.id_marché_participation == mar.id_marché&& a.budget_prestation!=null).ToList();
            foreach (Participation item in listparticipation)

            {
                List<Profil_cout_chargeVM> listPCVM = new List<Profil_cout_chargeVM>();
                Profil_cout_chargeVM element ;
                prestataire prestataire = dbmetier.prestataires.Where(a => a.id_prestataire == item.id_prestataire_participation).FirstOrDefault();
                List<Participation_profil> listepp = dbmetier.Participation_profil.Where(a => a.id_participation_fk == item.id_participation).ToList();
                foreach (var item2 in listepp)
                {
                    element = new Profil_cout_chargeVM();
                    element.cout = (int)item2.cout_unit_ht;
                    element.libelle = item2.Profil.libelle_profil;
                    listPCVM.Add(element);
                }
               
                
                pr = new ParticipationRequest
                {
                    prestataire_participant = prestataire,
                    budget_prestation = (int)item.budget_prestation,
                    listeprofil_cout = listPCVM,
                    

                };
                listeoffre.Add(pr);
                
                
            }
            ViewBag.id_marche = id_marche;
            return View(listeoffre);
        }
        
        // GET: Marche/Create
        [Authorize(Roles = "responsable_marche")]
        [HttpGet]
        public ActionResult Create()
        {
            
            ViewBag.listeprestataire = dbmetier.prestataires.ToList();
            ViewBag.listeProjet = dbmetier.Projets.ToList();
            var listedomaines = dbmetier.Domaines.ToList();
            ViewBag.listedomaines = new SelectList(listedomaines, "libelle_domaine", "libelle_domaine");
            var listProfils = dbmetier.Profils.ToList();
            ViewBag.listeProfils = new SelectList(listProfils, "libelle_profil", "libelle_profil");

            return View(new MarcheViewModel());
        }
        
        
        // POST: Marche/Create
        [HttpPost]
        [Authorize(Roles = "responsable_marche")]
        public ActionResult Create(MarcheViewModel marche)
        {
            
                var userId = User.Identity.GetUserId();
                
                Projet pro = dbmetier.Projets.Find(marche.id_projet);
                Marché mar = new Marché() ;
                mar.Délai = marche.Délai;
                mar.date_fin = marche.date_fin;
                mar.date_debut = System.DateTime.Now;
                mar.budget_prevu = marche.budget_prevu;
                mar.id_projet_fk = marche.id_projet;
                mar.matricule = marche.matricule;
                
                mar.id_responsable_fk = userId;
                dbmetier.Marché.Add(mar);
                dbmetier.SaveChanges();
            foreach (var item in marche.listeprestataire)
                {
                Models.Participation par = new Models.Participation();

                    par.id_marché_participation = mar.id_marché;
                    par.id_prestataire_participation = item;
                    dbmetier.Participations.Add(par);
                    dbmetier.SaveChanges();

            }
                
                    foreach (MissionViewModel item in marche.listMission)
                   {
                     

                    //ajout de domaine 
                    Domaine domaine = dbmetier.Domaines.Where(x => x.libelle_domaine == item.domaine).FirstOrDefault();

                    Mission mision = new Mission
                    {
                        id_mission = item.id_mission,
                        libelle_mission = item.libelle_mission,
                        id_domaine_mission_fk=domaine.id_domaine,
                        id_marche_mission = mar.id_marché
                    };
                    dbmetier.Missions.Add(mision);
                    dbmetier.SaveChanges();
                    foreach (var profillib in item.listeprofils)
                    {
                        Profil prof = dbmetier.Profils.Where(x => x.libelle_profil == profillib.libelle).FirstOrDefault();
                        Affectation_profils afectation = new Affectation_profils
                        {
                            id_mission_affectation = mision.id_mission,
                            id_profils_affectation = prof.id_profil,
                            charge_profil=profillib.charge
                        };
                        dbmetier.Affectation_profils.Add(afectation);
                        dbmetier.SaveChanges();

                }
                     
            }
                
            return Content("<script language='javascript' type='text/javascript'>alert('sucsssflly!');</script>");

        }
        [Authorize(Roles = "prestataire")]
        public ActionResult MarcheHistorique()
        {
            var id_prestataire = User.Identity.GetUserId();
            IList<Marché> liste_marche = dbmetier.Marché.Where(a => a.id_prestataire_final == id_prestataire).ToList();
            List<Marché> listmarchehistroque = new List<Marché>();
            foreach (var item in liste_marche)
            {
                IList<Mission> list_mission = dbmetier.Missions.Where(a => a.id_marche_mission == item.id_marché && a.date_fin_mission == null).ToList();
                if (list_mission != null)
                  listmarchehistroque.Add(item);
            }
           
            return View(liste_marche);

        }

        [HttpGet]
        [Authorize(Roles = "responsable_marche")]
        public ActionResult MarcheHistoriqueRsp()
        {
            var userId = User.Identity.GetUserId();
            IList<Marché> listMarche = dbmetier.Marché.Where(a => a.id_responsable_fk == userId).ToList();
            IList<Marché> listMarche_histo = new List<Marché>();
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
                if (istrue == false)
                    listMarche_histo.Add(item);
            }
            return View(listMarche_histo);

        }

        [HttpGet]
        [Authorize(Roles = "prestataire")]
        public ActionResult Marcheencour()
        {   var id_prestataire = User.Identity.GetUserId();
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




            return View(listMarche_encour);

        }
        [HttpGet]
        [Authorize(Roles = "prestataire")]
        public ActionResult MarchePropose()
        {
            try
            {
                var id_prestataire = User.Identity.GetUserId();
                IList<Marché> liste_marche = dbmetier.Participations.Where(a => a.id_prestataire_participation == id_prestataire && a.budget_prestation==null).Select(a => a.Marché).Where(a=>a.id_prestataire_final==null).ToList();
                
                return View(liste_marche);

            }
            catch
            {
                return View();
            }
        }
        [Authorize(Roles = "prestataire")]
        public ActionResult MarcheENAttent()
        {
            var id_prestataire = User.Identity.GetUserId();
            return View(dbmetier.Participations.Where(a=>a.id_prestataire_participation==id_prestataire&&a.budget_prestation!=null).Select(a => a.Marché).Where(a=>a.id_prestataire_final==null).ToList());
        }
        [Authorize(Roles = "prestataire")]
        public ActionResult Refuser(int id_Marche)

        {
            try
            {
                var id_prestataire = User.Identity.GetUserId();
                var participation= dbmetier.Participations.Where(a => a.id_prestataire_participation == id_prestataire && a.id_marché_participation==id_Marche ).FirstOrDefault();
                dbmetier.Participations.Remove(participation);
                dbmetier.SaveChanges();
                return RedirectToAction("MarchePropose");
            }
            catch (Exception)
            {

                throw;
            }
        }



        [Authorize(Roles = "prestataire")]
        public ActionResult Participer(ParticipationRequest PF_element)
        {
            
            
                var id_prestataire = User.Identity.GetUserId();
                // dbmetier..Entry(marché).State = EntityState.Modified;
                Models.Participation participation = dbmetier.Participations.Where(a => a.id_prestataire_participation == id_prestataire && a.id_marché_participation == PF_element.id_marche).FirstOrDefault();
                participation.budget_prestation = PF_element.budget_prestation;

                Profil prof = new Profil();
                Participation_profil pf;
                foreach (Models.viewmodel.Profil_cout_chargeVM item in PF_element.listeprofil_cout)
                {
                   prof = dbmetier.Profils.Where(a => a.libelle_profil == item.libelle).FirstOrDefault();
                    pf = new Participation_profil
                    { id_participation_fk = participation.id_participation,
                      id_profil_fk= prof.id_profil,
                      cout_unit_ht= item.cout
                    };
                    dbmetier.Participation_profil.Add(pf);
                }
                

                dbmetier.SaveChanges();
                return RedirectToAction("MarchePropose");

            
           
        }

        // GET: Marche/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Marche/Edit/5
        [HttpPost]
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
        [HttpGet]
        [Authorize(Roles = "responsable_marche")]
        public ActionResult Delete(int id)
        {
            Suprimer_marche(id);
           return  RedirectToAction("MarcheencourRsp");
        }
        public void Suprimer_marche(int id)
        {
            Marché marche = dbmetier.Marché.Find(id);
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
        // POST: Marche/Delete/5
        [HttpPost]
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
