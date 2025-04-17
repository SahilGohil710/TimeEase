using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginTest.Models
{
    public class SalaryModel
    {
        public string UserName { get; set; }
        public string UserID { get; set; }
        public string Salary { get; set; }
        public string Salary_InWords { get; set; }
        public bool Increment_ByPerc { get; set; }
        public double? IncrementPercent { get; set; }

    }
}