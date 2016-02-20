using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheWayFreeClinicVMS.Models
{
    public class License
    {
        
        //license id
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int licenceID { get; set; }
       
        // volunteer ID
        [ForeignKey("Volunteer")]
        public int volID { get; set; }

        //number/code
        public int lcNum { get; set; }

        // date
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime lcDate { get; set; }

        // clear
        [Required]
        [DefaultValue(true)]
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