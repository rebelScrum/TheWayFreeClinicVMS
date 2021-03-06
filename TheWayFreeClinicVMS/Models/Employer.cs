﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheWayFreeClinicVMS.Models
{
    public class Employer
    {
        public Employer()
        {
            Jobs = new List<Job>();
        }
        //id
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int empID { get; set; }

        //employer name
        [Required]
        [MaxLength(30)]
        [MinLength(2)]
        [Display(Name = "Company")]
        public string empName { get; set; }

        //phone
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression(@"^((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}$", ErrorMessage = "Entered phone format is not valid. Use (999)999-9999 format.")]
        [StringLength(15)]
        public string empPhone { get; set; }

        //street1
        [Required]
        [StringLength(30)]
        [Display(Name = "Street 1")]
        public string empStreet1 { get; set; }

        //street2
        [StringLength(30)]
        [Display(Name = "Street 2")]
        public string empStreet2 { get; set; }

        //city
        [Required]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,25}$")]
        [StringLength(25)]
        [Display(Name = "City")]
        public string empCity { get; set; }

        //state
        [Required]
        [RegularExpression(@"^[A-Z\s]{2}$")]
        [StringLength(2)]
        [Display(Name = "State")]
        public string empState { get; set; }

        //zip
        [Required]
        [RegularExpression(@"^(\d{5})$")]
        [StringLength(5)]
        [Display(Name = "Zip")]
        public string empZip { get; set; }

        //navigation jobs acts as relationship identifier, many jobs for one employer
        public virtual ICollection<Job> Jobs { get; set; }
    }
}