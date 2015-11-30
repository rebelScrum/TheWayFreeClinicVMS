using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TheWayFreeClinicVMS.Models
{
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
}