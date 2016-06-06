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
    [Authorize(Roles = "Admin, Volunteer")]
    public class ManageAvailabilitiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ManageAvailabilities/
        public ActionResult ViewAvailability(int? id)
        {
            ViewBag.FullName = getUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var volunteer = id;
            ViewBag.volID = volunteer;
            var schedule = db.Availabilities.Where(s => s.volID == volunteer).ToList();

            return View(schedule);
        }
        // Get: ManageAvailabilities/Create
        [HttpGet]
        public ActionResult AddAvailability(int? id)
        {
            ViewBag.FullName = getUserName();
            var volunteerID = id;
            ViewBag.days = new SelectList(db.Availabilities, "DaysAvailable");
            Availability availability = new Availability();
            availability.volID = db.Volunteers.Where(s => s.volID == volunteerID).Select(v => v.volID).Single();
            return View(availability);
        }

        // POST: ManageAvailabilities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAvailability([Bind(Include = "volID,avDay,avFrom,avUntil")] Availability availability)
        {
            
            if (ModelState.IsValid)
            {
                
                db.Availabilities.Add(availability);
                db.SaveChanges();
                return RedirectToAction("ViewAvailability", new { id = availability.volID });
            }
            return View(availability);
        }

        // GET: ManageAvailabilities/Edit/5
        public ActionResult EditAvailability(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Availability availability = db.Availabilities.Find(id);
            
            if (availability == null)
            {
                return HttpNotFound();
            }
            ViewBag.volID = new SelectList(db.Volunteers, "volID", "volFirstName", availability.volID);
            return View(availability);
        }

        // POST: ManageAvailabilities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAvailability([Bind(Include = "avID,volID,avDay,avFrom,avUntil")] Availability availability)
        {
            
            if (ModelState.IsValid)
            {
                db.Entry(availability).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewAvailability", new { id = availability.volID });
            }
           
            return View(availability);
        }

        // GET: ManageAvailabilities/Delete/5
        public ActionResult DeleteAvailability(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Availability availability = db.Availabilities.Find(id);
            if (availability == null)
            {
                return HttpNotFound();
            }
            return View(availability);
        }

        // POST: AdminDashboard/DeleteAvailability
        [HttpPost, ActionName("DeleteAvailability")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Availability availability = db.Availabilities.Find(id);
            db.Availabilities.Remove(availability);
            db.SaveChanges();
            return RedirectToAction("ViewAvailability", new { id = availability.volID});
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
