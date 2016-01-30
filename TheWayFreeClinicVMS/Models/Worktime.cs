using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheWayFreeClinicVMS.Models
{
    public class Worktime
    {
        //work log id
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int wrkID { get; set; }


        //Volunteer ID
        [ForeignKey("Volunteer")]
        public int volID { get; set; }

        //date
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime wrkDate { get; set; }

        //start time
        [Required]
        [DataType(DataType.Time)]
        public DateTime wrkStartTime { get; set; }

        //end time
        [DataType(DataType.Time)]
        public DateTime? wrkEndTime { get; set; }


        //navigation Volunteer property acts as relationship identifier
        public virtual Volunteer Volunteer { get; set; }
    }
}