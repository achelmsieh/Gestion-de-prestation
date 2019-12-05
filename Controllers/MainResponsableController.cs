using Microsoft.AspNet.Identity;
using System;
using aplication20_07_2019.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aplication20_07_2019.Controllers
{
    public class MainResponsableController : Controller
    {
        market_appEntities dbmetier = new market_appEntities();
        // GET: MainResponsable
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Information_personnel()
        {
            Responsable_marché responsable =  dbmetier.Responsable_marché.Find( User.Identity.GetUserId());
            return View(responsable);
        }

    }
}