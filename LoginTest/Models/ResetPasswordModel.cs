using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginTest.Models
{
    public class ResetPasswordModel
    {
        public string Token { get; set; }
        [DataType(DataType.Password)]
        public string ReType_Password { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}