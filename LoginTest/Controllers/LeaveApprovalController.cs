using LoginTest.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoginTest.Controllers
{
    [CheckUserLogin]
    public class LeaveApprovalController : Controller
    {
        // GET: LeaveApproval
        private readonly VSMS_AmeeShreeEntities _repository;

        public LeaveApprovalController(VSMS_AmeeShreeEntities repository)
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

        #region INDEX
        public ActionResult Index()
        {
            string email = ViewBag.Logged_Email; string fullName = ViewBag.Logged_FullName; string sessionid = ViewBag.Logged_SessionId;
            string AorE = ViewBag.Logged_AorE;

            var LoginAuth = _repository.M_Login.Where(m => m.SessionId.Trim() == sessionid.Trim() && m.EmailId.ToUpper().Trim() == email.ToUpper().Trim() && m.LoggedIn == false).FirstOrDefault();
            if (LoginAuth != null)
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }
        [HttpPost]
        public ActionResult Index_GridData()
        {
            int start = Convert.ToInt32(Request["Start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];

            int sortColumnIndex = int.Parse(Request["order[0][column]"]);
            string sortDirection = Request["order[0][dir]"];
            string[] columnMappings = new string[]
            {
                "LeaveType",
                "FullName",
                "LeaveFrom",
                "LeaveTo"
            };
            string sortColumnName = columnMappings[sortColumnIndex];

            CultureInfo provider = CultureInfo.InvariantCulture;
            //var identity = (System.Web.HttpContext.Current.User as VSMS.Presentation.Models.MyPrinciple).Identity as VSMS.Presentation.Models.MyIdentity;
            List<sp_LeaveApprovalGridData_Result> LeaveDetails = new List<sp_LeaveApprovalGridData_Result>();

            LeaveDetails = _repository.sp_LeaveApprovalGridData().ToList();

            var COUNT = LeaveDetails.Count();
            int totalRows = LeaveDetails.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                string upperSearchValue = searchValue.ToUpper();
                DateTime searchDate;
                bool isDateSearch = DateTime.TryParse(searchValue, out searchDate);

                LeaveDetails = LeaveDetails.Where(x =>
                    (x.LeaveType?.ToUpper().Contains(upperSearchValue) ?? false) ||
                    (x.FullName?.ToUpper().Contains(upperSearchValue) ?? false) ||
                    (x.Email?.ToUpper().Contains(upperSearchValue) ?? false) ||
                    (x.LeaveFrom?.ToString("dd-MMM-yyyy").Contains(upperSearchValue) ?? false) ||
                    (x.LeaveTo?.ToString("dd-MMM-yyyy").Contains(upperSearchValue) ?? false)
                ).ToList();
            }

            int totalRowsAfterFilter = LeaveDetails.Count();
            var filteredData = LeaveDetails.Skip(start).Take(length).ToList();

            // sorting
            if (!string.IsNullOrEmpty(sortColumnName) && !string.IsNullOrEmpty(sortDirection))
            {
                var propertyInfo = typeof(sp_LeaveApprovalGridData_Result).GetProperty(sortColumnName);
                if (propertyInfo != null)
                {
                    LeaveDetails = sortDirection.ToLower() == "asc"
                        ? LeaveDetails.OrderBy(x => propertyInfo.GetValue(x, null)).ToList()
                        : LeaveDetails.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();
                }
            }

            //paging
            LeaveDetails = LeaveDetails.Skip(start).Take(length).ToList();

            return Json(new
            {
                //data = LeaveDetails,
                data = LeaveDetails.Select(x => new
                {
                    x.AutoCode,
                    x.LeaveType,
                    x.FullName,
                    LeaveFrom = x.LeaveFrom?.ToString("yyyy-MM-dd"),  // Format date
                    LeaveTo = x.LeaveTo?.ToString("yyyy-MM-dd"),
                    x.Email
                }),
                draw = Request["draw"],
                recordsTotal = totalRows,
                recordsFiltered = totalRowsAfterFilter
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpPost]
        public JsonResult ProcessLeave(int leaveId, string action) // int leaveId -> AutoCode, string action -> Approve/Reject
        {
            string email = ViewBag.Logged_Email;
            string fullName = ViewBag.Logged_FullName;
            string sessionid = ViewBag.Logged_SessionId;
            string AorE = ViewBag.Logged_AorE;

            var LoginAuth = _repository.M_Login.Where(m => m.SessionId.Trim() == sessionid.Trim() && m.EmailId.ToUpper().Trim() == email.ToUpper().Trim() && m.LoggedIn == false).FirstOrDefault();
            if (LoginAuth != null)
            {
                return Json(new { redirectTo = Url.Action("Login", "Home") });
            }

            try
            {
                bool isApproved = action == "Approve";
                var leaveDetails = _repository.M_ApplyLeave.Where(x => x.AutoCode == leaveId).FirstOrDefault();

                if (leaveDetails != null)
                {
                    if (isApproved)
                    {
                        leaveDetails.LeaveAcceptedYN = "Y";
                        leaveDetails.LeaveAcceptedOn = DateTime.Now;
                        leaveDetails.LeaveAcceptedBy = fullName;
                        leaveDetails.LeaveAcceptedIP = GetIp();

                        UpdateAttendance(leaveDetails.LeaveFrom, leaveDetails.LeaveTo, leaveDetails.F_UserID, leaveDetails.LeaveFor);
                    }
                    else
                    {
                        leaveDetails.LeaveAcceptedYN = "R";
                        leaveDetails.LeaveAcceptedOn = DateTime.Now;
                        leaveDetails.LeaveAcceptedBy = fullName;
                        leaveDetails.LeaveAcceptedIP = GetIp();
                    }
                    _repository.SaveChanges();

                    // Send Email Notification
                    var user = _repository.M_Users.FirstOrDefault(x => x.UserID == leaveDetails.F_UserID);
                    if (user != null)
                    {
                        string subject = $"Leave {action} Notification";
                        string message = $@"Dear {user.FullName},<br/><br/>
                                         Your leave request from {leaveDetails.LeaveFrom:dd MMM yyyy} to {leaveDetails.LeaveTo:dd MMM yyyy} has been {action.ToLower()}d.<br/><br/>
                                         Regards,<br/>{fullName}";

                        var emailService = new EmailService();
                        emailService.SendEmail(user.Email, subject, message);
                    }
                }

                TempData["IsFailureMessage"] = false;
                TempData["Message"] = "Leave " + action.ToLower() + "d successfully!";
                return Json(new { success = true, message = "Leave " + action.ToLower() + "d successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
        private void UpdateAttendance(DateTime? fromDate, DateTime? toDate, int? userId, string leaveDuration)
        {

            var attendanceRecords = _repository.M_Attendance
                .Where(x => x.F_UserID == userId && x.Date >= fromDate && x.Date <= toDate)
                .ToList();

            foreach (var record in attendanceRecords)
            {
                if (leaveDuration.ToUpper().Trim() == "FH")
                {
                    record.FirstHalf = "LWP";
                }
                else if (leaveDuration.ToUpper().Trim() == "SH")
                {
                    record.SecondHalf = "LWP";
                }
                else if (leaveDuration.ToUpper().Trim() == "FD")
                {
                    record.FirstHalf = "LWP";
                    record.SecondHalf = "LWP";
                }
            }

            _repository.SaveChanges();
        }

    }
}