using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginTest.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string userName { get; set; }
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        public string LoginType { get; set; }

        //public List<M_EscalationMatrix> lst_EscalationMatrix { get; set; }
        public LoginModel()
        {
            //lst_EscalationMatrix = new List<M_EscalationMatrix>();
        }
    }
}