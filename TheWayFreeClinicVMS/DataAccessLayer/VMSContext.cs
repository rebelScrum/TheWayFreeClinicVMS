using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using TheWayFreeClinicVMS.Models;

namespace TheWayFreeClinicVMS.DataAccessLayer
{
    
    public class VMSContext : DbContext
    {
        public VMSContext() : base("VMSContext")
            {

            }
     
        //Database table creation
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<Pagroup> Pagroups { get; set; }
        public DbSet<Worktime> Worklog { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Speak> Speaks { get; set;}
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Econtact> Econtacts { get; set; }
        public DbSet<License> Licenses { get; set; }
       // public object Volunteer { get; internal set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}