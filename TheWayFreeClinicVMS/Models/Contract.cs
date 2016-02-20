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
        //Contract id
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int contrID { get; set; }

        //Contract number
        [MaxLength(30)]
        public string ctrNum { get; set; }

        // volunteer ID
        public int volID { get; set; }

        //PA Group ID
        public int pgrID { get; set; }

        //navigation Volunteer acts as relationship identifier
        public virtual Volunteer Volunteer { get; set; }

        //navigation Pagroup acts as relationship identifier
        public virtual Pagroup Pagroup { get; set; }

    }
}