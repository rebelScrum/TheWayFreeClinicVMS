namespace TheWayFreeClinicVMS.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<TheWayFreeClinicVMS.DataAccessLayer.VMSContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "TheWayFreeClinicVMS.DataAccessLayer.VMSContext";
        }

        protected override void Seed(TheWayFreeClinicVMS.DataAccessLayer.VMSContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            var specialty = new List<Specialty>
            {
                new Specialty {spcID=1, spcName="Physician" },
                new Specialty {spcID=2, spcName="Registered Nurse" },
                new Specialty {spcID=3, spcName="Support Staff" },
                new Specialty {spcID=4, spcName="Chiropractor" },
            };
            specialty.ForEach(sp => context.Specialties.AddOrUpdate(p => p.spcName, sp));
            context.SaveChanges();

            var volunteers = new List<Volunteer>
            {
                new Volunteer {volFirstName="Carson", volLastName="Alexander", volDOB=DateTime.Parse("01-12-1978"), volEmail="emailalex@gmail.com", volPhone="(904)789-7867", volStartDate=DateTime.Today, volStreet1="1223 Main Street", volStreet2="Apt 5",volCity="Jacksonville", volState="FL",
                volZip="32225", volActive=true,spcID=1},
                new Volunteer {volFirstName="Meredith", volLastName="Mathews", volDOB=DateTime.Parse("08-11-1988"), volEmail="mathews@gmail.com", volPhone="(904)678-1234", volStartDate=DateTime.Today, volStreet1="108 Middle Street", volStreet2="Apt 23",volCity="Jacksonville", volState="FL",
                volZip="32225", volActive=true, spcID=2},
                new Volunteer {volFirstName="Yan", volLastName="Lee", volDOB=DateTime.Parse("12-11-1967"), volEmail="leealexx@gmail.com", volPhone="(904)342-7447", volStartDate=DateTime.Today, volStreet1="25 Avenue East",volCity="Jacksonville", volState="FL",
                volZip="32224", volActive=false,spcID=3},
                new Volunteer {volFirstName="Daniel", volLastName="Chapman", volDOB=DateTime.Parse("09-04-1989"), volEmail="danchap@gmail.com", volPhone="(904)786-4563", volStartDate=DateTime.Today, volStreet1="876 Peerless Lane", volStreet2="Apt 376",volCity="Jacksonville", volState="FL",
                volZip="32267", volActive=true,spcID=4}
            };

            volunteers.ForEach(v => context.Volunteers.AddOrUpdate(p => p.volPhone, v));
            context.SaveChanges();

            var languages = new List<Language>
            {
                new Language {lngID = 1, lngName ="Spanish" },
                new Language {lngID = 2, lngName = "French" },
                new Language {lngID = 3, lngName = "Cantonese"}
            };

            languages.ForEach(l => context.Languages.AddOrUpdate(p => p.lngName, l));
            context.SaveChanges();

            var speaks = new List<Speak>
            {
                new Speak {lngID=1, volID = 4},
                new Speak {lngID=3, volID = 2},
                new Speak {lngID=2, volID = 3},
            };
            speaks.ForEach(s => context.Speaks.AddOrUpdate(p => p.volID, s));
            context.SaveChanges();

            var worklog = new List<Worktime>
            {
                new Worktime {wrkID=1, volID=1, wrkDate=DateTime.Parse("11-20-2015"), wrkEndTime=DateTime.UtcNow, wrkStartTime = DateTime.UtcNow },
                new Worktime {wrkID=2, volID=2, wrkDate=DateTime.Parse("11-21-2015"), wrkEndTime=DateTime.UtcNow, wrkStartTime = DateTime.UtcNow },
                new Worktime {wrkID=3, volID=1, wrkDate=DateTime.Parse("11-22-2015"), wrkEndTime=DateTime.UtcNow, wrkStartTime = DateTime.UtcNow },
                new Worktime {wrkID=4, volID=2, wrkDate=DateTime.Parse("11-23-2015"), wrkEndTime=DateTime.UtcNow, wrkStartTime = DateTime.UtcNow }
            };
            worklog.ForEach(w => context.Worklog.AddOrUpdate(p => p.wrkDate, w));
            context.SaveChanges();

            var employer = new List<Employer>
            {
                new Employer {empID=1, empName="Medical Hospital", empPhone="(904)347-7823", empStreet1="7896 Nice Street", empCity="Jacksonville", empState="FL", empZip="32224" }
            };
            employer.ForEach(e => context.Employers.AddOrUpdate(p => p.empName, e));
            context.SaveChanges();

            var job = new List<Job>
            {
                new Job {empID=1, volID=1, jobTitle="RN", jobStartDate=DateTime.Parse("11-20-2015") },
                new Job {empID=1, volID=2, jobTitle="Physiologist", jobStartDate=DateTime.Parse("11-25-2015") },
                new Job {empID=1, volID=2, jobTitle="Dentist", jobStartDate=DateTime.Parse("11-23-2015") }
            };
            job.ForEach(j => context.Jobs.AddOrUpdate(p => p.jobTitle, j));
            context.SaveChanges();

            var pagroup = new List<Pagroup>
            {
                new Pagroup {pgrID=1, pgrName="Group Name", pgrOfcFirstName="Jack", pgrOfcLastName="Sparrow", pgrPhone="(904)789-4564", pgrStreet1="765 1st Street", pgrCity="Jacksonville", pgrState="FL", pgrZip="34452"}

            };
            pagroup.ForEach(p => context.Pagroups.AddOrUpdate(r => r.pgrName, p));
            context.SaveChanges();

            var contract = new List<Contract>
            {
                new Contract {volID=1, ctrNum=190787, pgrID=1 },
                new Contract {volID=2, ctrNum=684563, pgrID=1 }
            };
            contract.ForEach(c => context.Contracts.AddOrUpdate(p => p.ctrNum, c));
            context.SaveChanges();

            var econtact = new List<Econtact>
            {
                new Econtact {volID=1, ecFirstName="Mark", ecLastName="Hamill", ecPhone="(904)675-56745" },
                new Econtact {volID=2, ecFirstName="Maria", ecLastName="Fisher", ecPhone="(904)897-67856" },
                new Econtact {volID=3, ecFirstName="David", ecLastName="Gabriel", ecPhone="(904)567-3564" },
                new Econtact {volID=4, ecFirstName="John", ecLastName="Gilmour", ecPhone="(904)456-3424" }
            };
            econtact.ForEach(ec => context.Econtacts.AddOrUpdate(p => p.ecLastName, ec));
            context.SaveChanges();

            var availability = new List<Availability>
            {
                new Availability {avID=1, volID=1, avDay=DaysAvailable.Monday, avFrom=DateTime.Parse("9:00"), avUntil=DateTime.Parse("5:00") },
                new Availability {avID=2, volID=1, avDay=DaysAvailable.Wednesday, avFrom=DateTime.Parse("12:00"), avUntil=DateTime.Parse("6:00") },
                new Availability {avID=3, volID=1, avDay=DaysAvailable.Friday, avFrom=DateTime.Parse("9:00"), avUntil=DateTime.Parse("3:00") },
                new Availability {avID=4, volID=2, avDay=DaysAvailable.Saturday, avFrom=DateTime.Parse("9:00"), avUntil=DateTime.Parse("5:00") },
                new Availability {avID=5, volID=4, avDay=DaysAvailable.Tuesday, avFrom=DateTime.Parse("10:00"), avUntil=DateTime.Parse("3:00") },
                new Availability {avID=6, volID=4, avDay=DaysAvailable.Thursday, avFrom=DateTime.Parse("10:00"), avUntil=DateTime.Parse("3:00") },
            };
            availability.ForEach(av => context.Availabilities.AddOrUpdate(p => p.avDay, av));
            context.SaveChanges();

            var license = new List<License>
            {
                new License {volID=1, lcDate=DateTime.Parse("09-12-2015"),lcExpire=DateTime.Parse("09-12-2020"), lcClear=true },
                new License {volID=4, lcDate=DateTime.Parse("07-04-2015"),lcExpire=DateTime.Parse("07-04-2027"), lcClear=true }
            };
            license.ForEach(lc => context.Licenses.AddOrUpdate(p => p.lcDate, lc));
            context.SaveChanges();
        }
    }
}
