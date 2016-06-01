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
    public class ManageContractsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ManageContracts
        [Authorize(Roles = "Admin, Volunteer")]
        public ActionResult Index(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var volunteer = id;
            ViewBag.volID = volunteer;
            var contracts = db.Contracts.Include(j => j.Pagroup).Include(j => j.Volunteer).Where(e => e.volID == volunteer);
            return View(contracts.ToList());
        }

        // GET: ManageContracts/Create
        [Authorize(Roles = "Admin, Volunteer")]
        public ActionResult Create(int? id)
        {
            ViewBag.pgrID = new SelectList(db.Pagroups, "pgrID", "pgrName");
            ViewBag.FullName = getUserName();
            var volunteerID = id;
            Contract contract = new Contract();
            contract.volID = db.Volunteers.Where(s => s.volID == volunteerID).Select(v => v.volID).Single();
            return View(contract);
        }

        // POST: ManageContracts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Volunteer")]
        public ActionResult Create([Bind(Include = "contrID,ctrNum,volID,pgrID")] Contract contract)
        {
            
            if (ModelState.IsValid)
            {
                db.Contracts.Add(contract);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = contract.volID });
            }

            ViewBag.pgrID = new SelectList(db.Pagroups, "pgrID", "pgrName", contract.pgrID);
            ViewBag.volID = new SelectList(db.Volunteers, "volID", "volFirstName", contract.volID);
            return View(contract);
        }

        // GET: ManageContracts/Edit/5
        [Authorize(Roles = "Admin, Volunteer")]
        public ActionResult Edit(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            ViewBag.pgrID = new SelectList(db.Pagroups, "pgrID", "pgrName", contract.pgrID);
            ViewBag.volID = new SelectList(db.Volunteers, "volID", "volFirstName", contract.volID);
            return View(contract);
        }

        // POST: ManageContracts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Volunteer")]
        public ActionResult Edit([Bind(Include = "contrID,ctrNum,volID,pgrID")] Contract contract)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contract).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = contract.volID });
            }
            ViewBag.pgrID = new SelectList(db.Pagroups, "pgrID", "pgrName", contract.pgrID);
            ViewBag.volID = new SelectList(db.Volunteers, "volID", "volFirstName", contract.volID);
            return View(contract);
        }

        //****************************************************************
        // PA Group
        //****************************************************************
        // GET: ManageContracts/Create
        public ActionResult CreateGroup()
        {
            ViewBag.FullName = getUserName();
            return View();
        }

        // POST: Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGroup([Bind(Include = "pgrID, pgrName, pgrOfcFirstName, pgrOfcLastName, pgrPhone, pgrStreet1, pgrStreet2, pgrCity, pgrState, pgrZip")] Pagroup group)
        {
            try
            {
                //var alreadyExists = db.Employers.Any(v => v.empName == employer.empName);
                if (ModelState.IsValid)
                {
                    db.Pagroups.Add(group);
                    db.SaveChanges();

                    return RedirectToAction("IndexGroup", "ManageContracts");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.Try again, and if the problem persists see your system administrator.");
            }

            return View(group);
        }

        public ActionResult IndexGroup()
        {
            ViewBag.FullName = getUserName();

            var pagroups = db.Pagroups;
            return View(pagroups.ToList());
        }

        // GET: Edit/5
        public ActionResult EditGroup(int? id)
        {
            ViewBag.FullName = getUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pagroup group = db.Pagroups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: ManageJobs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGroup([Bind(Include = "pgrID,pgrName,pgrOfcFirstName,pgrOfcLastName,pgrPhone,pgrStreet1,pgrStreet2,pgrCity,pgrState,pgrZip")] Pagroup group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexGroup");
            }

            return View(group);
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
