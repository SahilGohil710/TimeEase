using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace LoginTest.Models
{
    public class CustomIdentity : IIdentity
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string EmailID { get; set; }
        public string SessionId { get; set; }
        public string AorE { get; set; }

        public CustomIdentity(FormsAuthenticationTicket ticket, string fullName, string sessionId, string AorE, string EmailID)
        {
            this.Name = ticket.Name;
            this.FullName = fullName;
            this.EmailID = EmailID;
            this.SessionId = sessionId;
            this.AorE = AorE;
        }

        public string AuthenticationType => "Forms";
        public bool IsAuthenticated => true; // Mark as authenticated
    }
}