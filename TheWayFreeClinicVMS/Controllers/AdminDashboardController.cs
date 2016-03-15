using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TheWayFreeClinicVMS.Models;

namespace TheWayFreeClinicVMS.Controllers
{
    public class AdminDashboardController : Controller
    {
        public class HoursReportVol
        {
            public int id { get; set; }
            public double hours { get; set; }
            public Volunteer volunteer { get; set; }
        }
        private ApplicationDbContext db = new ApplicationDbContext();        

        // GET: AdminDashboard
        public ActionResult Index(string sortOrder, string searchString, int? specialtySearch)
        {
            ViewBag.FullName = getUserName();
            var volunteers = db.Volunteers.Include(v => v.Specialty);                      

            //selects volunteer list

            var sorts = from s in volunteers
                        select s;

            var specialties = db.Specialties.OrderBy(q => q.spcName).ToList();
            ViewBag.specialtySearch = new SelectList(specialties, "spcID", "spcName", specialtySearch);
            int specialtyID = specialtySearch.GetValueOrDefault();

            //filtering by first name, last name 
            if (!String.IsNullOrEmpty(searchString))
            {
                sorts = sorts.Where(s => s.volLastName.Contains(searchString)
                                       || s.volFirstName.Contains(searchString));
            }

            if (specialtySearch.HasValue) { 
            sorts = sorts.Where(s => s.spcID == specialtyID);
            }
            //sorting by last name and the starting date
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.ActiveSortParm = sortOrder == "Active" ? "Inactive" : "Active";
            ViewBag.viewName = "index";
           
           
            switch (sortOrder)
            {
                case "name_desc":
                    sorts = sorts.OrderByDescending(s => s.volLastName);
                    break;
                case "Active":
                    sorts = sorts.Where(s => s.volActive == true);
                    break;
                case "Inactive":
                    sorts = sorts.Where(s => s.volActive == false);
                    break;
                case "Date":
                    sorts = sorts.OrderBy(s => s.volStartDate);
                    break;
                case "date_desc":
                    sorts = sorts.OrderByDescending(s => s.volStartDate);
                    break;
                default:
                    sorts = sorts.OrderBy(s => s.volLastName);
                    break;
            }

           
            return View(sorts.ToList());
        }

        // GET: AdminDashboard/Details/5
        public ActionResult Details(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
                  
            Volunteer volunteer = db.Volunteers.Find(id);
            if (volunteer == null)
            {
                return HttpNotFound();
            }
            var schedule = db.Availabilities.Where(s => s.volID == volunteer.volID).ToList();
            return View(volunteer);
        }

        // GET: AdminDashboard/Create
        public ActionResult Create()
        {
            ViewBag.FullName = getUserName();
            ViewBag.spcID = new SelectList(db.Specialties, "spcID", "spcName");
            ViewBag.viewName = "create";
            return View();
        }

        // POST: AdminDashboard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "volID,volFirstName,volLastName,middleName,volDOB,volEmail,volPhone,volStreet1,volStreet2,volCity,volState,volZip,volStartDate,volActive,spcID")] Volunteer volunteer)
        {
            ViewBag.FullName = getUserName();
            ViewBag.spcID = new SelectList(db.Specialties, "spcID", "spcName", volunteer.spcID);
            try
            {
                var alreadyExists = db.Volunteers.Any(v => v.volEmail == volunteer.volEmail);
                if ((ModelState.IsValid) && !(alreadyExists))
                {
                    db.Volunteers.Add(volunteer);
                    db.SaveChanges();
                    
                    return RedirectToAction("Details", "AdminDashboard", new { id = volunteer.volID });
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.Try again, and if the problem persists see your system administrator.");
            }
            
            return View(volunteer);
        }

        // GET: AdminDashboard/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Volunteer volunteer = db.Volunteers.Find(id);
            if (volunteer == null)
            {
                return HttpNotFound();
            }
            ViewBag.volID = new SelectList(db.Econtacts, "volID", "ecFirstName", volunteer.volID);
            ViewBag.volID = new SelectList(db.Licenses, "volID", "volID", volunteer.volID);
            ViewBag.spcID = new SelectList(db.Specialties, "spcID", "spcName", volunteer.spcID);
            return View(volunteer);
        }

        // POST: AdminDashboard/Edit/5
      
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
        // GET: AdminDashboard/AddSpecialty
        public ActionResult AddSpecialty()
        {
            ViewBag.FullName = getUserName();
            return View("_AddSpecialty");
        }

        // POST: AdminDashboard/AddSpecialty     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSpecialty([Bind(Include = "spcID, spcName")] Specialty specialty)
        {
            ViewBag.FullName = getUserName();
            try
            {
                var alreadyExists = db.Specialties.Any(u => u.spcName == specialty.spcName);


                if ((ModelState.IsValid) && !(alreadyExists))
                {
                    db.Specialties.Add(specialty);
                    db.SaveChanges();
                    return RedirectToAction("Create");
                }

            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.Try again, and if the problem persists see your system administrator.");
            }

            return RedirectToAction("Create", "AdminDashboard");
        }

        //**************************************************************************************
        //Get Availability
        public ActionResult VolunteerAvailable(int? id)
        {
            var volunteerID = id;
            var schedule = db.Availabilities.Where(s => s.volID == volunteerID).ToList();
            return PartialView("_VolunteerAvailable", schedule);
        }
        //***********************************************************************************
        //Get Timesheet
        public ActionResult VolunteerTimesheet(int? id)
        {
            var volunteerID = id;
            var timesheet = db.Worklog.Where(s => s.volID == volunteerID).ToList();

            return PartialView("_VolunteerTimesheet", timesheet);
        }

        //************************************************************************************
        //Get Employment
        
        public ActionResult VolunteerEmployer(int? id)
        {
            var volunteerID = id;
            var jobs = db.Jobs.Where(e => e.volID == volunteerID).ToList();
            var employer = db.Employers.OrderBy(o => o.empName).ToList();
            ViewBag.empList = new SelectList(employer, "empID", "empName");
            return PartialView("_VolunteerEmployer", jobs);
        }
        //***********************************************************************************
        //Get Econtact
        public ActionResult VolunteerEcontact(int? id)
        {
            var volunteerID = id;
            
                var emergency = db.Econtacts.Where(e => e.volID == volunteerID).ToList();
                return PartialView("_VolunteerEcontact", emergency);
        }

        //************************************************************************************
        //Get Languages
        [HttpGet]
        public ActionResult VolunteerLanguages(int? id)
        {
            var volunteerID = id;
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

                    if (!alreadySpeaks) {
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


        // GET: AdminDashboard/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Volunteer volunteer = db.Volunteers.Find(id);
        //    if (volunteer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(volunteer);
        //}


        // POST: AdminDashboard/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public ActionResult DeleteConfirmed(int id)
        // {
        //    Volunteer volunteer = db.Volunteers.Find(id);
        //    db.Volunteers.Remove(volunteer);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //***********************************************************************************************
        public ActionResult Report(string sortOrder, string searchString, int? specialtySearch)
        {
            ViewBag.FullName = getUserName();

            var volunteers = db.Volunteers;
            var times = db.Worklog;

            List<HoursReportVol> HoursReportVolList = new List<HoursReportVol> { };
            HoursReportVol vol = new HoursReportVol();
            long tempTotal = 0000000000;
            var volHours = 0.0;

            var specialties = db.Specialties.OrderBy(q => q.spcName).ToList();
            ViewBag.specialtySearch = new SelectList(specialties, "spcID", "spcName", specialtySearch);
            int specialtyID = specialtySearch.GetValueOrDefault();

            var sorts = from v in volunteers
                       select v;

            //filtering by first name, last name 
            if (!String.IsNullOrEmpty(searchString))
            {
                sorts = sorts.Where(s => s.volLastName.Contains(searchString)
                                       || s.volFirstName.Contains(searchString));
            }

            if (specialtySearch.HasValue)
            {
                sorts = sorts.Where(s => s.spcID == specialtyID);
            }

            //create new object for each vol in sorts including properties for hours and exposing volID; adds to list of new objects
            foreach (var item in sorts)
            {
                vol = new HoursReportVol();
                vol.id = item.volID;
                vol.volunteer = item;
                HoursReportVolList.Add(vol);
            }
           

            foreach (var item in HoursReportVolList)
            { 
                foreach (var log in times.ToList())
                {
                    if (log.volID == item.id)
                    {
                        var end = log.wrkEndTime.Value.Ticks;
                        var beg = log.wrkStartTime.Ticks;
                        tempTotal += ( end - beg );
                    }
                }

                volHours = TimeSpan.FromTicks(tempTotal).TotalHours;
                                
                if (volHours == 0)
                {
                    item.hours = 0;
                }
                else
                {
                    item.hours = volHours;
                }
                tempTotal = 000000000;
            }



            //sorting by last name and the starting date
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.ActiveSortParm = sortOrder == "Active" ? "Inactive" : "Active";
            ViewBag.HoursSortParam = String.IsNullOrEmpty(sortOrder) ? "hours_desc" : "";
            ViewBag.viewName = "index";


            switch (sortOrder)
            {
                case "name_desc":
                    HoursReportVolList = HoursReportVolList.OrderByDescending(s => s.volunteer.volLastName).ToList();
                    break;
                case "Active":
                    HoursReportVolList = HoursReportVolList.Where(s => s.volunteer.volActive == true).ToList();
                    break;
                case "Inactive":
                    HoursReportVolList = HoursReportVolList.Where(s => s.volunteer.volActive == false).ToList();
                    break;
                case "hours_desc":
                    HoursReportVolList = HoursReportVolList.OrderByDescending(s => s.hours).ToList();
                    break;
            }

            return View(HoursReportVolList);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string getUserName()
        {
            var vols = db.Volunteers;
            string fullName = (from v in vols
                               where v.volEmail == User.Identity.Name
                               select v.volLastName + ", " + v.volFirstName).FirstOrDefault();
            return fullName;
        }

        //This may not be needed
        public ActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
