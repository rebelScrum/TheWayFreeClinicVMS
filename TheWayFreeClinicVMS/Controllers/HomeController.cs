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
using TheWayFreeClinicVMS.Models;


namespace TheWayFreeClinicVMS.Controllers
{

    public class HomePageMessage
    {
        public string filePath;
        public string fileName;
        public string fullText;
        public string preview;
    }

    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();        

        public ActionResult Index()
        {
            string text = "";

            DirectoryInfo d = new DirectoryInfo(Server.MapPath("~/Content/docs/HomePageMessages"));
            
            foreach (var file in d.GetFiles("*.txt"))
            {
                text += System.IO.File.ReadAllText(file.FullName);
            }

            ViewBag.message = text.Replace(Environment.NewLine, "<br />");
            ViewBag.error = TempData["error"];
            ViewBag.FullName = getUserName();
            var wlog = db.Worklog;

            var sorts = from s in wlog
                        select s;                          

            sorts = sorts.OrderByDescending(s => s.wrkDate);
            return View(sorts.ToList());          
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.FullName = getUserName();
            return View();
        }
        public ActionResult Help()
        {
            ViewBag.Message = "The help page.";
            ViewBag.FullName = getUserName();
            return View();
        }
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
                        newTime.wrkEndTime = null;                    
                        db.Worklog.Add(newTime);
                        db.SaveChanges();
                        ViewBag.clock = "Clocked In!";                      
                    }
                }
                catch (DataException)
                {
                    ViewBag.clock = "";
                    ViewBag.confirm = "";
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
            string text = "";
            int maxLength = 100;
            
            List<HomePageMessage> hpmList = new List<HomePageMessage>();

            DirectoryInfo d = new DirectoryInfo(Server.MapPath("~/Content/docs/HomePageMessages"));

            foreach (var file in d.GetFiles("*.txt"))
            {
                HomePageMessage hpm = new HomePageMessage();
                hpm.filePath = file.FullName;
                hpm.fileName = file.Name;
                hpm.fullText = System.IO.File.ReadAllText(file.FullName);

                if (hpm.fullText.Length > maxLength)
                {
                    hpm.preview = hpm.fullText.Substring(0, maxLength) + "...";  
                }
                else
                {
                    hpm.preview = hpm.fullText;
                }
                              
                hpmList.Add(hpm);          
            }

            ViewBag.message = text.Replace(Environment.NewLine, "<br />");

            ViewBag.FullName = getUserName();
            return View(hpmList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult homeMessageAdd(string message)
        {
            ViewBag.FullName = getUserName();
            var timeStamp = "[" + DateTime.Now.ToLongDateString() + "]";
            var fileName = DateTime.Now.ToString("MM-dd-yyyy_HHmmss") + ".txt";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Server.MapPath("~/Content/docs/HomePageMessages/") + (fileName), true))
            {
                file.WriteLine(timeStamp);
                file.WriteLine(message);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult homeMessageUpdate(string message, string fileName, string removeMessage)
        {
            ViewBag.FullName = getUserName();
            switch(removeMessage)
            {
                case "":
                    {
                        var timeStamp = "[" + DateTime.Now.ToLongDateString() + "]";
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(Server.MapPath("~/Content/docs/HomePageMessages/") + (fileName), false))
                        {
                            file.WriteLine(timeStamp);
                            file.WriteLine(message);
                            file.WriteLine();
                        }
                        break;
                    }
                case "remove":
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(Server.MapPath("~/Content/docs/HomePageMessages/") + (fileName));
                        try
                        {
                            fi.Delete();
                        }
                        catch (System.IO.IOException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult homeMessageDelete(string fileName)
        {
            ViewBag.FullName = getUserName();

            System.IO.FileInfo fi = new System.IO.FileInfo(Server.MapPath("~/Content/docs/HomePageMessages/") + (fileName));
            try
            {
                fi.Delete();
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine(e.Message);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult homeMessage(HttpPostedFileBase file)
        {
            ViewBag.FullName = getUserName();
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