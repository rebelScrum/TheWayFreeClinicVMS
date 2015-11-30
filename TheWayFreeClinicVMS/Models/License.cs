using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheWayFreeClinicVMS.Models
{
    public class License
    {
        // volunteer ID
        [Key, ForeignKey("Volunteer")]
        public int volID { get; set; }

        ////license id
        //[Key, Column(Order = 0)]
        //public int lcNum { get; set; }

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
}