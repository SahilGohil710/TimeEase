using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginTest.Models
{
    public class SelfAttendanceModel
    {
        public DateTime Date { get; set; }
        public string Day { get; set; }
        public TimeSpan? TimeIn { get; set; }
        public TimeSpan? TimeOut { get; set; }
        public decimal? WorkHours { get; set; }
        public string FirstHalf { get; set; }
        public string SecondHalf { get; set; }
    }
}