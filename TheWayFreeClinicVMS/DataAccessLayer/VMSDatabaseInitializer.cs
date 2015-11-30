using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TheWayFreeClinicVMS.Models;

namespace TheWayFreeClinicVMS.DataAccessLayer
{
    //drops database if model changes
    //recreates it to reflect changes
    //populates db with test data
    
    //TODO: change to migrations in production
    public class VMSDatabaseInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<VMSContext>
    {
        protected override void Seed(VMSContext context)
        {
            var volunteers = new List<ManageVolunteerModels.Volunteer>
            {
                new ManageVolunteerModels.Volunteer {volFirstName="Carson", volLastName="Alexander", volDOB=DateTime.Parse("01-12-1978"), volEmail="emailalex@gmail.com", volPhone="(904)789-7867", volStartDate=DateTime.Today, volStreet1="1223 Main Street", volStreet2="Apt 5",volCity="Jacksonville", volState="FL",
                volZip="32225"},
                new ManageVolunteerModels.Volunteer {volFirstName="Meredith", volLastName="Mathews", volDOB=DateTime.Parse("08-11-1988"), volEmail="mathews@gmail.com", volPhone="(904)678-1234", volStartDate=DateTime.Today, volStreet1="108 Middle Street", volStreet2="Apt 23",volCity="Jacksonville", volState="FL",
                volZip="32225"},
                new ManageVolunteerModels.Volunteer {volFirstName="Yan", volLastName="Lee", volDOB=DateTime.Parse("12-11-1967"), volEmail="leealexx@gmail.com", volPhone="(904)342-7447", volStartDate=DateTime.Today, volStreet1="25 Avenue East",volCity="Jacksonville", volState="FL",
                volZip="32224"}
            };

            volunteers.ForEach(v => context.Volunteers.Add(v));
            context.SaveChanges();

            var languages = new List<ManageVolunteerModels.Language>
            {
                new ManageVolunteerModels.Language {lngID = 1, lngName ="Spanish" },
                new ManageVolunteerModels.Language {lngID = 2, lngName = "French" },
                new ManageVolunteerModels.Language {lngID = 3, lngName = "Cantonese"}
            };

            languages.ForEach(l => context.Languages.Add(l));
            context.SaveChanges();

            var speaks = new List<ManageVolunteerModels.Speak>
            {
                new ManageVolunteerModels.Speak {lngID=1, volID = 1},
                new ManageVolunteerModels.Speak {lngID=3, volID = 1},
                new ManageVolunteerModels.Speak {lngID=2, volID = 3},
            };
            speaks.ForEach(s => context.Speaks.Add(s));
            context.SaveChanges();

            var worklog = new List<WorktimeModels.Worktime>
            {
                new WorktimeModels.Worktime {wrkID=1, volID=1, wrkDate=DateTime.Parse("11-20-2015"), wrkEndTime=DateTime.UtcNow, wrkStartTime = DateTime.UtcNow },
                new WorktimeModels.Worktime {wrkID=2, volID=2, wrkDate=DateTime.Parse("11-21-2015"), wrkEndTime=DateTime.UtcNow, wrkStartTime = DateTime.UtcNow },
                new WorktimeModels.Worktime {wrkID=3, volID=1, wrkDate=DateTime.Parse("11-21-2015"), wrkEndTime=DateTime.UtcNow, wrkStartTime = DateTime.UtcNow },
                new WorktimeModels.Worktime {wrkID=4, volID=2, wrkDate=DateTime.Parse("11-22-2015"), wrkEndTime=DateTime.UtcNow, wrkStartTime = DateTime.UtcNow }
            };
            worklog.ForEach(w => context.Worklog.Add(w));
            context.SaveChanges();

            var employer = new List<ManageVolunteerModels.Employer>
            {
                new ManageVolunteerModels.Employer {empID=1, empName="Medical Hospital", empPhone="(904)347-7823", empStreet1="7896 Nice Street", empCity="Jacksonville", empState="FL", empZip="32224" }
            };
            employer.ForEach(e => context.Employers.Add(e));
            context.SaveChanges();

            var job = new List<ManageVolunteerModels.Job>
            {
                new ManageVolunteerModels.Job {empID=1, volID=1, jobTitle="RN", jobStartDate=DateTime.Parse("11-20-2015") },
                new ManageVolunteerModels.Job {empID=1, volID=2, jobTitle="Physiologist", jobStartDate=DateTime.Parse("11-25-2015") },
                new ManageVolunteerModels.Job {empID=1, volID=2, jobTitle="Dentist", jobStartDate=DateTime.Parse("11-23-2015") }
            };
            job.ForEach(j => context.Jobs.Add(j));
            context.SaveChanges();

            var contract = new List<ManageVolunteerModels.Contract>
            {
                new ManageVolunteerModels.Contract {volID=1, ctrNum=190787, pgrID=1 },
                new ManageVolunteerModels.Contract {volID=2, ctrNum=684563, pgrID=1 }
            };
            contract.ForEach(c => context.Contracts.Add(c));
            context.SaveChanges();

            var pagroup = new List<ManageVolunteerModels.Pagroup>
            {
                new ManageVolunteerModels.Pagroup {pgrID=1, pgrName="Group Name", pgrOfcFirstName="Jack", pgrOfcLastName="Sparrow", pgrPhone="(904)789-4564", pgrStreet1="765 1st Street", pgrCity="Jacksonville", pgrState="FL", pgrZip="34452"}

            };
            pagroup.ForEach(p => context.Pagroups.Add(p));
            context.SaveChanges();
        } 
    }
}