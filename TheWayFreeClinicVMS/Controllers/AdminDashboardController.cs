using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TheWayFreeClinicVMS.Models;
using Microsoft.SqlServer;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace TheWayFreeClinicVMS.Controllers
{
    [Authorize(Roles = "Admin")]
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
            int year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year, 1, 1);
            var newVolunteers = (from v in volunteers
                                 where (v.volStartDate >= firstDay)
                                 select v.volID).Count();
            ViewBag.newVolunteers = newVolunteers;
            ViewBag.year = year;
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
                    return RedirectToAction("RegisterNewVol", "Account", volunteer); // new redirect; to alternate register method in account controller passing vol from form
                    //return RedirectToAction("Details", "AdminDashboard", new { id = volunteer.volID });  old redirect
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

            if (Request.Browser.Browser == "InternetExplorer")
            {
                ViewBag.date = worklog.wrkDate.ToShortDateString();
            }
            else
            {
                ViewBag.date = worklog.wrkDate.ToString("yyyy-MM-dd");
            }

            
            
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

                List<string> flags = new List<string>();

                using (System.IO.StreamReader file = new System.IO.StreamReader(Server.MapPath("~/Content/docs/WorklogDataMessages/") + ("WorklogDataFlags.txt"), false))
                {
                    try
                    {
                        var txt = "";

                        while ((txt = file.ReadLine()) != null)
                        {
                            var msgs = txt.Split(';');
                            foreach (var item in msgs)
                            {
                                if (item != "")
                                {
                                    var id_code = item.Split(',');

                                    if (id_code[0] != worklog.wrkID.ToString())
                                    {
                                        flags.Add(item);
                                    }
                                }
                            }
                        }
                        file.Close();      
                    }
                    catch (Exception e)
                    {

                    }
                }
                using (System.IO.StreamWriter newFile = new System.IO.StreamWriter(Server.MapPath("~/Content/docs/WorklogDataMessages/") + ("WorklogDataFlags.txt"), false))
                {
                    try
                    {
                        foreach (var item in flags)
                        {
                            if (item != "")
                            {
                                newFile.Write(item + ";");
                            }                            
                        }
                        newFile.Close();
                    }
                    catch (Exception e)
                    {

                    }
                }

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
            Session["HRFL"] = HoursReportFilteredList;

            return View(HoursReportFilteredList);
        }

        
        public ActionResult ExportHoursReportToCSV()
        {
            List<HoursReportVol> Data =  (List<HoursReportVol>)Session["HRFL"];
            StringWriter sw = new StringWriter();

            sw.WriteLine("\"Last Name\",\"First Name\",\"Specialty\",\"Active\",\"E-mail\",\"Hours\"");
            string hdr = "attachment;filename=" + DateTime.Now.ToShortDateString() + ".csv";
            Response.ClearContent();
            Response.AddHeader("content-disposition", hdr);
            Response.ContentType = "text/csv";

            foreach (var line in Data)
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",
                                           line.volunteer.volLastName,
                                           line.volunteer.volFirstName,
                                           line.volunteer.Specialty.spcName,
                                           line.volunteer.volActive,
                                           line.volunteer.volEmail,
                                           line.hours));
            }

            Response.Write(sw.ToString());

            Response.End();

            return View("Report", Data);
        }
        
        public ActionResult AvailabilityReport(string searchString, /*string hiddenDateRange,*/ int? specialtySearch, string active, bool all = false, bool mon = false, bool tue = false, bool wed = false, bool thu = false, bool fri = false, bool sat = false)
        {
            ViewBag.viewName = "index";
            ViewBag.FullName = getUserName();

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
            if (mon) { daysQuery.Add("Monday"); ViewBag.MonChkd = "checked"; ViewBag.MonActv = "active"; ViewBag.Mon = true; }
            if (tue) { daysQuery.Add("Tuesday"); ViewBag.TueChkd = "checked"; ViewBag.TueActv = "active"; ViewBag.Tue = true; }
            if (wed) { daysQuery.Add("Wednesday"); ViewBag.WedChkd = "checked"; ViewBag.WedActv = "active"; ViewBag.Wed = true; }
            if (thu) { daysQuery.Add("Thursday"); ViewBag.ThuChkd = "checked"; ViewBag.ThuActv = "active"; ViewBag.Thu = true; }
            if (fri) { daysQuery.Add("Friday"); ViewBag.FriChkd = "checked"; ViewBag.FriActv = "active"; ViewBag.Fri = true; }
            if (sat) { daysQuery.Add("Saturday"); ViewBag.SatChkd = "checked"; ViewBag.SatActv = "active"; ViewBag.Sat = true; }

            var specialties = db.Specialties.OrderBy(q => q.spcName).ToList();
            ViewBag.specialtySearch = new SelectList(specialties, "spcID", "spcName", specialtySearch);
            int specialtyID = specialtySearch.GetValueOrDefault();

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

            Session["ARFL"] = AvailabilityReportFilteredList;
               
            return View(AvailabilityReportFilteredList.Distinct());
        }
        
        public ActionResult ExportAvailabilityReportToCSV()
        {
            List<AvailabilityReportVol> Data = (List<AvailabilityReportVol>)Session["ARFL"];

            StringWriter sw = new StringWriter();

            sw.WriteLine("\"Last Name\",\"First Name\",\"Specialty\",\"Active\",\"Days\",\"Hours\"");
            string hdr = "attachment;filename=" + "Availability Report" + DateTime.Now.ToShortDateString() + ".csv";
            Response.ClearContent();
            Response.AddHeader("content-disposition", hdr);
            Response.ContentType = "text/csv";

            foreach (var line in Data.Distinct())
            {
                var daysQueriedString = "";
                var daysString = "";
                var hoursString = "";
                var allDaysString = "";

                foreach (var day in line.daysQueried)
                {                    
                    daysQueriedString += day + "\n";
                }
                foreach (var day in line.days)
                {
                    daysString += day + "\n";
                }
                foreach(var hours in line.hours)
                {
                    hoursString += hours + "\n";
                }

                if (daysString != null)
                {
                    allDaysString = daysQueriedString + daysString;
                }
                else
                {
                    allDaysString = daysQueriedString;
                }
                

                allDaysString = allDaysString.TrimEnd('\n');
                hoursString = hoursString.TrimEnd('\n');

                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",
                                           line.volunteer.volLastName,
                                           line.volunteer.volFirstName,
                                           line.volunteer.Specialty.spcName,
                                           line.volunteer.volActive,
                                           allDaysString,
                                           hoursString));
            }

            Response.Write(sw.ToString());

            Response.End();

            return View("AvailabilityReport", Data);
        }
        
        public ActionResult WorklogData(string sortOrder, string searchString, int? specialtySearch)
        {
            ViewBag.FullName = getUserName();

            var volunteers = db.Volunteers.Include(v => v.Specialty);
            var wlog = db.Worklog;
            var wlogSorts = (from w in wlog
                             join v in volunteers on w.volID equals v.volID
                             select w).OrderBy(w => w.wrkDate);
            

            var specialties = db.Specialties.OrderBy(q => q.spcName).ToList();
            ViewBag.specialtySearch = new SelectList(specialties, "spcID", "spcName", specialtySearch);
            int specialtyID = specialtySearch.GetValueOrDefault();

            //sorting by last name and the starting date
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "name_asc";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            //filtering by first name, last name 
            if (!String.IsNullOrEmpty(searchString))
            {
                wlogSorts = wlogSorts.Where(s => s.Volunteer.volLastName.Contains(searchString)
                                       || s.Volunteer.volFirstName.Contains(searchString)).OrderBy(w => w.wrkDate);
            }

            if (specialtySearch.HasValue)
            {
                wlogSorts = wlogSorts.Where(s => s.Volunteer.spcID == specialtyID).OrderBy(w => w.wrkDate);
            }

            switch (sortOrder)
            {
                case "name_desc":
                    wlogSorts = wlogSorts.OrderByDescending(w => w.Volunteer.volLastName);
                    break;
                case "name_asc":
                    wlogSorts = wlogSorts.OrderBy(w => w.Volunteer.volLastName);
                    break;
                case "Date":
                    wlogSorts = wlogSorts.OrderBy(w => w.wrkDate);
                    break;
                case "date_desc":
                    wlogSorts = wlogSorts.OrderByDescending(w => w.wrkDate);
                    break;
                default:
                    wlogSorts = wlogSorts.OrderByDescending(w => w.wrkDate);
                    break;
            }

            using (StreamReader file = new StreamReader(Server.MapPath("~/Content/docs/WorklogDataMessages/") + ("WorklogDataFlags.txt"), false))
            {
                string txt;
                List<string> msgsNum = new List<string>();
                List<string> msgsCode = new List<string>();

                while ((txt = file.ReadLine()) != null)
                {
                    var msgs = txt.Split(';');
                    foreach (string item in msgs)
                    {
                        if (item != "")
                        {
                            item.TrimEnd(';');
                            var items = item.Split(',');
                            msgsNum.Add(items[0]);
                            msgsCode.Add(items[1]);
                        }

                    }

                    ViewBag.msgNum = msgsNum;
                    ViewBag.msgCode = msgsCode;
                }

                file.Close();
            }

            
            return View(wlogSorts.ToList());
        }

        public ActionResult BackupDB()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BackupDbToFile()
        {            
                SqlConnection con = new SqlConnection();
                SqlCommand sqlcmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();

                //con.ConnectionString = @"Server=MyPC\SqlServer2k8;database=Test;Integrated Security=true;";
                con.ConnectionString = @"Data Source=99.127.65.108,1433;Initial Catalog=rebelscrumdb;Persist Security Info=True;User ID=rebelNadiia;Password=thewayfreeclinic;";
                string backupDIR = Server.MapPath("~/Content/docs/Backups/");
                string fileName = DateTime.Now.ToString("MM-dd-yyyy_HHmmss");

                if (!System.IO.Directory.Exists(backupDIR))
                {
                    System.IO.Directory.CreateDirectory(backupDIR);
                }
            try
            {
                con.Open();
                sqlcmd = new SqlCommand("backup database rebelscrumdb to disk='" + backupDIR + "\\" + fileName + ".Bak'", con);
                sqlcmd.ExecuteNonQuery();
                con.Close();

                if (Server.MapPath("~/Content/docs/Backups/" + fileName + ".Bak") != null)
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(backupDIR + fileName + ".Bak");
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName + ".Bak");
                }
            }
            catch (Exception ex)
            {
                ViewBag.BackupMsg = "Error Occured During DB backup process !<br>" + ex.ToString();
            }

            return RedirectToAction("BackupDB");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void BackupDBToExcel(string tableName)
        {
            string dateCreated = DateTime.Now.ToShortDateString();
            string fileName = tableName + "_" + dateCreated;

            GridView gv = new GridView();

            switch (tableName)
            {
                case "Availability":
                    {
                        gv.DataSource = db.Availabilities.ToList();
                        break;
                    }
                case "Contract":
                    {
                        gv.DataSource = db.Contracts.ToList();
                        break;
                    }
                case "Econtact":
                    {
                        gv.DataSource = db.Econtacts.ToList();
                        break;
                    }
                case "Employer":
                    {
                        gv.DataSource = db.Employers.ToList();
                        break;
                    }
                case "Job":
                    {
                        gv.DataSource = db.Jobs.ToList();
                        break;
                    }
                case "Language":
                    {
                        gv.DataSource = db.Languages.ToList();
                        break;
                    }
                case "License":
                    {
                        gv.DataSource = db.Licenses.ToList();
                        break;
                    }
                case "Pagroup":
                    {
                        gv.DataSource = db.Pagroups.ToList();
                        break;
                    }
                case "Speak":
                    {
                        gv.DataSource = db.Speaks.ToList();
                        break;
                    }
                case "Specialty":
                    {
                        gv.DataSource = db.Specialties.ToList();
                        break;
                    }
                case "Volunteer":
                    {
                        gv.DataSource = db.Volunteers.ToList();
                        break;
                    }
                case "Worktime":
                    {
                        gv.DataSource = db.Worklog.ToList();
                        break;
                    }
            }            
            
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
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
    }
}
