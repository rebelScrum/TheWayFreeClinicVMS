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
    public class AdminDashboardController : Controller
    {
        private VMSContext db = new VMSContext();

        // GET: AdminDashboard
        public ActionResult Index(string sortOrder, string searchString, int? specialtySearch)
        {
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
            
            ViewBag.spcID = new SelectList(db.Specialties, "spcID", "spcName");
            ViewBag.viewName = "create";
            return View();
        }

        // POST: AdminDashboard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "volID,volFirstName,volLastName,middleName,volDOB,volEmail,volPhone,volStreet1,volStreet2,volCity,volState,volZip,volStartDate,volActive,spcID")] Volunteer volunteer)
        {
            ViewBag.spcID = new SelectList(db.Specialties, "spcID", "spcName", volunteer.spcID);
            try
            {
                if (ModelState.IsValid)
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
            if (ModelState.IsValid)
            {
                db.Entry(volunteer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.volID = new SelectList(db.Econtacts, "volID", "ecFirstName", volunteer.volID);
            ViewBag.volID = new SelectList(db.Licenses, "volID", "volID", volunteer.volID);
            ViewBag.spcID = new SelectList(db.Specialties, "spcID", "spcName", volunteer.spcID);
            return View(volunteer);
        }

       
        // GET: AdminDashboard/AddSpecialty
        public ActionResult AddSpecialty()
        {
            return View("_AddSpecialty");
        }

        // POST: AdminDashboard/AddSpecialty
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSpecialty([Bind(Include = "spcID, spcName")] Specialty specialty)
        {
            try
            {
                if (ModelState.IsValid)
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

        public ActionResult VolunteerAvailable(int? id)
        {
            var volunteerID = id;
            var schedule = db.Availabilities.Where(s => s.volID == volunteerID).ToList();
            
            return PartialView("_VolunteerAvailable", schedule);
        }

        public ActionResult VolunteerTimesheet(int? id)
        {
            var volunteerID = id;
            var timesheet = db.Worklog.Where(s => s.volID == volunteerID).ToList();

            return PartialView("_VolunteerTimesheet", timesheet);
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


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //This may not be needed
        public ActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
