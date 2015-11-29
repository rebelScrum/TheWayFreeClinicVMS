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
                new ManageVolunteerModels.Volunteer {volFirstName="Carson", volLastName="Alexander", volDOB=DateTime.Parse("01/12/1978"), volEmail="emailalex@gmail.com", volPhone="(904)789-7867", volStartDate=DateTime.Today, volStreet1="1223 Main Street", volStreet2="Apt 5",volCity="Jacksonville", volState="FL",
                volZip="32225"}
            };

        }
    }
}