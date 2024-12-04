using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCVDO.Models
{
    public class Application
    {
        public int A_Id { get; set; }
        public int R_Id { get; set; }
        public string ApplicantName { get; set; }
        public string Gender { get; set; }
        public string Department { get; set; }
        public string Events { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Comments { get; set; }
        public string ApplicationStatus { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime ApplicationUpdateDate { get; set; }
        public int? UserId { get; set; }
        public string ApprovalComments { get; set; }

        public bool Dance { get; set; }
        public bool Song { get; set; }
        public bool Drawing { get; set; }
    }
}