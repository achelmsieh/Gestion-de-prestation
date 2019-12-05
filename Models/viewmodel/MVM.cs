using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aplication20_07_2019.Models.viewmodel
{
    public class MVM
    {
        public Mission mission { get; set; }
       
        public List<Profil_cout_chargeVM> listeprofils { get; set; }
        public MVM()
        {
            this.listeprofils = new List<Profil_cout_chargeVM>();

        }
        public int cout { get; set; }
    }
}