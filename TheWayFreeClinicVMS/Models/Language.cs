using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheWayFreeClinicVMS.Models
{
    public class Language
    {
        // language id
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int lngID { get; set; }

        //name
        [MaxLength(20)]
        public string lngName { get; set; }

        //many volunteers can speak the same language
        public virtual ICollection<Speak> Speaks { get; set; }
    }
}