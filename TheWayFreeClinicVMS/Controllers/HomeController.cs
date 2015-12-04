using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TheWayFreeClinicVMS.DataAccessLayer;
using TheWayFreeClinicVMS.Models;

namespace TheWayFreeClinicVMS.Controllers
{
    public class HomeController : Controller
    {
        private VMSContext db = new VMSContext();

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "wrkID,volID,wrkDate,wrkStartTime,wrkEndTime")] Worktime worktime, string email)
        {
            var thisVolID = (from i in db.Volunteers  where i.volEmail == email select i.volID).SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    worktime.volID = thisVolID;
                    worktime.wrkID = (int)DateTime.UtcNow.Ticks;
                    worktime.wrkDate = DateTime.Today;
                    worktime.wrkStartTime = DateTime.UtcNow;
                    worktime.wrkEndTime = DateTime.UtcNow;
                            
                    db.Worklog.Add(worktime);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.Try again, and if the problem persists see your system administrator.");
            }

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}