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
        public DbSet<ManageVolunteerModels.Volunteer> Volunteers { get; set; }
        public DbSet<ManageVolunteerModels.Employer> Employers { get; set; }
        public DbSet<ManageVolunteerModels.Pagroup> Pagroups { get; set; }
        public DbSet<WorktimeModels.Worktime> Worklog { get; set; }
        public DbSet<ManageVolunteerModels.Language> Languages { get; set; }
        public DbSet<ManageVolunteerModels.Speak> Speaks { get; set;}
        public DbSet<ManageVolunteerModels.Contract> Contracts { get; set; }
        public DbSet<ManageVolunteerModels.Job> Jobs { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}