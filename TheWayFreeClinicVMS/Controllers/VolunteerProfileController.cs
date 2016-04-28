using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TheWayFreeClinicVMS.Models;

namespace TheWayFreeClinicVMS.Controllers
{
    public class VolunteerProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: VolunteerProfile
        public ActionResult Index()
        {
            ViewBag.FullName = getUserName();
            var vol = db.Volunteers;
            
            var user = (from i in vol where i.volEmail == User.Identity.Name select i.volID).SingleOrDefault();
            
            Volunteer volunteer = db.Volunteers.Find(user);
            if (volunteer == null || User.Identity.Name != volunteer.volEmail)
            {
                return HttpNotFound();
            }
            return View(volunteer);
            
        }

        // GET: VolunteerProfile/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Volunteer volunteer = db.Volunteers.Find(id);
            if (volunteer == null || User.Identity.Name != volunteer.volEmail)
            {
                return HttpNotFound();
            }
            ViewBag.spcID = new SelectList(db.Specialties, "spcID", "spcName", volunteer.spcID);
            return View(volunteer);
        }

        // POST: VolunteerProfile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "volID,volFirstName,volLastName,middleName,volDOB,volEmail,volPhone,volStreet1,volStreet2,volCity,volState,volZip,volStartDate,volActive,spcID")] Volunteer volunteer)
        {
            ViewBag.FullName = getUserName();
            if (ModelState.IsValid)
            {
                db.Entry(volunteer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.spcID = new SelectList(db.Specialties, "spcID", "spcName", volunteer.spcID);
            return View(volunteer);
        }


        //**************************************************************************************
        //Get Availability
        public ActionResult VolunteerAvailable()
        {
            var vol = db.Volunteers;
            var user = (from i in vol where i.volEmail == User.Identity.Name select i.volID).SingleOrDefault();
            Volunteer volunteer = db.Volunteers.Find(user);
            if (volunteer == null || User.Identity.Name != volunteer.volEmail)
            {
                return HttpNotFound();
            }
            var volunteerID = user;
            var schedule = db.Availabilities.Where(s => s.volID == volunteerID).ToList();
            return PartialView("_VolunteerAvailable", schedule);
        }
        //**************************************************************************************
        //Get License
        public ActionResult VolunteerLicense()
        {
            var vol = db.Volunteers;
            var user = (from i in vol where i.volEmail == User.Identity.Name select i.volID).SingleOrDefault();
            var volunteerID = user;
            var license = db.Licenses.Where(s => s.volID == volunteerID).ToList();
            return PartialView("_VolunteerLicense", license);
        }
        //*************************************************************************************
        //Get Contract
        public ActionResult VolunteerContract()
        {
            var vol = db.Volunteers;
            var user = (from i in vol where i.volEmail == User.Identity.Name select i.volID).SingleOrDefault();
            var volunteerID = user;
            var contract = db.Contracts.Where(s => s.volID == volunteerID).ToList();
            var groups = db.Pagroups.OrderBy(o => o.pgrName).ToList();
            ViewBag.grpList = new SelectList(groups, "pgrID", "pgrName");
            return PartialView("_VolunteerContract", contract);
        }
        //***********************************************************************************
        //Get Timesheet
        public ActionResult VolunteerTimesheet()
        {
            var vol = db.Volunteers;
            var user = (from i in vol where i.volEmail == User.Identity.Name select i.volID).SingleOrDefault();
            var volunteerID = user;
            var timesheet = db.Worklog.Where(s => s.volID == volunteerID).ToList().OrderByDescending(d => d.wrkDate);

            var wlog = db.Worklog.Include(v => v.Volunteer);
            var volunteers = db.Volunteers;

            //will set Worktime object if Endtime null. i.e., user clocked-in. 
            Worktime time = (from w in wlog where w.volID == volunteerID && w.wrkEndTime == null select w).SingleOrDefault();
            if (time != null) //time variable holds a record with wrkEndTime==null, user is still clocked in
            {
                ViewBag.ClockStatus = "Clocked In";
                ViewBag.ClockActionBtn = "Click To Clock Out";
            }
            else //user has no record containing null wrkEndTime, user currently clocked out
            {
                ViewBag.ClockStatus = "Clocked Out";
                ViewBag.ClockActionBtn = "Click To Clock In";
            }

            return PartialView("_VolunteerTimesheet", timesheet);
        }

        //***********************************************************************************
        //Update Timesheet
        public ActionResult UpdateTimesheet()
        {
            var vol = db.Volunteers;
            var user = (from i in vol where i.volEmail == User.Identity.Name select i.volID).SingleOrDefault();
            ViewBag.FullName = getUserName();
            
            Worktime worklog = db.Worklog.Find(user);


            if (worklog == null)
            {
                return HttpNotFound();
            }
            ViewBag.volID = new SelectList(db.Volunteers, "volID", "volFirstName", worklog.volID);

            return View(worklog);
        }
        // POST: ManageTimesheet/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateTimesheet([Bind(Include = "wrkID,volID,wrkDate,wrkStartTime,wrkEndTime")] Worktime worklog)
        {

            if (ModelState.IsValid)
            {
                db.Entry(worklog).State = EntityState.Modified;

                //db.Worklog.Add(worklog);
                db.SaveChanges();

                return RedirectToAction("Index", new { id = worklog.volID });
            }

            return View(worklog);
        }
        //************************************************************************************
        //Get Employment

        public ActionResult VolunteerEmployer()
        {
            var vol = db.Volunteers;
            var user = (from i in vol where i.volEmail == User.Identity.Name select i.volID).SingleOrDefault();
            var volunteerID = user;
            var jobs = db.Jobs.Where(e => e.volID == volunteerID).ToList();
            var employer = db.Employers.OrderBy(o => o.empName).ToList();
            ViewBag.empList = new SelectList(employer, "empID", "empName");
            return PartialView("_VolunteerEmployer", jobs);
        }
        //***********************************************************************************
        //Get Econtact
        public ActionResult VolunteerEcontact()
        {
            var vol = db.Volunteers;
            var user = (from i in vol where i.volEmail == User.Identity.Name select i.volID).SingleOrDefault();
            var volunteerID = user;

            var emergency = db.Econtacts.Where(e => e.volID == volunteerID).ToList();
            return PartialView("_VolunteerEcontact", emergency);
        }

        //************************************************************************************
        //Get Languages
        [HttpGet]
        public ActionResult VolunteerLanguages()
        {
            var vol = db.Volunteers;
            var user = (from i in vol where i.volEmail == User.Identity.Name select i.volID).SingleOrDefault();
            var volunteerID = user;
            var speaks = db.Speaks.Where(sp => sp.volID == volunteerID).ToList();
            //var volLang = string.Join(", ", speaks);  

            var lng = db.Languages.OrderBy(q => q.lngName).ToList();
            ViewBag.langSearch = new SelectList(lng, "lngID", "lngName");

            return PartialView("_VolunteerLanguages", speaks);
        }

        //Add Languages
        [HttpPost]
        public ActionResult VolunteerLanguages([Bind(Include = "speakID, lngID, volID")] int? id, int? langSearch)
        {
            var thisID = id;
            Speak spks = new Speak();
            bool alreadySpeaks = false;

            try
            {
                if (ModelState.IsValid)
                {
                    var lng = db.Languages.OrderBy(q => q.lngName).ToList();
                    ViewBag.langSearch = new SelectList(lng, "lngID", "lngName", langSearch);
                    int lngID = langSearch.GetValueOrDefault();

                    spks.lngID = lngID;
                    spks.volID = id.GetValueOrDefault();

                    alreadySpeaks = db.Speaks.Any(u => u.lngID == lngID && u.volID == id);

                    if (!alreadySpeaks)
                    {
                        db.Speaks.Add(spks);
                        db.SaveChanges();
                    }
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.Try again, and if the problem persists see your system administrator.");
            }
            var speaks = db.Speaks.Where(sp => sp.volID == id).ToList();

            return PartialView("_VolunteerLanguages", speaks);
        }


        public string getUserName()
        {
            var vols = db.Volunteers;
            string fullName = (from v in vols
                               where v.volEmail == User.Identity.Name
                               select v.volLastName + ", " + v.volFirstName).FirstOrDefault();


            return fullName;
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