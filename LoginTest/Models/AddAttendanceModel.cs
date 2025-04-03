using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginTest.Models
{
    public class AddAttendanceModel
    {
        public bool IsRed { get; set; }
        public bool IsGreen { get; set; }
        [Display(Name = "Uploading Remark")]
        public string ErrorRemark { get; set; }
        public string UserID { get; set; }
        public string AttendanceDate { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public decimal workHours { get; set; }
        public string firsthalf { get; set; }
        public string secondHalf { get; set; }
        public string insertedOn { get; set; }
        public string insertedIP { get; set; }
        public string insertedBy { get; set; }

    }
}