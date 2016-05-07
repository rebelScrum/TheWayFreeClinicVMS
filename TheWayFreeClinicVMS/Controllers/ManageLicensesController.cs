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
    [Authorize(Roles = "Admin")]
    public class ManageLicensesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ManageLicenses
        public ActionResult Index(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var volunteer = id;
            ViewBag.volID = volunteer;
            var licenses = db.Licenses.Where(e => e.volID == volunteer);

            return View(licenses.ToList());
        }


        // GET: ManageLicenses/Create
        public ActionResult Create(int? id)
        {
            ViewBag.FullName = getUserName();
            var volunteerID = id;
            License license = new License();
            license.volID = db.Volunteers.Where(s => s.volID == volunteerID).Select(v => v.volID).Single();

            return View(license);
        }

        // POST: ManageLicenses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "volID,lcNum,lcDate,lcClear,lcExpire")] License license)
        {
            if (ModelState.IsValid)
            {
                db.Licenses.Add(license);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = license.volID });
            }
            return View(license);
        }

        // GET: ManageLicenses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            License license = db.Licenses.Find(id);
            if (license == null)
            {
                return HttpNotFound();
            }
            return View(license);
        }

        // POST: ManageLicenses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "licenceID,volID,lcNum,lcDate,lcClear,lcExpire")] License license)
        {
            if (ModelState.IsValid)
            {
                db.Entry(license).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = license.volID });
            }
            ViewBag.volID = new SelectList(db.Volunteers, "volID", "volFirstName", license.volID);
            return View(license);
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
