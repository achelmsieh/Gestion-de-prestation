using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aplication20_07_2019.Models.viewmodel
{
    public class MissionViewModel
    {
        public int id_mission { get; set; }
        public Nullable<System.DateTime> date_debut_mission { get; set; }
        public Nullable<System.DateTime> date_fin_mission { get; set; }
        public string libelle_mission { get; set; }
        public Nullable<double> charge_jh { get; set; }
        public Nullable<double> cout_ht { get; set; }
        public int id_marche_mission { get; set; }
        public Nullable<int> id_ordreservice_fk { get; set; }
        public Nullable<int> id_domaine_mission_fk { get; set; }
        public List<Profil_cout_chargeVM> listeprofils { get; set; }
        public string domaine { get; set; }
        public MissionViewModel()
        {
            this.listeprofils = new List<Profil_cout_chargeVM>();
           
        }
    }
}