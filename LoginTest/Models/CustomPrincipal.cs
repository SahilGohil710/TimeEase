using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace LoginTest.Models
{
    public class CustomPrincipal : IPrincipal
    {
        public IIdentity Identity { get; set; }

        public CustomPrincipal(IIdentity identity)
        {
            this.Identity = identity;
        }

        public bool IsInRole(string role)
        {
            // Implement role checking logic here if needed
            return false;
        }
    }
}