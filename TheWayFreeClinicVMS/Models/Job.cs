using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheWayFreeClinicVMS.Models
{
    public class Job
    {
        //volunteer id
        [Key, ForeignKey("Volunteer"),Column(Order = 0)]

        public int volID { get; set; }

        //employer id
        [Key, ForeignKey("Employer"),Column(Order = 1)]
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

}