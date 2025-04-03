using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginTest.Models
{
    public class AddEmpModel
    {
        public string AorE { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Password { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string DOB { get; set; }
        public string EmergencyPhoneNo { get; set; }
        public string JobTitle { get; set; }
        public string JoinDate { get; set; }
        public string WorkLocation { get; set; }
    }
}