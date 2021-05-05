using VPMS_Project.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VPMS_Project.Models
{
    public class CustomerModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Client Name")]
        public string name { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Contact Number")]
        public int contactNumber { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string address { get; set; }


        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string emailAddress { get; set; }

        public int projectCount { get; set; }




    }
}
