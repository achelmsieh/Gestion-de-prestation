using aplication20_07_2019.Models.viewmodel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace aplication20_07_2019.Models
{
    public class MarcheViewModel
    {
        [Required(ErrorMessage ="Entrer la matricule")]
        public string matricule { get; set; }
        [Required(ErrorMessage ="Entrer le budget")]
        public int budget_prevu { get; set; }
        [Required(ErrorMessage = "Entrer la date de fin")]
        public Nullable<System.DateTime> date_fin { get; set; }
        public Nullable<int> montant_annuel { get; set; }
        public Nullable<int> montant_total { get; set; }
        public string id_responsable_fk { get; set; }
        public string id_prestataire_final { get; set; }
        [Required(ErrorMessage = "selection un projet")]
        public Nullable<int> id_projet { get; set; }
        [Required(ErrorMessage = "Entrer le délai")]
        public Nullable<int> Délai { get; set; }
        public Nullable<System.DateTime> date_debut { get; set; }
        public int id_marché { get; set; }
        [Required(ErrorMessage = "Selectionner au moins un prestataire")]
        public ICollection<string> listeprestataire { get; set; }
        [Required(ErrorMessage = "ajoute au moins une mission")]
        public List<MissionViewModel> listMission { get; set; }
        


    }
}