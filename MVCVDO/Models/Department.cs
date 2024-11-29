using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCVDO.Models
{
    public class Department
    {
        public int D_Id { get; set; }

        [Required]
        public string DepartmentName { get; set; }

        [Required(ErrorMessage = "Select a status")]
        public string Status { get; set; }
    }
}