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
        //job id
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int jobID { get; set; }

        //volunteer id
        public int volID { get; set; }

        //employer id
        public int empID { get; set; }

        //job title
        [Required]
        [MaxLength(30)]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$",
        ErrorMessage = "Numbers and special characters are not allowed in the job title")]
        [Display(Name = "Title")]
        public string jobTitle { get; set; }

        //job start date
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime jobStartDate { get; set; }

        //job end date
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime? jobEndDate { get; set; }

        //navigation Volunteer acts as relationship identifier
        public virtual Volunteer Volunteer { get; set; }

        //navigation Employer acts as relationship identifier
        public virtual Employer Employer { get; set; }
    }

}