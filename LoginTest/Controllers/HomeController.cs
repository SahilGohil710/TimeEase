using LoginTest.Models;
using LoginTest.Services.CommonService;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace LoginTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly VSMS_AmeeShreeEntities _repository;

        public HomeController(VSMS_AmeeShreeEntities repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }
            _repository = repository;
        }

        public string GetIp()
        {
            string ipAddress = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
            }

            // If the IP address is "::1", it means the request is coming from localhost (IPv6).
            if (ipAddress == "::1")
            {
                ipAddress = "127.0.0.1";
            }

            return ipAddress;
        }
        [CheckUserLogin]
        public ActionResult Index()
        {
            string email = ViewBag.Logged_Email; string fullName = ViewBag.Logged_FullName; string compid = ViewBag.Logged_EmailID;
            string sessionid = ViewBag.Logged_SessionId; string AorE = ViewBag.Logged_AorE;

            var LoginAuth = _repository.M_Login.Where(m => m.SessionId.Trim() == sessionid.Trim() && m.EmailId.ToUpper().Trim() == email.ToUpper().Trim() && m.LoggedIn == false).FirstOrDefault();
            if (LoginAuth != null)
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        #region UTILITIES
        public JsonResult CheckDuplicateEmail(string email)
        {
            // Fetch the list of all emails from the database
            var chkdup_Email = _repository.M_Users.Select(x => x.Email).ToList();

            // Check if the provided email exists in the list
            bool isDuplicate = chkdup_Email.Contains(email);

            return Json(new { isDuplicate = isDuplicate }, JsonRequestBehavior.AllowGet);
        }
        private void ValidateRequiredField(string fieldValue, string fieldName, string displayName, int maxLength)
        {
            if (string.IsNullOrEmpty(fieldValue) || fieldValue.ToUpper() == "NA")
            {
                ModelState.AddModelError(fieldName, $"Please add {displayName}");
            }
            else if (fieldValue.Length > maxLength)
            {
                ModelState.AddModelError(fieldName, $"{displayName} should not be greater than {maxLength} characters");
            }
        }
        // Method to validate password strength
        private bool IsValidPassword(string password)
        {
            return password.Length >= 8 && password.Length <= 30 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch)) &&
                   !password.Contains(" ");
        }
        public string Decrypt(string id)
        {

            string cipherText = id;
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                encryptor.Padding = PaddingMode.PKCS7;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        private string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }


        [HttpPost]
        public JsonResult CheckDupEmail(string email)
        {
            string normalizedEmail = email.ToUpper().Trim();
            bool isDuplicate = _repository.M_Users.Any(x => x.Email.ToUpper().Trim() == normalizedEmail);
            return Json(new { isDuplicate = isDuplicate });
        }
        [HttpPost]
        public ActionResult ChkDupPhoneNo(string PhoneNo)
        {
            //long PhoneNumber;
            //if (!long.TryParse(PhoneNo, out PhoneNumber))
            //{
            //    return Json(new { isDuplicate = false, error = "Invalid phone number format." });
            //}

            bool isDuplicate = _repository.M_Users.Any(x => x.PhoneNo == PhoneNo);
            return Json(new { isDuplicate = isDuplicate });
        }

        #endregion

        #region LOGIN
        public ActionResult Login()
        {
            if (Session["SessionExpiredMessage"] != null)
            {
                ViewBag.SessionExpiredMessage = Session["SessionExpiredMessage"].ToString();
                Session.Remove("SessionExpiredMessage"); // Remove after displaying
            }

            return View();
        }
        [HttpPost]
        public ActionResult Login(string Email, string Password)
        {
            // Validate Email
            if (string.IsNullOrEmpty(Email))
            {
                TempData["validationMessage_forLogin"] = "Email is required.";
                return RedirectToAction("Login");
            }
            else if (Email.Length > 100)
            {
                TempData["validationMessage_forLogin"] = "Email cannot exceed 100 characters.";
                return RedirectToAction("Login");
            }

            // Validate Password
            if (string.IsNullOrEmpty(Password))
            {
                TempData["validationMessage_forLogin"] = "Password is required.";
                return RedirectToAction("Login");
            }
            else if (Password.Length > 30)
            {
                TempData["validationMessage_forLogin"] = "Password cannot exceed 30 characters.";
                return RedirectToAction("Login");
            }

            bool isEmailExist = _repository.M_Users.Any(x => x.Email.ToUpper().Trim() == Email.ToUpper().Trim());
            if (!isEmailExist)
            {
                TempData["validationMessage_forLogin"] = "Email not found.";
                return RedirectToAction("Login");
            }

            string isPwdExist = _repository.M_Users.Where(x => x.Email.ToUpper().Trim() == Email.ToUpper().Trim()).Select(x => x.Password).FirstOrDefault();
            if (isPwdExist == null)
            {
                TempData["validationMessage_forLogin"] = "Your password has not been set yet. Please click on 'Forgot Password' to create your password.";
                return RedirectToAction("Login");
            }

            // Hash the password
            Password = Encrypt(Password);

            // Check credentials
            var isSuccessLogin = _repository.M_Users
                .FirstOrDefault(x => x.Email.ToUpper().Trim() == Email.ToUpper().Trim()
                                     && x.Password == Password);
            var sessionId = Guid.NewGuid().ToString();  // Create a unique session ID

            if (isSuccessLogin != null)
            {
                // Serialize user data to store in the ticket's UserData
                _repository.Database.ExecuteSqlCommand("update [dbo].[M_Login] set  LoggedIn =0 where  LoggedIn =1 and EmailId = '" + Email + "' and SessionId != '" + sessionId + "'");
                var user = new
                {
                    UserCode = isSuccessLogin.Email,
                    AorE = isSuccessLogin.AorE,
                    FullName = isSuccessLogin.FullName,
                    SessionId = sessionId,
                    EmailID = isSuccessLogin.Email
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                string userData = js.Serialize(user);

                // Create FormsAuthenticationTicket with user data
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1, // Version
                    isSuccessLogin.Email, // Name (email in this case)
                    DateTime.Now, // Issue time
                    DateTime.Now.AddMinutes(30), // Expiration time
                    false, // Persistent
                    userData // User data (Provide an empty string if no custom data is needed)
                );


                // Encrypt the ticket
                string encTicket = FormsAuthentication.Encrypt(ticket);

                // Create authentication cookie
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket)
                {
                    HttpOnly = true, // Prevent access via JavaScript
                    Path = FormsAuthentication.FormsCookiePath, // Ensure the path matches
                    Domain = Request.Url.Host // Ensure the domain matches
                };

                // Add cookie to the response
                Response.Cookies.Add(authCookie);

                // Set the custom principal to the current context
                var customIdentity = new CustomIdentity(ticket, user.FullName, user.SessionId, user.AorE, user.EmailID);
                var customPrincipal = new CustomPrincipal(customIdentity);
                HttpContext.User = customPrincipal; // Ensure the custom principal is set correctly

                _repository.Database.ExecuteSqlCommand("delete from [dbo].[M_Login] where convert(datetime,LoginOn) < '" + DateTime.Now.Date.AddDays(-90).ToString("yyyy-MM-dd") + "'");
                M_Login logins = new M_Login();
                logins.EmailId = Email;
                logins.SessionId = sessionId;
                logins.LoggedIn = true;
                logins.LoginOn = DateTime.Now;
                logins.logOn_IP = GetIp();
                _repository.M_Login.Add(logins);
                _repository.SaveChanges();

                // Redirect to home page
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["validationMessage_forLogin"] = "Invalid Credentials.";
                string Current_IP = GetIp();

                // Record failed login attempt
                M_Login_failed logins = new M_Login_failed
                {
                    EmailID = Email.Trim(),
                    login_IP = Current_IP,
                    LogOn = DateTime.Now,
                    LogOn_Status = "Failed"
                };
                _repository.M_Login_failed.Add(logins);
                _repository.SaveChanges();

                return RedirectToAction("Login");
            }
        }
        #endregion

        #region LOGOUT
        [CheckUserLogin]
        public ActionResult Logout()
        {

            string Logged_userEmail = ViewBag.Logged_Email; string Logged_fullName = ViewBag.Logged_FullName;
            string Logged_sessionid = ViewBag.Logged_SessionId;

            // Clear authentication cookie
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName)
                {
                    Expires = DateTime.Now.AddDays(-1), // Expire the cookie
                    Value = null
                };
                Response.Cookies.Add(authCookie);
            }

            // Clear session
            Session.Clear();   // Removes all keys and values from the current session
            Session.Abandon(); // Ends the current session
            Session.RemoveAll();

            // Optional: Remove all custom cookies if any
            if (Request.Cookies != null)
            {
                foreach (var cookie in Request.Cookies.AllKeys)
                {
                    Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
                }
            }

            _repository.Database.ExecuteSqlCommand("update [dbo].[M_Login] set  LoggedIn =0 where  LoggedIn =1 and EmailId = '" + Logged_userEmail + "' and SessionId = '" + Logged_sessionid + "'");

            // Redirect to Login page or Home page
            return RedirectToAction("Login", "Home");
        }
        #endregion

        #region SIGNUP
        public ActionResult SignUp()
        {
            try
            {
                //TempData["SuccessResetMsg"] = "";
                return View();
            }
            catch (DbEntityValidationException ve)
            {
                TempData["IsFailureMessage"] = true;
                TempData["Message"] = "Sign-Up Error";
                // Update the values of the entity that failed to save
                var error = ve.EntityValidationErrors.First().ValidationErrors.First();
                var msg = String.Format("Validation Error : {0} - {1}",
                           error.PropertyName, error.ErrorMessage);
                var elmahException = new Exception(msg);
                Elmah.ErrorSignal.FromCurrentContext().Raise(elmahException);
                throw new Exception(msg);
            }

        }

        [HttpPost]
        public ActionResult SignUp(SignUpModel model)
        {
            #region VALIDATIONS
            if (string.IsNullOrEmpty(model.FullName))
            {
                ModelState.AddModelError("FullName", "Full Name is required.");
            }

            if (model.PhoneNo.ToString().Length < 10)
            {
                ModelState.AddModelError("PhoneNo", "Phone Number must be of 10 digits.");
            }
            if ((string.IsNullOrEmpty(model.Email)))
            {
                ModelState.AddModelError("Email", "Email-ID is required.");
            }
            if ((string.IsNullOrEmpty(model.Password)))
            {
                ModelState.AddModelError("Password", "Password is required.");
            }
            #endregion

            if (ModelState.IsValid)
            {
                try
                {
                    var insertM_Users = new M_Users
                    {
                        FullName = model.FullName.Trim(),
                        Email = model.Email.Trim(),
                        PhoneNo = model.PhoneNo,
                        Password = Encrypt(model.Password.Trim()),
                        InsertedIP = GetIp(),
                        InsertedOn = DateTime.Now
                    };
                    _repository.M_Users.Add(insertM_Users);
                    _repository.SaveChanges();

                    return RedirectToAction("Login", "Home");
                }
                catch (DbEntityValidationException ve)
                {
                    TempData["IsFailureMessage"] = true;
                    TempData["Message"] = "Sign Up POST Method";
                    // Update the values of the entity that failed to save
                    var error = ve.EntityValidationErrors.First().ValidationErrors.First();
                    var msg = String.Format("Validation Error : {0} - {1}", error.PropertyName, error.ErrorMessage);
                    var elmahException = new Exception(msg);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(elmahException);
                    throw new Exception(msg);
                }
            }
            else
            {
                return View(model);
            }
        }
        #endregion

        #region FORGOT PASSWORD
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ForgotPassword(string email)
        {
            const string emailPattern = @"^(?!.*\.\.)(?!.*\.$)(?!^\.)([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})$";
            var emailRegex = new System.Text.RegularExpressions.Regex(emailPattern);

            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Please enter Email-ID." });
            }

            if (!emailRegex.IsMatch(email))
            {
                return Json(new { success = false, message = "Please enter a valid email address." });
            }

            if (email.Length > 100)
            {
                return Json(new { success = false, message = "Email should not be greater than 100 characters." });
            }

            var user = _repository.M_Users.FirstOrDefault(x => x.Email.ToUpper().Trim() == email.ToUpper().Trim());
            if (user != null)
            {
                // Generate a unique token
                var token = Guid.NewGuid().ToString();

                // Store the token and expiration in the database
                user.ResetPasswordToken = token;
                user.TokenExpiration = DateTime.Now.AddMinutes(5); // Token valid for 2 minutes
                _repository.SaveChanges();

                // Send the email
                var resetLink = Url.Action("ResetPassword", "Home", new { token }, Request.Url.Scheme);
                var message = $@"
            Dear {user.FullName},
            <br/><br/>
            Please use the following link to reset your password: <a href='{resetLink}'>Reset Password</a>.
            <br/><br/>
            <b>This link will expire in 2 minutes.</b>
            <br/><br/>
            Kindly do not reply to this email, as it is machine-generated.";

                var emailService = new EmailService();
                emailService.SendEmail(user.Email, "Reset Your Password", message);

                return Json(new { success = true, message = "A reset link has been sent to your email." });
            }
            else
            {
                return Json(new { success = false, message = "Email-ID not found." });
            }
        }
        #endregion

        #region RESET PASSWORD
        public ActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Invalid reset token.";
                return RedirectToAction("ForgotPassword");
            }

            var user = _repository.M_Users.FirstOrDefault(x => x.ResetPasswordToken == token);
            if (user == null || user.TokenExpiration < DateTime.Now)
            {
                TempData["Error"] = "The reset link has expired. Please request a new one.";
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordModel { Token = token };
            return View(model);
        }
        [HttpPost]
        public ActionResult ConfirmResetPassword(string token, ResetPasswordModel model)
        {
            // Validate password fields
            if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.ReType_Password))
            {
                TempData["Error"] = "Password cannot be empty.";
                return View("ResetPassword");
            }

            // Validate that passwords match
            if (model.Password != model.ReType_Password)
            {
                TempData["Error"] = "Passwords do not match.";
                return View("ResetPassword");
            }

            // Validate password strength
            if (!IsValidPassword(model.Password))
            {
                TempData["Error"] = "Password must be 8-30 characters long, contain at least one uppercase letter, one lowercase letter, one number, one special character, and must not contain spaces.";
                return View("ResetPassword");
            }

            // Find user by token
            var user = _repository.M_Users.FirstOrDefault(x => x.ResetPasswordToken == token && x.TokenExpiration > DateTime.Now);

            if (user != null)
            {
                user.Password = Encrypt(model.ReType_Password); // Ensure you hash the password
                user.ResetPasswordToken = null;
                user.TokenExpiration = null;
                _repository.SaveChanges();

                TempData["SuccessResetMsg"] = "Your password has been successfully reset.";
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid or expired token.";
                return View("ResetPassword");
            }
        }
        #endregion
    }
}