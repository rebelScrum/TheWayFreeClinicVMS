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

                //only add messages < 30 days old
                DateTime msgDate = Convert.ToDateTime(file.Name.Replace("-", "/").Remove(file.Name.IndexOf("_")));
                DateTime expiryDate = msgDate.AddDays(30);
                List<HomePageMessage> hpmList = new List<HomePageMessage>();

                bool expired = DateTime.Now > expiryDate;

                if (expired)
                {
                    try
                    {
                        System.IO.File.Move(Server.MapPath("~/Content/docs/HomePageMessages/") + (file.Name), Server.MapPath("~/Content/docs/HomePageMessages/HomePageMessagesArchive/") + (file.Name));
                    }
                    catch (System.IO.IOException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    text += System.IO.File.ReadAllText(file.FullName);
                }
                
            }

            ViewBag.message = text.Replace(Environment.NewLine, "<br />");
            ViewBag.error = TempData["error"];
            ViewBag.FullName = getUserName();
            var wlog = db.Worklog;

            var sorts = from s in wlog
                        select s;

            sorts = sorts.OrderByDescending(s => s.wrkDate);
            sorts = sorts.Take(15);
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
            ViewBag.FullName = getUserName();

            string text = "";

            DirectoryInfo d = new DirectoryInfo(Server.MapPath("~/Content/docs/HomePageMessages"));

            foreach (var file in d.GetFiles("*.txt"))
            {
                text += System.IO.File.ReadAllText(file.FullName);
            }

            ViewBag.message = text.Replace(Environment.NewLine, "<br />");

            ViewBag.confirm = email + " is now... ";

            ViewBag.clock = "";

            var wlog = db.Worklog.Include(v => v.Volunteer);
            var volunteers = db.Volunteers;

            //get id from email
            var thisVolID = (from i in volunteers where i.volEmail == email select i.volID).SingleOrDefault();

            //will set Worktime object if Endtime null. i.e., user clocked-in. 
            Worktime time = (from w in wlog where w.volID == thisVolID && w.wrkEndTime == null select w).SingleOrDefault();
            TimeSpan closingTime = new TimeSpan(17, 00, 00);
            //if wrkEndTime null, user is still clocked in. Update wrkEndTime with timestamp. 
            //Then, user has no worktime record with empty end time. At next entry query will return null and move to else.

            if (time != null) //user still clocked in
            {
                time.wrkDate = DateTime.Now;
                time.wrkEndTime = DateTime.Now;

                if (time.wrkEndTime.Value.Date != time.wrkStartTime.Date) //if clocked out the day after clocked in
                {
                    time.wrkEndTime = new DateTime(time.wrkStartTime.Year, time.wrkStartTime.Month, time.wrkStartTime.Day, 17, 0, 0);
                    ViewBag.SignInMsg = "You were still signed-in. \n\n Last sign-in occured: " + time.wrkStartTime + "\n\n You are now signed-out for that day (5:00 PM). \n Please enter your e-mail again if you want to sign-in for today";

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Server.MapPath("~/Content/docs/WorklogDataMessages/") + ("WorklogDataFlags.txt"), true))
                    {
                        var flag = time.wrkID + ",2;";
                        file.Write(flag);
                    }                    //send notification to admin
                }
                else if (time.wrkEndTime.Value.TimeOfDay >= closingTime) //clocked out same day as clock in, but after 5 PM
                {
                    time.wrkEndTime = new DateTime(time.wrkStartTime.Year, time.wrkStartTime.Month, time.wrkStartTime.Day, 17, 0, 1);
                    ViewBag.SignInMsg = "You were still signed-in. \n\n Last sign-in occured: " + time.wrkStartTime + "\n\n You are now signed-out for that day (5:00 PM). \n Please enter your e-mail again if you want to sign-in for today";

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Server.MapPath("~/Content/docs/WorklogDataMessages/") + ("WorklogDataFlags.txt"), true))
                    {
                        var flag = time.wrkID + ",1;";
                        file.Write(flag);
                    }                    //send notification to admin
                }

                ViewBag.clock = "Clocked Out!";
                db.SaveChanges();
            }
            else //user has no record containing null wrkEndTime, user currently clocked out
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (DateTime.Now.TimeOfDay >= closingTime) // if user tries to clock in after 5pm today
                        {
                            ViewBag.confirm = "";
                            ViewBag.clock = "You cannot sign in after 5 PM. Please see your administrator for more information.";
                        }
                        else //normal working hours
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
            sorts = sorts.Take(15);
            return View(sorts.ToList());
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult homeMessageAdd(string message)
        {
            ViewBag.FullName = getUserName();
            var timeStamp = "[" + DateTime.Now.ToLongDateString() + "]";
            var fileName = DateTime.Now.ToString("MM-dd-yyyy_HHmmss") + ".txt";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Server.MapPath("~/Content/docs/HomePageMessages/") + (fileName), false))
            {
                file.WriteLine(timeStamp);
                file.WriteLine(message);
                file.WriteLine();
            }

            return RedirectToAction("homeMessage");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult homeMessageUpdate(string message, string fileName, string removeMessage)
        {
            ViewBag.FullName = getUserName();
            switch (removeMessage)
            {
                case "":
                    {
                        var timeStamp = "[" + DateTime.Now.ToLongDateString() + "]";
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(Server.MapPath("~/Content/docs/HomePageMessages/") + (fileName), false))
                        {
                            file.Write("");
                            file.WriteLine(timeStamp);
                            file.WriteLine(message);
                            file.WriteLine();
                        }
                        break;
                    }
                case "remove":
                    {
                        try
                        {
                            System.IO.File.Move(Server.MapPath("~/Content/docs/HomePageMessages/") + (fileName), Server.MapPath("~/Content/docs/HomePageMessages/HomePageMessagesArchive/") + (fileName));
                        }
                        catch (System.IO.IOException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    }
            }

            return RedirectToAction("homeMessage");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult homeAllMessages()
        {
            string text = "";
            int maxLength = 100;

            List<HomePageMessage> hpmList = new List<HomePageMessage>();

            DirectoryInfo d = new DirectoryInfo(Server.MapPath("~/Content/docs/HomePageMessages/HomePageMessagesArchive/"));

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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult homeMessageDelete(string fileName, string removeMessage)
        {
            ViewBag.FullName = getUserName();

            switch (removeMessage)
            {
                case "restore":
                    {
                        try
                        {
                            System.IO.File.Move(Server.MapPath("~/Content/docs/HomePageMessages/HomePageMessagesArchive/") + (fileName), Server.MapPath("~/Content/docs/HomePageMessages/") + (fileName));
                        }
                        catch (System.IO.IOException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    }
                case "delete":
                    {
                        try
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(Server.MapPath("~/Content/docs/HomePageMessages/HomePageMessagesArchive/") + (fileName));

                            fi.Delete();
                        }
                        catch (System.IO.IOException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    }
            }

            return RedirectToAction("homeAllMessages");
        }

        [Authorize(Roles = "Admin")]
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