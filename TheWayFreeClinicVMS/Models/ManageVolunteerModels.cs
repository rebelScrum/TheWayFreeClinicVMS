using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheWayFreeClinicVMS.Models
{
    public class ManageVolunteerModels
    {
        //********************************************************************************************
        public class Volunteer
        {
            //ID
            [Key]
            public int volID { get; set; }

            // First Name
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,25}$",
            ErrorMessage = "Numbers and special characters are not allowed in the first name.")]
            [Required]
            [StringLength(25)]
            [Display(Name = "First Name")]
            public string volFirstName { get; set; }

            //Last Name
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$",
            ErrorMessage = "Numbers and special characters are not allowed in the last name.")]
            [Required]
            [StringLength(30)]
            [Display(Name = "Last Name")]
            public string volLastName { get; set; }

            //Middle Name
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$",
            ErrorMessage = "Numbers and special characters are not allowed in the middle name.")]
            [StringLength(30)]
            [Display(Name = "Middle Name")]
            public string middleName { get; set; }

            //date of birthday
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
            public DateTime volDOB { get; set; }

            //email
            [Required]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string volEmail { get; set; }

            //phone
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Phone Number")]
            [Required(ErrorMessage = "Phone Number Required!")]
            [RegularExpression(@"^((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}$", ErrorMessage = "Entered phone format is not valid. Use (999)999-9999 format.")]
            [StringLength(15)]
            public string volPhone { get; set; }

            //street1
            [Required]
            [StringLength(30)]
            public string volStreet1 { get; set; }

            //street2
            [StringLength(30)]
            public string volStreet2 { get; set; }

            //city
            [Required]
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,25}$")]
            [StringLength(25)]
            public string volCity { get; set; }

            //state
            [Required]
            [RegularExpression(@"^[A-Z\s]{2}$")]
            [StringLength(2)]
            public string volState { get; set; }

            //zip
            [Required]
            [RegularExpression(@"^(\d{5})$")]
            [StringLength(5)]
            public string volZip { get; set; }

            //start date
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
            public DateTime volStartDate { get; set; }

            //specialty ID, foreign key from specialty look up table
           
            public int spc_ID { get; set; }

            // EF navigation relationships
            public virtual Econtact Econtact { get; set; }

            public virtual ICollection<Speak> Speaks { get; set; }

            public virtual Specialty Specialty { get; set; }

            public virtual ICollection<Contract> Contracts { get; set; }

            public virtual ICollection<Availability> Available { get; set; }

            public virtual ICollection<Job> Jobs { get; set; }

            public virtual License License { get; set; }

            public virtual ICollection<WorktimeModels.Worktime> Worklog { get; set; }
        }

        //**********************************************************************************************
        public class Econtact
        {
            //emergency contact id
            [Key, Column(Order = 0)]
            public int ecID { get; set; }

            //volunteer ID
            [Key, Column(Order = 1)]
            public int volID { get; set; }

            //First Name
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,25}$",
            ErrorMessage = "Numbers and special characters are not allowed in the first name.")]
            [Required]
            [StringLength(25)]
            [Display(Name = "First Name")]
            public string ecFirstName { get; set; }

            //Last Name
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$",
            ErrorMessage = "Numbers and special characters are not allowed in the last name.")]
            [Required]
            [StringLength(30)]
            [Display(Name = "Last Name")]
            public string ecLastName { get; set; }

            //Phone
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Phone Number")]
            [Required(ErrorMessage = "Phone Number Required!")]
            [RegularExpression(@"^((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}$", ErrorMessage = "Entered phone format is not valid. Use (999)999-9999 format.")]
            [StringLength(15)]
            public string ecPhone { get; set; }

            //navigation Volunteer acts as relationship identifier
            public virtual Volunteer Volunteer { get; set; }
        }

        //*************************************************************************************************

        public class Employer
        {
            //id
            [Key]
           
            public int empID { get; set; }

            //employer name
            [Required]
            [MaxLength(30)]
            [MinLength(2)]
            public string empName { get; set; }

            //phone
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Phone Number")]
            [Required(ErrorMessage = "Phone Number Required!")]
            [RegularExpression(@"^((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}$", ErrorMessage = "Entered phone format is not valid. Use (999)999-9999 format.")]
            [StringLength(15)]
            public string empPhone { get; set; }

            //street1
            [Required]
            [StringLength(30)]
            public string empStreet1 { get; set; }

            //street2
            [StringLength(30)]
            public string empStreet2 { get; set; }

            //city
            [Required]
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,25}$")]
            [StringLength(25)]
            public string empCity { get; set; }

            //state
            [Required]
            [RegularExpression(@"^[A-Z\s]{2}$")]
            [StringLength(2)]
            public string empState { get; set; }

            //zip
            [Required]
            [RegularExpression(@"^(\d{5})$")]
            [StringLength(5)]
            public string empZip { get; set; }

            //navigation jobs acts as relationship identifier, many jobs for one employer
            public virtual ICollection<Job> Jobs { get; set; }
        }
        
        //*******************************************************************************************
        public class Job
        {
            //volunteer id
            [Key, Column(Order = 0)]

            public int volID { get; set; }

            //employer id
            [Key, Column(Order = 1)]
            public int empID { get; set; }

            //job title
            [Required]
            [MaxLength(30)]
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$",
            ErrorMessage = "Numbers and special characters are not allowed in the job title")]
            public string jobTitle { get; set; }

            //job start date
            [Required]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
            public DateTime jobStartDate { get; set; }

            //job end date
            [Required]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
            public DateTime jobEndDate { get; set; }

            //navigation Volunteer acts as relationship identifier
            public virtual Volunteer Volunteer { get; set; }

            //navigation Employer acts as relationship identifier
            public virtual Employer Employer { get; set; }
        }

        //**************************************************************************************************

        // enumerable list of days when the clinic is open
        public enum DaysAvailable { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }
        public class Availability
        {
            //availability ID
            [Key, Column(Order = 0)]
           
            public int avID { get; set; }

            //volunteer ID, foreign key
            [Key, Column(Order = 1)]
            public int volID { get; set; }

            //days
            [Required]
            public DaysAvailable avDay { get; set; }

            //from time
            [Required]
            [DataType(DataType.Time)]
            public DateTime avFrom { get; set; }

            //until time
            [Required]
            [DataType(DataType.Time)]
            public DateTime avUntil { get; set; }

            //active: boolean true - yes, false - no
            [Required]
            public bool avActive { get; set; }

            //navigation Volunteer property acts as relationship identifier
            public virtual Volunteer Volunteer { get; set; }
        }

        //****************************************************************************************************

        public class Specialty
        {
            //specialty id
            [Key]
           
            public int spcID { get; set; }

            //specialty name
            [StringLength(20)]
            [Required]
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$")]
            public string spcName { get; set; }

            //many volunteers can have the same specialty
            public virtual ICollection<Volunteer> Volunteers { get; set; }
        }

        //************************************************************************************************
        public class Speak
        {
            //language
            [Key, Column(Order = 0)]
            public int lngID { get; set; }

            //volunteer id
           
            [Key, Column(Order = 1)]
            public int volID { get; set; }

            //navigation Volunteer acts as relationship identifier
            public virtual Volunteer Volunteer { get; set; }

            //navigation language acts as relationship identifier
            public virtual Language Language { get; set; }
        }
        //*****************************************************************************************
        public class Language
        {
            // language id
            [Key]
            public int lngID { get; set; }

            //name
            [MaxLength(20)]
            public string lngName { get; set; }

            //many volunteers can speak the same language
            public virtual ICollection<Speak> Speaks { get; set; }
        }
        //****************************************************************************************
        public class License
        {
            // volunteer ID
            [Key, Column(Order = 1)]
         
            public int volID { get; set; }

            //license id
            [Key, Column(Order = 0)]
            public int lcNum { get; set; }

            // date
            [Required]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
            public DateTime lcDate { get; set; }

            // clear
            [Required]
            public bool lcClear { get; set; }

            //expiration date
            [Required]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
            public DateTime lcExpire { get; set; }

            //navigation Volunteer acts as relationship identifier
            public virtual Volunteer Volunteer { get; set; }
        }
        //**************************************************************************************************
        public class Pagroup
        {
            // id
            [Key]
            public int pgrID { get; set; }

            //name
            [Required]
            [MaxLength(50)]
            public string pgrName { get; set; }

            //officer name
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,25}$",
            ErrorMessage = "Numbers and special characters are not allowed in the first name.")]
            [StringLength(25)]
            [Display(Name = "Officer First Name")]
            public string pgrOfcFirstName { get; set; }

            //Last Name
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$",
            ErrorMessage = "Numbers and special characters are not allowed in the last name.")]
            [StringLength(30)]
            [Display(Name = "Officer Last Name")]
            public string pgrOfcLastName { get; set; }

            //phone
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Phone Number")]
            [Required(ErrorMessage = "Phone Number Required!")]
            [RegularExpression(@"^((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}$", ErrorMessage = "Entered phone format is not valid. Use (999)999-9999 format.")]
            [StringLength(15)]
            public string pgrPhone { get; set; }

            //street1
            [Required]
            [StringLength(30)]
            public string pgrStreet1 { get; set; }

            //street2
            [StringLength(30)]
            public string pgrStreet2 { get; set; }

            //city
            [Required]
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,25}$")]
            [StringLength(25)]
            public string pgrCity { get; set; }

            //state
            [Required]
            [RegularExpression(@"^[A-Z\s]{2}$")]
            [StringLength(2)]
            public string pgrState { get; set; }

            //zip
            [Required]
            [RegularExpression(@"^(\d{5})$")]
            [StringLength(5)]
            public string pgrZip { get; set; }

            //many contracts for one group
            public virtual ICollection<Contract> Contracts { get; set; }
        }
        //**************************************************************************************************
        public class Contract
        {
            //Contract number
            [Key, Column(Order = 0)]
            public int ctrNum { get; set; }

            // volunteer ID
            [Key, Column(Order = 1)]
           
            public int volID { get; set; }

            //PA Group ID
            [Key, Column(Order = 2)]
            public int pgrID { get; set; }

            //navigation Volunteer acts as relationship identifier
            public virtual Volunteer Volunteer { get; set; }

            //navigation Pagroup acts as relationship identifier
            public virtual Pagroup Pagroup { get; set; }

        }
        //*************************************************************************************************
    }
}