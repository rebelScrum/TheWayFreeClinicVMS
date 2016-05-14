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
    public class ManageEcontactsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ManageEcontacts
        public ActionResult Index(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var volunteer = id;
            ViewBag.volID = volunteer;
            var econtacts = db.Econtacts.Where(e => e.volID == volunteer);
            
            return View(econtacts.ToList());
        }


        // GET: ManageEcontacts/Create
        public ActionResult Create(int? id)
        {
            ViewBag.FullName = getUserName();
            var volunteerID = id;
            Econtact econtact = new Econtact();
            econtact.volID = db.Volunteers.Where(s => s.volID == volunteerID).Select(v => v.volID).Single();
            return View(econtact);
        }

        // POST: ManageEcontacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "volID,ecFirstName,ecLastName,ecPhone")] Econtact econtact)
        {
            if (ModelState.IsValid)
            {
                db.Econtacts.Add(econtact);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = econtact.volID});
            }
    
            return View(econtact);
        }

        // GET: ManageEcontacts/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Econtact econtact = db.Econtacts.Find(id);
            if (econtact == null)
            {
                return HttpNotFound();
            }
            ViewBag.volID = new SelectList(db.Volunteers, "volID", "volFirstName", econtact.volID);
            return View(econtact);
        }

        // POST: ManageEcontacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ecID,volID,ecFirstName,ecLastName,ecPhone")] Econtact econtact)
        {
            if (ModelState.IsValid)
            {
                db.Entry(econtact).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = econtact.volID});
            }
            ViewBag.volID = new SelectList(db.Volunteers, "volID", "volFirstName", econtact.volID);
            return View(econtact);
        }

        // GET: ManageEcontacts/Delete/5
        public ActionResult Delete(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Econtact econtact = db.Econtacts.Find(id);
            if (econtact == null)
            {
                return HttpNotFound();
            }
            return View(econtact);
        }

        // POST: ManageEcontacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Econtact econtact = db.Econtacts.Find(id);
            db.Econtacts.Remove(econtact);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = econtact.volID});
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
