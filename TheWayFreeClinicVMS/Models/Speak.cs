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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int speakID{ get; set; }
        
        //language
       [ForeignKey("Language")]
       public int lngID { get; set; }

        //volunteer id

        [ForeignKey("Volunteer")]
        public int volID { get; set; }

        //navigation Volunteer acts as relationship identifier
        public virtual Volunteer Volunteer { get; set; }

        //navigation language acts as relationship identifier
        public virtual Language Language { get; set; }
    }
}