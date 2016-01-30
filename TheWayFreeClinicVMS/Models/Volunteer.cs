using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheWayFreeClinicVMS.Models
{
    public class Volunteer
    {
        //ID
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public string volMiddleName { get; set; }

        //date of birthday
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Birth")]
        public DateTime volDOB { get; set; }

        //email
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(50)]
        [Display(Name = "Email")]
        public string volEmail { get; set; }

        //phone
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression(@"^((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}$", 
        ErrorMessage = "Entered phone format is not valid. Use (999)999-9999 format.")]
        [StringLength(15)]
        public string volPhone { get; set; }

        //street1
        [Required]
        [StringLength(30)]
        [Display(Name = "Street 1")]
        public string volStreet1 { get; set; }

        //street2
        [StringLength(30)]
        [Display(Name = "Street 2")]
        public string volStreet2 { get; set; }

        //city
        [Required]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,25}$",
        ErrorMessage = "Numbers and special characters are not allowed in the city name.")]
        [StringLength(25)]
        [Display(Name = "City")]
        public string volCity { get; set; }

        //state
        [Required]
        [RegularExpression(@"^[A-Z\s]{2}$",
        ErrorMessage = "Please enter two letter state abbreviation (FL).")]
        [StringLength(2)]
        [Display(Name = "State")]
        public string volState { get; set; }

        //zip
        [Required]
        [RegularExpression(@"^(\d{5})$",
        ErrorMessage = "Please enter 5 digits zip code.")]
        [StringLength(5)]
        [Display(Name = "Zip")]
        public string volZip { get; set; }

        //start date
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        
        public DateTime volStartDate { get; set; }

        //active: boolean true - yes, false - no
        [Required]
        [Display(Name = "Active")]
        [DefaultValue(true)]
        public bool volActive { get; set; }

        //specialty ID, foreign key from specialty look up table
        [ForeignKey("Specialty")]
        [Display(Name = "Specialty")]
        public int spcID { get; set; }

        // EF navigation relationships
        public virtual ICollection<Econtact> Econtact{ get; set; }

        public virtual ICollection<Speak> Speaks { get; set; }

        public virtual Specialty Specialty { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }

        public virtual ICollection<Availability> Available { get; set; }

        public virtual ICollection<Job> Jobs { get; set; }

        public virtual ICollection<License> License{ get; set; }

        public virtual ICollection<Worktime> Worklog { get; set; }
    }
}