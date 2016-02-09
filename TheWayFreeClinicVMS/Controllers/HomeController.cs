using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TheWayFreeClinicVMS.DataAccessLayer;
using TheWayFreeClinicVMS.Models;


namespace TheWayFreeClinicVMS.Controllers
{
    public class HomeController : Controller
    {
        private VMSContext db = new VMSContext();

        public ActionResult Index()
        {
            string text = System.IO.File.ReadAllText(Server.MapPath("~/Content/docs/") + ("message1.txt"));
            ViewBag.message = text;
            ViewBag.error = TempData["error"];
            var wlog = db.Worklog;

            var sorts = from s in wlog
                        select s;

            sorts = sorts.OrderByDescending(s => s.wrkDate);
            return View(sorts.ToList());
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

<<<<<<< HEAD
        public ActionResult Help()
        {
            ViewBag.Message = "The help page.";

            return View();
        }

=======
>>>>>>> refs/remotes/origin/Home-Page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "wrkID,volID,wrkDate,wrkStartTime,wrkEndTime")] string email)
        {
            string text = System.IO.File.ReadAllText(Server.MapPath("~/Content/docs/") + ("message1.txt"));
            ViewBag.message = text;
            ViewBag.confirm = "you are now...";
            ViewBag.clock = "";

            var wlog = db.Worklog.Include(v => v.Volunteer);
            var volunteers = db.Volunteers;

            //get id from email
            var thisVolID = (from i in volunteers  where i.volEmail == email select i.volID).SingleOrDefault();

            //will set Worktime object if Endtime null. i.e., user clocked-in. 
            Worktime time = (from w in wlog where w.volID == thisVolID && w.wrkEndTime == null select w).SingleOrDefault();
            
            //if wrkEndTime null, user is still clocked in. Update wrkEndTime with timestamp. 
            //Now user has no worktime record with empty end time. At next entry query will return null and move to else.
            if (time != null)
            {                
                time.wrkDate = DateTime.Now;
                time.wrkEndTime = DateTime.Now;
                ViewBag.clock = "Clocked Out!";
                db.SaveChanges();
            }
            else //this user has no record containing null wrkEndTime
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        Worktime newTime = new Worktime();

                        newTime.volID = thisVolID;
                        newTime.wrkDate = DateTime.Now;
                        newTime.wrkStartTime = DateTime.Now;
                        newTime.wrkEndTime = null;//same as startTime, signifying clocked in.
                        ViewBag.clock = "Clocked In!";
                        db.Worklog.Add(newTime);
                        db.SaveChanges();                        
                    }
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "We cannot find your account. Try again. If the problem persists, contact your system administrator.");
                }
            }

            var sorts = from s in wlog
                        select s;

            sorts = sorts.OrderByDescending(s => s.wrkDate);

            return View(sorts.ToList());
        }

        public ActionResult homeMessage()
        {
            return View();
        }

        [HttpPost, ActionName("textBoxAction")]
        [ValidateAntiForgeryToken]
        public ActionResult homeMessage(string message)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Server.MapPath("~/Content/docs/") + ("message1.txt"), true))
            {
                file.WriteLine(message);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult homeMessage(HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string fullPath = Request.MapPath("~/Content/img/homePageImg/" + "img");

                    string[] filePaths = Directory.GetFiles(Server.MapPath("~/Content/img/homePageImg/"));
                    foreach (string filePath in filePaths)
                    {
                        System.IO.File.Delete(filePath);
                    }

                    string extension = Path.GetExtension(file.FileName);
                    string imagePath = null;

                    imagePath = Server.MapPath("~/Content/img/homePageImg/" + "img" + extension);
                    file.SaveAs(imagePath);

                    string renamedImagePath = Server.MapPath("~/Content/img/homePageImg/" + "img");
                    System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath);
                    if (extension != ".png")
                    {
                        image.Save(renamedImagePath + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    }
                    image.Dispose();                   
                    
                }
                else
                {
                    TempData["error"] = "ModelState Not Valid";
                }
            }
            
            return RedirectToAction("Index");
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