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
    public class ApplyLeaveController : Controller
    {
        // GET: ApplyLeave
        private readonly VSMS_AmeeShreeEntities _repository;
        public ApplyLeaveController(VSMS_AmeeShreeEntities repository)
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

        public ActionResult Index()
        {
            string email = ViewBag.Logged_Email;
            string fullName = ViewBag.Logged_FullName;
            string sessionid = ViewBag.Logged_SessionId;
            string AorE = ViewBag.Logged_AorE;

            var LoginAuth = _repository.M_Login
                .FirstOrDefault(m => m.SessionId.Trim() == sessionid.Trim() &&
                                     m.EmailId.ToUpper().Trim() == email.ToUpper().Trim() &&
                                     m.LoggedIn == false);
            if (LoginAuth != null)
            {
                return RedirectToAction("Login", "Home");
            }

            ViewBag.LeaveTypes = _repository.M_LeaveType
           .Select(lt => new SelectListItem
           {
               Text = lt.LeaveType,
               Value = lt.LeaveType
           })
           .ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Index_GridData(int? month, int? year)
        {
            string email = ViewBag.Logged_Email;
            string fullName = ViewBag.Logged_FullName;
            string sessionid = ViewBag.Logged_SessionId;
            string AorE = ViewBag.Logged_AorE;

            var LoginAuth = _repository.M_Login
                .FirstOrDefault(m => m.SessionId.Trim() == sessionid.Trim() &&
                                     m.EmailId.ToUpper().Trim() == email.ToUpper().Trim() &&
                                     m.LoggedIn == false);
            if (LoginAuth != null)
            {
                return RedirectToAction("Login", "Home");
            }

            int userid = _repository.M_Users.Where(x => x.Email.ToUpper().Trim() == email.ToUpper().Trim()).Select(x => x.UserID).FirstOrDefault();

            int start = Convert.ToInt32(Request["Start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            if (month == null && year == null)
            {
                month = DateTime.Now.Month;
                year = DateTime.Now.Year;
            }

            TempData["month"] = month;
            TempData["year"] = year;

            int sortColumnIndex = int.Parse(Request["order[0][column]"]);
            string sortDirection = Request["order[0][dir]"];
            string[] columnMappings = new string[]
            {
                "LeaveType",
                "LeaveFrom",
                "LeaveTo",
                "InsertedOn",
                "LeavePostedOn",
                "LeaveAcceptedYN",
                "LeaveAcceptedOn"
            };
            string sortColumnName = columnMappings[sortColumnIndex];

            CultureInfo provider = CultureInfo.InvariantCulture;
            //var identity = (System.Web.HttpContext.Current.User as VSMS.Presentation.Models.MyPrinciple).Identity as VSMS.Presentation.Models.MyIdentity;
            List<sp_ApplyLeaveGridData_Result> EmpDetails = new List<sp_ApplyLeaveGridData_Result>();

            EmpDetails = _repository.sp_ApplyLeaveGridData(month, year, userid).ToList();


            var COUNT = EmpDetails.Count();
            int totalRows = EmpDetails.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                string upperSearchValue = searchValue.ToUpper();
                DateTime searchDate;
                bool isDateSearch = DateTime.TryParse(searchValue, out searchDate);

                EmpDetails = EmpDetails.Where(x =>
                    (x.LeaveType?.ToUpper().Contains(upperSearchValue) ?? false) ||
                    (x.LeaveFrom?.ToString("dd-MMM-yyyy").Contains(upperSearchValue) ?? false) ||
                    (x.LeaveTo?.ToString("dd-MMM-yyyy").Contains(upperSearchValue) ?? false) ||
                    (x.LeavePostedOn?.ToString("dd-MMM-yyyy").ToUpper().Contains(upperSearchValue) ?? false) ||
                    (x.LeavePostedOn?.ToString("dd-MMM-yyyy").ToUpper().Contains(upperSearchValue) ?? false) ||
                    (x.LeaveAcceptedYN?.ToUpper().Contains(upperSearchValue) ?? false) ||
                    (x.LeaveAcceptedOn?.ToString("dd-MMM-yyyy").ToUpper().Contains(upperSearchValue) ?? false)
                ).ToList();
            }


            int totalRowsAfterFilter = EmpDetails.Count();
            var filteredData = EmpDetails.Skip(start).Take(length).ToList();

            // sorting
            if (!string.IsNullOrEmpty(sortColumnName) && !string.IsNullOrEmpty(sortDirection))
            {
                var propertyInfo = typeof(sp_ApplyLeaveGridData_Result).GetProperty(sortColumnName);
                if (propertyInfo != null)
                {
                    EmpDetails = sortDirection.ToLower() == "asc"
                        ? EmpDetails.OrderBy(x => propertyInfo.GetValue(x, null)).ToList()
                        : EmpDetails.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();
                }
            }

            //paging
            EmpDetails = EmpDetails.Skip(start).Take(length).ToList();

            return Json(new
            {
                data = EmpDetails.Select(x => new
                {
                    x.LeaveType,
                    LeaveFrom = x.LeaveFrom?.ToString("yyyy-MM-dd"),  // Format date
                    LeaveTo = x.LeaveTo?.ToString("yyyy-MM-dd"),
                    LeavePostedOn = x.LeavePostedOn?.ToString("yyyy-MM-dd"),
                    LeaveAcceptedYN = x.LeaveAcceptedYN,
                    LeaveAcceptedOn = x.LeaveAcceptedOn?.ToString("yyyy-MM-dd")
                }),
                draw = Request["draw"],
                recordsTotal = totalRows,
                recordsFiltered = totalRowsAfterFilter
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Create()
        {
            string email = ViewBag.Logged_Email;
            string fullName = ViewBag.Logged_FullName;
            string sessionid = ViewBag.Logged_SessionId;
            string AorE = ViewBag.Logged_AorE;

            var LoginAuth = _repository.M_Login
                .FirstOrDefault(m => m.SessionId.Trim() == sessionid.Trim() &&
                                     m.EmailId.ToUpper().Trim() == email.ToUpper().Trim() &&
                                     m.LoggedIn == false);
            if (LoginAuth != null)
            {
                return RedirectToAction("Login", "Home");
            }

            ViewBag.LeaveTypes = _repository.M_LeaveType
                              .Select(lt => new SelectListItem
                              {
                                  Text = lt.LeaveType,
                                  Value = lt.LeaveType
                              })
                              .ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Create(ApplyleaveModel model)
        {
            string email = ViewBag.Logged_Email;
            string fullName = ViewBag.Logged_FullName;
            string sessionid = ViewBag.Logged_SessionId;
            string AorE = ViewBag.Logged_AorE;

            var LoginAuth = _repository.M_Login
                .FirstOrDefault(m => m.SessionId.Trim() == sessionid.Trim() &&
                                     m.EmailId.ToUpper().Trim() == email.ToUpper().Trim() &&
                                     m.LoggedIn == false);
            if (LoginAuth != null)
            {
                return RedirectToAction("Login", "Home");
            }


            if (string.IsNullOrEmpty(model.LeaveType))
            {
                ModelState.AddModelError("LeaveType", "Please select Leave Type");
            }
            if (string.IsNullOrEmpty(model.Reason))
            {
                ModelState.AddModelError("Reason", "Reason is required");
            }
            if (string.IsNullOrEmpty(model.FromDate.ToString()))
            {
                ModelState.AddModelError("FromDate", "Please select From Date");
            }
            if (string.IsNullOrEmpty(model.ToDate.ToString()))
            {
                ModelState.AddModelError("ToDate", "Please select To Date");
            }
            if (string.IsNullOrEmpty(model.LeaveDuration))
            {
                ModelState.AddModelError("LeaveDuration", "Please select Leave Duration");
            }

            if (ModelState.IsValid)
            {
                int UserId = _repository.M_Users.Where(x => x.Email.ToUpper().Trim() == email.ToUpper().Trim()).Select(x => x.UserID).FirstOrDefault();
                int leaveTypeID = _repository.M_LeaveType.Where(x => x.LeaveType.ToUpper().Trim() == model.LeaveType.ToUpper().Trim()).Select(x => x.LeaveID).FirstOrDefault();
                int fromMonth = model.FromDate.Month;
                int fromYear = model.FromDate.Year;
                string leaveDuration = "";

                if (model.LeaveDuration == "First Half")
                {
                    leaveDuration = "FH"; // Leave Without Pay
                }
                else if (model.LeaveDuration == "Second Half")
                {
                    leaveDuration = "SH";
                }
                else if (model.LeaveDuration == "Full Day")
                {
                    leaveDuration = "FD";
                }

                M_ApplyLeave leave = new M_ApplyLeave
                {
                    F_UserID = UserId,
                    F_LeaveTypeID = leaveTypeID,
                    LeaveFrom = model.FromDate,
                    LeaveTo = model.ToDate,
                    Month = fromMonth,
                    Year = fromYear,
                    LeaveFor = leaveDuration,
                    LeaveAcceptedYN = "N",
                    LeaveAcceptedOn = null,
                    LeaveAcceptedBy = null,
                    LeaveAcceptedIP = null,
                    InsertedOn = DateTime.Now,
                    InsertedBy = UserId.ToString(),
                    InsertedIP = GetIp()
                };

                //UpdateAttendance(model.FromDate, model.ToDate, UserId, model.LeaveDuration);

                _repository.M_ApplyLeave.Add(leave);
                _repository.SaveChanges();

                var user = _repository.M_Users.FirstOrDefault(x => x.UserID == UserId);
                // Get all head users with AorE == "A"
                var headEmails = _repository.M_Users
                                 .Where(x => x.AorE == "A")
                                 .Select(x => x.Email)
                                 .ToList();

                // Ensure there are recipients
                if (headEmails.Any())
                {
                    string subject = $"New Leave Request from {user.FullName}";

                    string message = $@"
                                    Dear Sir/Madam,<br/><br/>
                                    Employee <b>{user.FullName}</b> has applied for leave.<br/><br/>
                                    <b>Details:</b><br/>
                                    - <b>Leave Type:</b> {model.LeaveType}<br/>
                                    - <b>From:</b> {model.FromDate:dd MMM yyyy}<br/>
                                    - <b>To:</b> {model.ToDate:dd MMM yyyy}<br/>
                                    - <b>Duration:</b> {model.LeaveDuration}<br/>
                                    - <b>Reason:</b> {model.Reason}<br/><br/>
                                    
                                    Kindly review and take the necessary action.<br/><br/>
                                    
                                    Regards,<br/>
                                    {fullName}
                                ";

                    var emailService = new EmailService();

                    // Send email to multiple recipients
                    emailService.SendEmail(string.Join(",", headEmails), subject, message);
                }

                TempData["IsFailureMessage"] = false;
                TempData["Message"] = "Leave applied successfully. Pending approval from the head; it will reflect here once accepted.";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.LeaveTypes = _repository.M_LeaveType.Select(x => new SelectListItem
                {
                    Text = x.LeaveType,
                    Value = x.LeaveType
                }).ToList();

                var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                ViewBag.ValidationErrors = errors;
                return View("Create");
            }
        }
        //private void UpdateAttendance(DateTime fromDate, DateTime toDate, int userId, string leaveDuration)
        //{

        //        var attendanceRecords = _repository.M_Attendance
        //            .Where(x => x.F_UserID == userId && x.Date >= fromDate && x.Date <= toDate)
        //            .ToList();

        //        foreach (var record in attendanceRecords)
        //        {
        //            if (leaveDuration == "First Half")
        //            {
        //                record.FirstHalf = "LWP"; // Leave Without Pay
        //            }
        //            else if (leaveDuration == "Second Half")
        //            {
        //                record.SecondHalf = "LWP";
        //            }
        //            else if (leaveDuration == "Full Day")
        //            {
        //                record.FirstHalf = "LWP";
        //                record.SecondHalf = "LWP";
        //            }
        //        }

        //    _repository.SaveChanges();
        //}
    }
}