using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVCVDO.Models
{
    public class Users
    {
        public int id { get; set; }

        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }

        [Compare("password", ErrorMessage = "Confirm Password doesn't match, Type Again!")]
        public string currentPassword { get; set; }

        public string usertype { get; set; }


    }
}