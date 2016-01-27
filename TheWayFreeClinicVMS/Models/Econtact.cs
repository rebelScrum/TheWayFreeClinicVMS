using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheWayFreeClinicVMS.Models
{
    public class Econtact
    {
        //emergency contact id
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ecID { get; set; }

        //volunteer ID
        [ForeignKey("Volunteer")]
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
}