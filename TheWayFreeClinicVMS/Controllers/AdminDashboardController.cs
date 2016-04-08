using Microsoft.AspNet.Identity;
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

        private ApplicationDbContext db = new ApplicationDbContext();

        public class HoursReportVol
        {
            public int id { get; set; }
            public double hours { get; set; }
            public Volunteer volunteer { get; set; }
        }

        public class AvailabilityReportVol
        {
            public int id { get; set; }
            public List<string> days { get; set; }
            public List<string> daysQueried { get; set; }   
            public List<string> hours { get; set; }
            public Volunteer volunteer { get; set; }
        }
       

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
            Volunteer volunteer = new Volunteer();
            //sets every newly created volunteer to active as default
            volunteer.volActive = true;
            return View(volunteer);
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
        //**************************************************************************************
        //Get License
        public ActionResult VolunteerLicense(int? id)
        {
            var volunteerID = id;
            var license = db.Licenses.Where(s => s.volID == volunteerID).ToList();
            return PartialView("_VolunteerLicense", license);
        }
        //*************************************************************************************
        //Get Contract
        public ActionResult VolunteerContract(int? id)
        {
            var volunteerID = id;
            var contract = db.Contracts.Where(s => s.volID == volunteerID).ToList();
            var groups = db.Pagroups.OrderBy(o => o.pgrName).ToList();
            ViewBag.grpList = new SelectList(groups, "pgrID", "pgrName");
            return PartialView("_VolunteerContract", contract);
        }
        //***********************************************************************************
        //Get Timesheet
        public ActionResult VolunteerTimesheet(int? id)
        {
            var volunteerID = id;
            var timesheet = db.Worklog.Where(s => s.volID == volunteerID).ToList();

            return PartialView("_VolunteerTimesheet", timesheet);
        }

        //***********************************************************************************
        //Update Timesheet
        public ActionResult UpdateTimesheet(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Worktime worklog = db.Worklog.Find(id);
            

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
        public ActionResult UpdateTimesheet([Bind(Include = "wrkID,volID,wrkDay,wrkStartTime,wrkEndTime")] Worktime worklog)
        {

            if (ModelState.IsValid)
            {
                db.Entry(worklog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = worklog.volID });
            }

            return View(worklog);
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

        // View list of Admins
        public ActionResult ManageAdmins()
        {
            string roleID = db.Roles.Where(r => r.Name == "Admin").Select(s => s.Id).FirstOrDefault();
            var admins = db.Users.Where(x => x.Roles.Any(y => y.RoleId == roleID)).ToList();
            return View(admins);
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

                
        public ActionResult Report(string searchString, string hiddenDateRange, int? specialtySearch, string active)

        {
            ViewBag.viewName = "index";
            ViewBag.FullName = getUserName();
            ViewBag.dateRange = hiddenDateRange;
            ViewBag.start = hiddenDateRange;
            ViewBag.allSelected = "";
            ViewBag.trueSelected = "";
            ViewBag.falseSelected = "";

            string[] tokens = new string[] {" - "};

            string[] dateRange;
            long begDateTicks = 0000000000;
            long endDateTicks = 0000000000;      
            long tempTotal = 0000000000;
            var volHours = 0.0;
            var grandTotalHours = 0.0;

            List<HoursReportVol> HoursReportFullList = new List<HoursReportVol> { };
            List<HoursReportVol> HoursReportFilteredList = new List<HoursReportVol> { };
            HoursReportVol vol = new HoursReportVol();

            var volunteers = db.Volunteers;
            var times = db.Worklog;

            var sorts = from v in volunteers
                        select v;

            var specialties = db.Specialties.OrderBy(q => q.spcName).ToList();
            ViewBag.specialtySearch = new SelectList(specialties, "spcID", "spcName", specialtySearch);
            int specialtyID = specialtySearch.GetValueOrDefault();

            //parse date range, convert to ticks
            if (hiddenDateRange != null && hiddenDateRange != "")
            {
                dateRange = hiddenDateRange.Split(tokens, StringSplitOptions.None);
                ViewBag.startDate = dateRange[0];
                ViewBag.endDate = dateRange[1];
                begDateTicks = Convert.ToDateTime(dateRange[0]).Ticks;
                endDateTicks = Convert.ToDateTime(dateRange[1]).AddHours(23).AddMinutes(59).AddSeconds(59).Ticks; // up to last second of selected day
            }
            else if (hiddenDateRange == "")
            {                
                return View(HoursReportFilteredList);
            }

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

            if (active != null)
            {
                switch (active)
                {
                    case "0":
                        ViewBag.falseSelected = "selected";
                        sorts = sorts.Where(s => s.volActive == false);
                        break;
                    case "1":
                        ViewBag.trueSelected = "selected";
                        sorts = sorts.Where(s => s.volActive == true);
                        break;
                    case "2":
                        ViewBag.allSelected = "selected";
                        sorts = sorts.Where(s => s.volActive == true || s.volActive == false);
                        break;
                }                
            }

            //create new object for each vol in sorts including properties for hours and exposing volID; adds to list of new objects
            foreach (var item in sorts)
            {
                vol = new HoursReportVol();
                vol.id = item.volID;
                vol.volunteer = item;
                HoursReportFullList.Add(vol);
            }           

            foreach (var item in HoursReportFullList)
            { 
                foreach (var log in times.ToList())
                {
                    if (log.volID == item.id && log.wrkEndTime.HasValue)
                    {
                        var beg = log.wrkStartTime.Ticks;
                        var end = log.wrkEndTime.Value.Ticks;                          

                        if (beg >= begDateTicks && end <= endDateTicks  )
                        {
                            tempTotal += (end - beg);                            
                        }
                    }
                }

                volHours = TimeSpan.FromTicks(tempTotal).TotalHours;
                grandTotalHours += volHours;                

                if (volHours == 0)
                {
                    item.hours = 0;
                }
                else
                {
                    item.hours = Math.Round(volHours, 3);
                    HoursReportFilteredList.Add(item);
                }
                tempTotal = 000000000;
            }

            ViewBag.grandTotalHours = Math.Round(grandTotalHours, 3);

            return View(HoursReportFilteredList);
        }

        public ActionResult AvailabilityReport(string searchString, /*string hiddenDateRange,*/ int? specialtySearch, string active, bool all = false, bool mon = false, bool tue = false, bool wed = false, bool thu = false, bool fri = false, bool sat = false)
        {
            ViewBag.viewName = "index";
            ViewBag.FullName = getUserName();
            //ViewBag.dateRange = hiddenDateRange;

            //string[] tokens = new string[] { " - " };
            //string[] dateRange;
            //long begDateTicks = 0000000000;
            //long endDateTicks = 0000000000;

            List<AvailabilityReportVol> AvailabilityReportFullList = new List<AvailabilityReportVol> { };
            List<AvailabilityReportVol> AvailabilityReportFilteredList = new List<AvailabilityReportVol> { };
            AvailabilityReportVol vol = new AvailabilityReportVol();

            var volunteers = db.Volunteers.ToList();
            var availabilities = db.Availabilities.ToList();

            var sorts = (from v in volunteers
                        join a in availabilities on v.volID equals a.volID
                        where v.volID == a.volID 
                        select v).Distinct();

            List<string> daysQuery = new List<string>();
            if (all) { ViewBag.allChkd = "checked"; ViewBag.allActv = "active"; }
            if (mon) { daysQuery.Add("Monday"); ViewBag.MonChkd = "checked"; ViewBag.MonActv = "active"; }
            if (tue) { daysQuery.Add("Tuesday"); ViewBag.TueChkd = "checked"; ViewBag.TueActv = "active"; }
            if (wed) { daysQuery.Add("Wednesday"); ViewBag.WedChkd = "checked"; ViewBag.WedActv = "active"; }
            if (thu) { daysQuery.Add("Thursday"); ViewBag.ThuChkd = "checked"; ViewBag.ThuActv = "active"; }
            if (fri) { daysQuery.Add("Friday"); ViewBag.FriChkd = "checked"; ViewBag.FriActv = "active"; }
            if (sat) { daysQuery.Add("Saturday"); ViewBag.SatChkd = "checked"; ViewBag.SatActv = "active"; }

            var specialties = db.Specialties.OrderBy(q => q.spcName).ToList();
            ViewBag.specialtySearch = new SelectList(specialties, "spcID", "spcName", specialtySearch);
            int specialtyID = specialtySearch.GetValueOrDefault();

            //parse date range, convert to ticks
            //if (hiddenDateRange != null && hiddenDateRange != "")
            //{
            //    dateRange = hiddenDateRange.Split(tokens, StringSplitOptions.None);
            //    ViewBag.startDate = dateRange[0];
            //    ViewBag.endDate = dateRange[1];
            //    begDateTicks = Convert.ToDateTime(dateRange[0]).Ticks;
            //    endDateTicks = Convert.ToDateTime(dateRange[1]).AddHours(23).AddMinutes(59).AddSeconds(59).Ticks; // up to last second of selected day
            //}
            //else if (hiddenDateRange == "")
            //{
            //    return View("AvailabilityReport");
            //}

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

            if (active != null)
            {
                switch (active)
                {
                    case "0":
                        ViewBag.falseSelected = "selected";
                        sorts = sorts.Where(s => s.volActive == false);
                        break;
                    case "1":
                        ViewBag.trueSelected = "selected";
                        sorts = sorts.Where(s => s.volActive == true);
                        break;
                    case "2":
                        ViewBag.allSelected = "selected";
                        sorts = sorts.Where(s => s.volActive == true || s.volActive == false);
                        break;
                }
            }

            //create new object for each vol in sorts including properties for hours and exposing volID; adds to list of new objects
            foreach (var item in sorts)
            {
                List<string> days = new List<string>();
                List<string> daysQueried = new List<string>();
                List<string> hours = new List<string>();
                vol = new AvailabilityReportVol();
                
                vol.id = item.volID;
                vol.volunteer = item;

                foreach (var avail in availabilities)
                {
                    if (vol.id == avail.volID)
                    {
                        if (daysQuery.Contains(avail.avDay.ToString()))
                        {
                            daysQueried.Add(avail.avDay.ToString());
                        }
                        else
                        {
                            days.Add(avail.avDay.ToString()); 
                        }
                                     
                        hours.Add(avail.avFrom.ToString("hh:mm tt") + " - " + avail.avUntil.ToString("hh:mm tt"));
                    }
                }
                vol.days = days;
                vol.daysQueried = daysQueried;
                vol.hours = hours;
                AvailabilityReportFullList.Add(vol);
            }

            foreach (var item in AvailabilityReportFullList)
            {
                foreach(var day in item.daysQueried)
                {
                    if (daysQuery.Contains(day))
                    {
                        AvailabilityReportFilteredList.Add(item);
                    }
                }
            }
               
            return View(AvailabilityReportFilteredList.Distinct());
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
