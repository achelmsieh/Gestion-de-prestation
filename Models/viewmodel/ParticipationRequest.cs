using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aplication20_07_2019.Models.viewmodel
{
    public class ParticipationRequest
    {   
        public int id_marche { get; set; }
        
        public int budget_prestation { get; set; }
        public List<Profil_cout_chargeVM> listeprofil_cout { get; set; }
        public prestataire prestataire_participant { get; set; }
        public ParticipationRequest()
        {
            this.listeprofil_cout = new List<Profil_cout_chargeVM>();

        }

    }
}