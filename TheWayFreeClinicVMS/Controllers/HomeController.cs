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
            var wlog = db.Worklog;

            var sorts = from s in wlog
                        select s;

            sorts = sorts.OrderByDescending(s => s.wrkDate);

            return View(sorts.ToList());
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
        public ActionResult Index([Bind(Include = "wrkID,volID,wrkDate,wrkStartTime,wrkEndTime")] string email)
        {
            ViewBag.confirm = "you are now...";

            var wlog = db.Worklog.Include(v => v.Volunteer);
            var volunteers = db.Volunteers;

            //get id from email
            var thisVolID = (from i in volunteers  where i.volEmail == email select i.volID).SingleOrDefault();

            //will set Worktime object if Endtime null. i.e., user clocked-in. 
            Worktime time = (from w in wlog where w.volID == thisVolID && w.wrkEndTime == null select w).SingleOrDefault();
            
            //if wrkEndTime null, user is still clocked in. Update wrkEndTime with timestamp. 
            //Now user has no worktime record with empty end time. At next entry query will return null and move to else.
            if (time != null)
            {                
                time.wrkDate = DateTime.Now;
                time.wrkEndTime = DateTime.Now;

                db.SaveChanges();
            }
            else //this user has no record containing null wrkEndTime
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        Worktime newTime = new Worktime();

                        newTime.volID = thisVolID;
                        newTime.wrkDate = DateTime.Now;
                        newTime.wrkStartTime = DateTime.Now;
                        newTime.wrkEndTime = null;//same as startTime, signifying clocked in.

                        db.Worklog.Add(newTime);
                        db.SaveChanges();                        
                    }
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "We cannot find your account. Try again. If the problem persists, contact your system administrator.");
                }
            }

            var sorts = from s in wlog
                        select s;

            sorts = sorts.OrderByDescending(s => s.wrkDate);

            return View(sorts.ToList());
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