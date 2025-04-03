using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace LoginTest.Models
{
    public class CheckUserLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                if (ticket != null)
                {
                    // Deserialize the UserData field to extract user details
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var user = js.Deserialize<dynamic>(ticket.UserData);

                    // Create a custom identity and principal
                    var customIdentity = new CustomIdentity(ticket, user["FullName"], user["SessionId"], user["AorE"], user["EmailID"]);
                    var customPrincipal = new CustomPrincipal(customIdentity);

                    // Set the custom principal
                    HttpContext.Current.User = customPrincipal;

                    // Optionally set these details in the ViewBag for use in views
                    filterContext.Controller.ViewBag.Logged_Email = user["EmailID"];
                    filterContext.Controller.ViewBag.Logged_FullName = user["FullName"];
                    filterContext.Controller.ViewBag.Logged_SessionId = user["SessionId"];
                    filterContext.Controller.ViewBag.Logged_AorE = user["AorE"];
                }
            }

            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                HttpContext.Current.Session["SessionExpiredMessage"] = "Session expired. Please log in again.";
                filterContext.Result = new RedirectResult("/Home/Login");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}