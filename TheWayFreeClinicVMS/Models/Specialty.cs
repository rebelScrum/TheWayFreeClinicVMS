using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheWayFreeClinicVMS.Models
{
    public class Specialty
    {
        public Specialty()
        {
            Volunteers = new List<Volunteer>();
        }

        //specialty id

        [Display(Name = "Specialty")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int spcID { get; set; }

        //specialty name
        [StringLength(20)]
        [Required]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$",
         ErrorMessage = "Numbers and special characters are not allowed in the specialty name.")]
        [Display(Name = "Specialty Name")]
        public string spcName { get; set; }

        //many volunteers can have the same specialty
        public virtual ICollection<Volunteer> Volunteers { get; set; }
    }
}