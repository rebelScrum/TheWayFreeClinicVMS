using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheWayFreeClinicVMS.Models
{
    public class Speak
    {
        //language
        [Key, Column(Order = 0)]
        public int lngID { get; set; }

        //volunteer id

        [Key, Column(Order = 1)]
        public int volID { get; set; }

        //navigation Volunteer acts as relationship identifier
        public virtual Volunteer Volunteer { get; set; }

        //navigation language acts as relationship identifier
        public virtual Language Language { get; set; }
    }
}