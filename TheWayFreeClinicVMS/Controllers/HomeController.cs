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
            //get id from email
            var thisVolID = (from i in db.Volunteers  where i.volEmail == email select i.volID).SingleOrDefault();

            //query will return a Worktime object if startTime and Endtime same. i.e., user made new clock-in. 
            var query = (from w in db.Worklog where w.volID == thisVolID && w.wrkEndTime == w.wrkStartTime select w).SingleOrDefault();
            
            //if not null, user is still clocked in. Update wrkEndTime with timestamp. 
            //Now user has no worktime record with same start/end time. At next entry query will return null and move to else.
            if (query !=null )
            {                
                query.wrkEndTime = DateTime.UtcNow;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            else
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        worktime.volID = thisVolID;
                        worktime.wrkID = (int)DateTime.Now.Ticks;
                        worktime.wrkDate = DateTime.Today;
                        worktime.wrkStartTime = DateTime.UtcNow; 
                        worktime.wrkEndTime = DateTime.UtcNow; //same as startTime, signifying clocked in.

                        db.Worklog.Add(worktime);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "We cannot find your account. Try again, and if the problem persists see your system administrator.");
                }
            }           

            return View();
        }

        private string alert(string v)
        {
            throw new NotImplementedException();
        }

        public ActionResult Modal()
        {
            return PartialView("_CheckInOutConfirmation");
        }

        [HttpPost]
        public ActionResult CheckInOutConfirmation()
        {
            return RedirectToAction("Index");
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