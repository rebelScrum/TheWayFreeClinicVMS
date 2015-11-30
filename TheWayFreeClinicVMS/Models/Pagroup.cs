using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TheWayFreeClinicVMS.Models
{
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
}