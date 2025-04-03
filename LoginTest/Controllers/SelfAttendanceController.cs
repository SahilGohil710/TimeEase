using LoginTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoginTest.Controllers
{
    [CheckUserLogin]
    public class SelfAttendanceController : Controller
    {
        // GET: SelfAttendance
        private readonly VSMS_AmeeShreeEntities _repository;
        public SelfAttendanceController(VSMS_AmeeShreeEntities repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }
            _repository = repository;
        }
        public ActionResult Index(int? month, int? year)
        {
            string loggedInEmail = ViewBag.Logged_Email; string fullName = ViewBag.Logged_FullName; string sessionid = ViewBag.Logged_SessionId;
            string AorE = ViewBag.Logged_AorE;

            var LoginAuth = _repository.M_Login.Where(m => m.SessionId.Trim() == sessionid.Trim() && m.EmailId.ToUpper().Trim() == loggedInEmail.ToUpper().Trim() && m.LoggedIn == false).FirstOrDefault();
            if (LoginAuth != null)
            {
                return RedirectToAction("Login", "Home");
            }

            DateTime today = DateTime.Today;

            // Set default values if not provided
            if (!month.HasValue || !year.HasValue)
            {
                month = today.Month;
                year = today.Year;
            }

            DateTime selectedDate = new DateTime(year.Value, month.Value, 1);
            DateTime past12MonthsDate = today.AddMonths(-11); // Limit selection to past 12 months

            // Prevent selection of future months
            if (year.Value > today.Year || (year.Value == today.Year && month.Value > today.Month))
            {
                TempData["ErrorMessage"] = "You cannot select a future month.";
                return RedirectToAction("Index", new { month = today.Month, year = today.Year });
            }

            int daysInMonth = DateTime.DaysInMonth(year.Value, month.Value);

            List<SelfAttendanceModel> fullMonthData = new List<SelfAttendanceModel>();

            int userID = _repository.M_Users.Where(x => x.Email.ToUpper().Trim() == loggedInEmail.ToUpper().Trim()).Select(x => x.UserID).FirstOrDefault();

            // Create a connection to fetch attendance records
            var attendanceRecords = _repository.M_Attendance
                                    .Where(a => a.F_UserID == userID && a.Date.HasValue && a.Date.Value.Year == year.Value && a.Date.Value.Month == month.Value)
                                    .ToList();

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime currentDate = new DateTime(year.Value, month.Value, day);
                bool isWeekend = currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday;

                var record = attendanceRecords.FirstOrDefault(a => a.Date.Value.Date == currentDate.Date);

                fullMonthData.Add(new SelfAttendanceModel
                {
                    Date = currentDate,
                    Day = currentDate.DayOfWeek.ToString(),
                    TimeIn = record?.InTime,
                    TimeOut = record?.OutTime,
                    WorkHours = record?.WorkHours,
                    //FirstHalf = record != null ? record.FirstHalf : (isWeekend ? "WO" : "A"),
                    //SecondHalf = record != null ? record.SecondHalf : (isWeekend ? "WO" : "A")
                    FirstHalf = record?.FirstHalf,
                    SecondHalf = record?.SecondHalf
                });
            }

            ViewBag.SelectedMonth = month;
            ViewBag.SelectedYear = year;
            ViewBag.Past12MonthsDate = past12MonthsDate;
            int weeklyOff;
            double nonPayableDays;
            double salaryFor;
            int payableDays;
            double lwpDays;

            int totalDays = fullMonthData.Count;
            double presentDays = fullMonthData.Sum(d =>
                                 (d.FirstHalf?.ToUpper().Trim() == "P" && d.SecondHalf?.ToUpper().Trim() == "P") ? 1 :
                                 (d.FirstHalf?.ToUpper().Trim() == "P" || d.SecondHalf?.ToUpper().Trim() == "P") ? 0.5 : 0
                                );
            

            if (attendanceRecords.Count == 0)
            {
                weeklyOff = fullMonthData.Count(d => d.Date.DayOfWeek == DayOfWeek.Saturday || d.Date.DayOfWeek == DayOfWeek.Sunday);
                payableDays = (int)Math.Ceiling(presentDays) + weeklyOff;
                nonPayableDays = totalDays - weeklyOff;
                lwpDays = fullMonthData.Sum(d =>
                                    (d.FirstHalf?.ToUpper().Trim() == "LWP" && d.SecondHalf?.ToUpper().Trim() == "LWP") ? 1 :
                                    (d.FirstHalf?.ToUpper().Trim() == "LWP" || d.SecondHalf?.ToUpper().Trim() == "LWP") ? 0.5 : 0
                                );
                salaryFor = weeklyOff;
                
            }
            else
            {
                
                weeklyOff = fullMonthData.Count(d => d.FirstHalf?.ToUpper().Trim() == "WO" && d.SecondHalf?.ToUpper().Trim() == "WO");
                payableDays = (int)Math.Ceiling(presentDays) + weeklyOff;
                nonPayableDays = fullMonthData
                                .Where(d => d.Date.DayOfWeek != DayOfWeek.Saturday && d.Date.DayOfWeek != DayOfWeek.Sunday)
                                .Sum(d =>
                                    (d.FirstHalf?.ToUpper().Trim() == "A" && d.SecondHalf?.ToUpper().Trim() == "A") ? 1 :
                                    (d.FirstHalf?.ToUpper().Trim() == "A" || d.SecondHalf?.ToUpper().Trim() == "A") ? 0.5 : 0
                                );
                lwpDays = fullMonthData.Sum(d =>
                                    (d.FirstHalf?.ToUpper().Trim() == "LWP" && d.SecondHalf?.ToUpper().Trim() == "LWP") ? 1 :
                                    (d.FirstHalf?.ToUpper().Trim() == "LWP" || d.SecondHalf?.ToUpper().Trim() == "LWP") ? 0.5 : 0
                                );
                salaryFor = payableDays - (nonPayableDays + lwpDays);
            }

            // Summary calculations
           

            //int weeklyOff = fullMonthData.Count(d => d.Date.DayOfWeek == DayOfWeek.Saturday || d.Date.DayOfWeek == DayOfWeek.Sunday);
            //int weeklyOff = fullMonthData.Count(d => d.FirstHalf?.ToUpper().Trim() == "WO" && d.SecondHalf?.ToUpper().Trim() == "WO");
            //int lwpDays = fullMonthData.Count(d => d.FirstHalf?.ToUpper().Trim() == "LWP" || d.SecondHalf?.ToUpper().Trim() == "LWP");
            //double lwpDays = fullMonthData.Sum(d =>
            //    (d.FirstHalf?.ToUpper().Trim() == "LWP" && d.SecondHalf?.ToUpper().Trim() == "LWP") ? 1 :
            //    (d.FirstHalf?.ToUpper().Trim() == "LWP" || d.SecondHalf?.ToUpper().Trim() == "LWP") ? 0.5 : 0
            //);

            //double nonPayableDays = fullMonthData
            //    .Where(d => d.Date.DayOfWeek != DayOfWeek.Saturday && d.Date.DayOfWeek != DayOfWeek.Sunday)
            //    .Sum(d =>
            //        (d.FirstHalf?.ToUpper().Trim() == "A" && d.SecondHalf?.ToUpper().Trim() == "A") ? 1 :
            //        (d.FirstHalf?.ToUpper().Trim() == "A" || d.SecondHalf?.ToUpper().Trim() == "A") ? 0.5 : 0
            //    );
            //double salaryFor = payableDays - (nonPayableDays + lwpDays);

            // Pass values to ViewBag
            ViewBag.PresentDays = presentDays;
            ViewBag.WeeklyOff = weeklyOff;
            ViewBag.PayableDays = payableDays;
            ViewBag.LWP = lwpDays;
            ViewBag.NonPayableDays = nonPayableDays; // Can be decimal
            ViewBag.TotalDays = totalDays;
            ViewBag.SalaryFor = salaryFor;

            return View(fullMonthData);
        }
    }
}