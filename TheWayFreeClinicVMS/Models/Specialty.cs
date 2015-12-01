using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
}