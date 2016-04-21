using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheWayFreeClinicVMS.Models
{
    public class RegisterVolunteerViewModel
    {
        public IEnumerable<Volunteer> Volunteer { get; set; }
        public IEnumerable<Language> Language { get; set; }
        public IEnumerable<Speak> Speaks { get; set; }
        public IEnumerable<Econtact> Econtact { get; set; }
        public IEnumerable<Job> Jobs { get; set; }
        public IEnumerable<Employer> Employer { get; set; }
        public IEnumerable<Contract> Contract { get; set; }
        public IEnumerable<Pagroup> PAGroup { get; set; }
        public IEnumerable<Availability> Availability { get; set; }
        public IEnumerable<Worktime> Worktime { get; set; }
        public IEnumerable<License> License { get; set; }
        public IEnumerable<Specialty> Specialty { get; set; }
    }
}