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

        public int volID { get; set; }

        //number/code
        [Display(Name = "Number")]
        public int lcNum { get; set; }

        // date
        [Display(Name = "Start Date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime lcDate { get; set; }

        // clear
        [Display(Name = "Clear Record")]
        [Required]
        [DefaultValue(true)]
        public bool lcClear { get; set; }

        //expiration date
        [Display(Name = "Expiration Date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime lcExpire { get; set; }

        //navigation Volunteer acts as relationship identifier
        public virtual Volunteer Volunteer { get; set; }
    }
}