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
    public class ManageJobsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ManageJobs
        public ActionResult Index(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var volunteer = id;
            ViewBag.volID = volunteer;
            var jobs = db.Jobs.Include(j => j.Employer).Include(j => j.Volunteer).Where(e => e.volID == volunteer);
            return View(jobs.ToList());
        }

        // GET: ManageJobs/Details/5
        public ActionResult Details(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // GET: ManageJobs/Create
        public ActionResult Create(int? id)
        {
            ViewBag.FullName = getUserName();
            ViewBag.empID = new SelectList(db.Employers, "empID", "empName");
            var volunteerID = id;
            Job job = new Job();
            job.volID = db.Volunteers.Where(s => s.volID == volunteerID).Select(v => v.volID).Single();
            return View(job);
        }

        // POST: ManageJobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "jobID,volID,empID,jobTitle,jobStartDate,jobEndDate")] Job job)
        {
            if (ModelState.IsValid)
            {
                db.Jobs.Add(job);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = job.volID });
            }

            ViewBag.empID = new SelectList(db.Employers, "empID", "empName", job.empID);
            return View(job);
        }

        // GET: ManageJobs/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            ViewBag.empID = new SelectList(db.Employers, "empID", "empName", job.empID);
            
            return View(job);
        }

        // POST: ManageJobs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "jobID,volID,empID,jobTitle,jobStartDate,jobEndDate")] Job job)
        {
            if (ModelState.IsValid)
            {
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = job.volID});
            }
            ViewBag.empID = new SelectList(db.Employers, "empID", "empName", job.empID);
            return View(job);
        }

        // GET: ManageJobs/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    ViewBag.FullName = getUserName();
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Job job = db.Jobs.Find(id);
        //    if (job == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(job);
        //}

        // POST: ManageJobs/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Job job = db.Jobs.Find(id);
        //    db.Jobs.Remove(job);
        //    db.SaveChanges();
        //    return RedirectToAction("Index", new { id = job.volID});
        //}

        //****************************************************************
        // Employer
        //****************************************************************
        // GET: ManageJobs/Create
        public ActionResult CreateEmployer()
        {
            ViewBag.FullName = getUserName();
            return View();
        }

        // POST: ManageJobs/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEmployer([Bind(Include = "empName,empPhone,empStreet1,empStreet2,empCity,empState,empZip")] Employer employer)
        {
            try
            {
                //var alreadyExists = db.Employers.Any(v => v.empName == employer.empName);
                if (ModelState.IsValid)
                {
                    db.Employers.Add(employer);
                    db.SaveChanges();

                    return RedirectToAction("IndexEmployer", "ManageJobs");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.Try again, and if the problem persists see your system administrator.");
            }

            return View(employer);    
        }
        public ActionResult IndexEmployer()
        {
            ViewBag.FullName = getUserName();

            var employers = db.Employers;
            return View(employers.ToList());
        }

        public string getUserName()
        {
            var vols = db.Volunteers;
            string fullName = (from v in vols
                               where v.volEmail == User.Identity.Name
                               select v.volLastName + ", " + v.volFirstName).FirstOrDefault();


            return fullName;
        }
        // GET: ManageJobs/Details/5
        public ActionResult DetailsEmployer(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employer emp = db.Employers.Find(id);
            if (emp == null)
            {
                return HttpNotFound();
            }
            return View(emp);
        }

        // GET: ManageJobs/Edit/5
        public ActionResult EditEmployer(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employer emp = db.Employers.Find(id);
            if (emp == null)
            {
                return HttpNotFound();
            }
            return View(emp);
        }

        // POST: ManageJobs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmployer([Bind(Include = "empID,empName,empPhone,empStreet1,empStreet2,empCity,empState,empZip")] Employer employer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexEmployer");
            }
            
            return View(employer);
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
