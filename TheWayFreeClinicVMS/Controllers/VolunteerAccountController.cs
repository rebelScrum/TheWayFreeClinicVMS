using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheWayFreeClinicVMS.Controllers
{
    public class VolunteerAccountController : Controller
    {
        // GET: VolunteerAccount
        public ActionResult Index()
        {
            return View();
        }

        // GET: VolunteerAccount/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: VolunteerAccount/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VolunteerAccount/Create
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

        // GET: VolunteerAccount/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: VolunteerAccount/Edit/5
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

        // GET: VolunteerAccount/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VolunteerAccount/Delete/5
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
