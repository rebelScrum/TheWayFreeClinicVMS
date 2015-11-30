using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheWayFreeClinicVMS.Models
{
    public class Contract
    {
        //Contract number
        [Key, Column(Order = 0)]
        public int ctrNum { get; set; }

        // volunteer ID
        [Key, Column(Order = 1)]

        public int volID { get; set; }

        //PA Group ID
        [Key, Column(Order = 2)]
        public int pgrID { get; set; }

        //navigation Volunteer acts as relationship identifier
        public virtual Volunteer Volunteer { get; set; }

        //navigation Pagroup acts as relationship identifier
        public virtual Pagroup Pagroup { get; set; }

    }
}