using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aplication20_07_2019.Controllers
{
    public class MainAdminController : Controller
    {
        // GET: MainAdmin
        public ActionResult Index()
        {
            return View();
        }

        // GET: MainAdmin/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MainAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MainAdmin/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MainAdmin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MainAdmin/Edit/5
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

        // GET: MainAdmin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MainAdmin/Delete/5
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
