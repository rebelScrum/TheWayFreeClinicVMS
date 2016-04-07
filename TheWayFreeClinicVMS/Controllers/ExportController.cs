﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TheWayFreeClinicVMS.Models;
//using Microsoft.Office.Interop.Excel;

namespace TheWayFreeClinicVMS.Controllers
{  
    public class ExportController : Controller
    {
        public class HoursReportVol
        {
            public int id { get; set; }
            public double hours { get; set; }
            public Volunteer volunteer { get; set; }
        }

        private ApplicationDbContext db = new ApplicationDbContext();        

        // GET: Export
        public ActionResult ExportHoursReport(string hiddenDateRange, int? specialtySearch)
        {
            ViewBag.viewName = "index";
            ViewBag.FullName = getUserName();
            string[] tokens = new string[] { " - " };
            string[] dateRange;
            long begDateTicks = 0000000000;
            long endDateTicks = 0000000000;
            long tempTotal = 0000000000;
            var volHours = 0.0;
            var grandTotalHours = 0.0;

            List<AdminDashboardController.HoursReportVol> HoursReportFullList = new List<AdminDashboardController.HoursReportVol> { };
            List<AdminDashboardController.HoursReportVol> HoursReportFilteredList = new List<AdminDashboardController.HoursReportVol> { };
            AdminDashboardController.HoursReportVol vol = new AdminDashboardController.HoursReportVol();

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

            if (specialtySearch.HasValue)
            {
                sorts = sorts.Where(s => s.spcID == specialtyID);
            }

            //create new object for each vol in sorts including properties for hours and exposing volID; adds to list of new objects
            foreach (var item in sorts)
            {
                vol = new AdminDashboardController.HoursReportVol();
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

                        if (beg >= begDateTicks && end <= endDateTicks)
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

            ExportHoursReportToCSV(HoursReportFilteredList, hiddenDateRange);

            return View(HoursReportFilteredList);
        }

        public string getUserName()
        {
            var vols = db.Volunteers;
            string fullName = (from v in vols
                               where v.volEmail == User.Identity.Name
                               select v.volLastName + ", " + v.volFirstName).FirstOrDefault();
            return fullName;
        }

        public void ExportHoursReportToCSV(List<AdminDashboardController.HoursReportVol> Data, string hiddenDateRange)
        {                 
            StringWriter sw = new StringWriter();

            sw.WriteLine("\"Last Name\",\"First Name\",\"Specialty\",\"Active\",\"E-mail\",\"Hours\"");
            string hdr = "attachment;filename=" + hiddenDateRange.Replace(",", "") + ".csv";
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
        }

    }
}