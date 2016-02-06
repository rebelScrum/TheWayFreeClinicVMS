using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheWayFreeClinicVMS.Models
{
    // enumerable list of days when the clinic is open
    public enum DaysAvailable { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }
    public class Availability
    {
        //availability ID
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int avID { get; set; }

        //volunteer ID, foreign key
        [ForeignKey("Volunteer")]
        public int volID { get; set; }

        //days
        [Required]
        [Display(Name = "Day")]
        public DaysAvailable avDay { get; set; }

        //from time
        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "From")]
        public DateTime avFrom { get; set; }

        //until time
        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Until")]
        public DateTime avUntil { get; set; }


        //navigation Volunteer property acts as relationship identifier
        public virtual Volunteer Volunteer { get; set; }
    }
}