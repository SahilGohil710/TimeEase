using LoginTest.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LoginTest.Controllers
{
    [CheckUserLogin]
    public class AddEmpController : Controller
    {
        // GET: AddEmp
        private readonly VSMS_AmeeShreeEntities _repository;
        public AddEmpController(VSMS_AmeeShreeEntities repository)
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
        public ActionResult Index_GridData(string FromDate, string ToDate, string AppicationStatus)
        {
            int start = Convert.ToInt32(Request["Start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string AppStatus = "2";
            TempData["fromDate"] = FromDate;
            TempData["toDate"] = ToDate;
            TempData["AppicationStatus"] = AppicationStatus;
            string fromDate = string.IsNullOrEmpty(FromDate) ? null : FromDate;
            string toDate = string.IsNullOrEmpty(ToDate) ? null : ToDate;

            switch (AppicationStatus.ToUpper().Trim())
            {
                case "RESIGNED":
                    AppStatus = "Y";
                    break;
                case "LIVE":
                    AppStatus = "N";
                    break;
                default:
                    AppStatus = "2"; // for 'ALL' or any other value
                    break;
            }
            int sortColumnIndex = int.Parse(Request["order[0][column]"]);
            string sortDirection = Request["order[0][dir]"];
            string[] columnMappings = new string[]
            {
                "UserID",
                "AorE",
                "FullName",
                "EmailID",
                "PhoneNo"
            };
            string sortColumnName = columnMappings[sortColumnIndex];

            CultureInfo provider = CultureInfo.InvariantCulture;
            //var identity = (System.Web.HttpContext.Current.User as VSMS.Presentation.Models.MyPrinciple).Identity as VSMS.Presentation.Models.MyIdentity;
            List<sp_AddEmpGridData_Result> EmpDetails = new List<sp_AddEmpGridData_Result>();

            EmpDetails = _repository.sp_AddEmpGridData(fromDate, toDate, AppStatus).ToList();

            if (fromDate == null || fromDate == "")
            {
                var COUNT = EmpDetails.Count();
                int totalRows = EmpDetails.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    string upperSearchValue = searchValue.ToUpper();
                    DateTime searchDate;
                    bool isDateSearch = DateTime.TryParse(searchValue, out searchDate);

                    EmpDetails = EmpDetails.Where(x =>
                        (x.FullName?.ToUpper().Contains(upperSearchValue) ?? false) ||
                        (x.Email?.ToUpper().Contains(upperSearchValue) ?? false) ||
                        (x.PhoneNo?.ToUpper().Contains(upperSearchValue) ?? false) ||
                        //(upperSearchValue.Contains("LIVE") && (x.isResignedYN.ToUpper() == "N")) ||
                        //(upperSearchValue.Contains("RESIGNED") && (x.isResignedYN.ToUpper() == "Y")) ||
                        //(upperSearchValue.Contains("ADMIN") && (x.AorE.ToUpper() == "A")) ||
                        //(upperSearchValue.Contains("EMP") && (x.AorE.ToUpper() == "E"))
                        (upperSearchValue.Contains("LIV") && (x.isResignedYN.ToUpper() == "LIVE")) ||
                        (upperSearchValue.Contains("RES") && (x.isResignedYN.ToUpper() == "RESIGNED")) ||
                        (upperSearchValue.Contains("ADM") && (x.AorE.ToUpper() == "ADMIN")) ||
                        (upperSearchValue.Contains("EMP") && (x.AorE.ToUpper() == "EMPLOYEE"))
                    ).ToList();
                }

                int totalRowsAfterFilter = EmpDetails.Count();
                var filteredData = EmpDetails.Skip(start).Take(length).ToList();

                // sorting
                if (!string.IsNullOrEmpty(sortColumnName) && !string.IsNullOrEmpty(sortDirection))
                {
                    var propertyInfo = typeof(sp_AddEmpGridData_Result).GetProperty(sortColumnName);
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
                    data = EmpDetails,
                    draw = Request["draw"],
                    recordsTotal = totalRows,
                    recordsFiltered = totalRowsAfterFilter
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var COUNT = EmpDetails.Count();
                int totalRows = EmpDetails.Count();
                if (fromDate != null && toDate != null && AppicationStatus != null && AppicationStatus == "Resigned")
                {
                    DateTime FDT = DateTime.ParseExact(fromDate, new string[] { "dd.MM.yyyy", "dd-MMM-yyyy", "dd-MM-yyyy", "dd/MM/yyyy" }, provider, DateTimeStyles.None);
                    DateTime TDT = DateTime.ParseExact(toDate, new string[] { "dd.MM.yyyy", "dd-MMM-yyyy", "dd-MM-yyyy", "dd/MM/yyyy" }, provider, DateTimeStyles.None);


                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        string upperSearchValue = searchValue.ToUpper();
                        DateTime searchDate;
                        bool isDateSearch = DateTime.TryParse(searchValue, out searchDate);

                        EmpDetails = EmpDetails.Where(x =>
                            (x.FullName?.ToUpper().Contains(upperSearchValue) ?? false) ||
                            (x.Email?.ToUpper().Contains(upperSearchValue) ?? false) ||
                            (x.PhoneNo?.ToUpper().Contains(upperSearchValue) ?? false) ||
                        (upperSearchValue.Contains("LIV") && (x.isResignedYN.ToUpper() == "LIVE")) ||
                        (upperSearchValue.Contains("RES") && (x.isResignedYN.ToUpper() == "RESIGNED")) ||
                        (upperSearchValue.Contains("ADM") && (x.AorE.ToUpper() == "ADMIN")) ||
                        (upperSearchValue.Contains("EMP") && (x.AorE.ToUpper() == "EMPLOYEE"))
                        ).ToList();
                    }
                    int totalRowsAfterFilter = EmpDetails.Count();
                    var filteredData = EmpDetails.Skip(start).Take(length).ToList();
                    // sorting
                    if (!string.IsNullOrEmpty(sortColumnName) && !string.IsNullOrEmpty(sortDirection))
                    {
                        var propertyInfo = typeof(sp_AddEmpGridData_Result).GetProperty(sortColumnName);
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
                        data = EmpDetails,
                        draw = Request["draw"],
                        recordsTotal = totalRows,
                        recordsFiltered = totalRowsAfterFilter
                    }, JsonRequestBehavior.AllowGet);
                }
                if (fromDate != null && toDate != null && AppicationStatus != null && AppicationStatus == "Live")
                {
                    DateTime FDT = DateTime.ParseExact(fromDate, new string[] { "dd.MM.yyyy", "dd-MMM-yyyy", "dd-MM-yyyy", "dd/MM/yyyy" }, provider, DateTimeStyles.None);
                    DateTime TDT = DateTime.ParseExact(toDate, new string[] { "dd.MM.yyyy", "dd-MMM-yyyy", "dd-MM-yyyy", "dd/MM/yyyy" }, provider, DateTimeStyles.None);

                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        string upperSearchValue = searchValue.ToUpper();
                        DateTime searchDate;
                        bool isDateSearch = DateTime.TryParse(searchValue, out searchDate);

                        EmpDetails = EmpDetails.Where(x =>
                            (x.FullName?.ToUpper().Contains(upperSearchValue) ?? false) ||
                            (x.Email?.ToUpper().Contains(upperSearchValue) ?? false) ||
                            (x.PhoneNo?.ToUpper().Contains(upperSearchValue) ?? false) ||
                        (upperSearchValue.Contains("LIV") && (x.isResignedYN.ToUpper() == "LIVE")) ||
                        (upperSearchValue.Contains("RES") && (x.isResignedYN.ToUpper() == "RESIGNED")) ||
                        (upperSearchValue.Contains("ADM") && (x.AorE.ToUpper() == "ADMIN")) ||
                        (upperSearchValue.Contains("EMP") && (x.AorE.ToUpper() == "EMPLOYEE"))
                        ).ToList();
                    }
                    int totalRowsAfterFilter = EmpDetails.Count();
                    var filteredData = EmpDetails.Skip(start).Take(length).ToList();
                    // sorting
                    if (!string.IsNullOrEmpty(sortColumnName) && !string.IsNullOrEmpty(sortDirection))
                    {
                        var propertyInfo = typeof(sp_AddEmpGridData_Result).GetProperty(sortColumnName);
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
                        data = EmpDetails,
                        draw = Request["draw"],
                        recordsTotal = totalRows,
                        recordsFiltered = totalRowsAfterFilter
                    }, JsonRequestBehavior.AllowGet);
                }
                if (fromDate != null && toDate != null && AppicationStatus != null && AppicationStatus == "All")
                {
                    DateTime FDT = DateTime.ParseExact(fromDate, new string[] { "dd.MM.yyyy", "dd-MMM-yyyy", "dd-MM-yyyy", "dd/MM/yyyy" }, provider, DateTimeStyles.None);
                    DateTime TDT = DateTime.ParseExact(toDate, new string[] { "dd.MM.yyyy", "dd-MMM-yyyy", "dd-MM-yyyy", "dd/MM/yyyy" }, provider, DateTimeStyles.None);

                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        string upperSearchValue = searchValue.ToUpper();
                        DateTime searchDate;
                        bool isDateSearch = DateTime.TryParse(searchValue, out searchDate);

                        EmpDetails = EmpDetails.Where(x =>
                            (x.FullName?.ToUpper().Contains(upperSearchValue) ?? false) ||
                            (x.Email?.ToUpper().Contains(upperSearchValue) ?? false) ||
                            (x.PhoneNo?.ToUpper().Contains(upperSearchValue) ?? false) ||
                        (upperSearchValue.Contains("LIV") && (x.isResignedYN.ToUpper() == "LIVE")) ||
                        (upperSearchValue.Contains("RES") && (x.isResignedYN.ToUpper() == "RESIGNED")) ||
                        (upperSearchValue.Contains("ADM") && (x.AorE.ToUpper() == "ADMIN")) ||
                        (upperSearchValue.Contains("EMP") && (x.AorE.ToUpper() == "EMPLOYEE"))
                        ).ToList();
                    }
                    int totalRowsAfterFilter = EmpDetails.Count();
                    var filteredData = EmpDetails.Skip(start).Take(length).ToList();
                    // sorting
                    if (!string.IsNullOrEmpty(sortColumnName) && !string.IsNullOrEmpty(sortDirection))
                    {
                        var propertyInfo = typeof(sp_AddEmpGridData_Result).GetProperty(sortColumnName);
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
                        data = EmpDetails,
                        draw = Request["draw"],
                        recordsTotal = totalRows,
                        recordsFiltered = totalRowsAfterFilter
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        string upperSearchValue = searchValue.ToUpper();
                        DateTime searchDate;
                        bool isDateSearch = DateTime.TryParse(searchValue, out searchDate);

                        EmpDetails = EmpDetails.Where(x =>
                            (x.FullName?.ToUpper().Contains(upperSearchValue) ?? false) ||
                            (x.Email?.ToUpper().Contains(upperSearchValue) ?? false) ||
                            (x.PhoneNo?.ToUpper().Contains(upperSearchValue) ?? false) ||
                        (upperSearchValue.Contains("LIV") && (x.isResignedYN.ToUpper() == "LIVE")) ||
                        (upperSearchValue.Contains("RES") && (x.isResignedYN.ToUpper() == "RESIGNED")) ||
                        (upperSearchValue.Contains("ADM") && (x.AorE.ToUpper() == "ADMIN")) ||
                        (upperSearchValue.Contains("EMP") && (x.AorE.ToUpper() == "EMPLOYEE"))
                        ).ToList();
                    }
                    int totalRowsAfterFilter = EmpDetails.Count();
                    var filteredData = EmpDetails.Skip(start).Take(length).ToList();
                    // sorting
                    if (!string.IsNullOrEmpty(sortColumnName) && !string.IsNullOrEmpty(sortDirection))
                    {
                        var propertyInfo = typeof(sp_AddEmpGridData_Result).GetProperty(sortColumnName);
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
                        data = EmpDetails,
                        draw = Request["draw"],
                        recordsTotal = totalRows,
                        recordsFiltered = totalRowsAfterFilter
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion

        #region CREATE
        public ActionResult Create()
        {
            TempData["ValidationErrors"] = "";

            return View();
        }
        [HttpPost]
        public ActionResult Create(AddEmpModel model)
        {
            string email = ViewBag.Logged_Email; string fullName = ViewBag.Logged_FullName; string sessionid = ViewBag.Logged_SessionId;
            string AorE = ViewBag.Logged_AorE;

            var LoginAuth = _repository.M_Login.Where(m => m.SessionId.Trim() == sessionid.Trim() && m.EmailId.ToUpper().Trim() == email.ToUpper().Trim() && m.LoggedIn == false).FirstOrDefault();
            if (LoginAuth != null)
            {
                return RedirectToAction("Login", "Home");
            }

            #region VALIDATIONS
            if (string.IsNullOrEmpty(model.FullName))
            {
                ModelState.AddModelError("FullName", "Full Name is required");
            }
            else if (model.FullName.Length > 100)
            {
                ModelState.AddModelError("FullName", "Full Name should not be greater than 100 characters");
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("Email", "Email-ID is required");
            }
            else if (model.Email.Length > 100)
            {
                ModelState.AddModelError("Email", "Email-ID should not be greater than 100 characters");
            }
            if (string.IsNullOrEmpty(model.PhoneNo))
            {
                ModelState.AddModelError("PhoneNo", "Phone No is required");
            }
            else if (model.PhoneNo.Length > 10)
            {
                ModelState.AddModelError("PhoneNo", "Phone No should not be greater than 10 digits");
            }
            if (string.IsNullOrEmpty(model.DOB))
            {
                ModelState.AddModelError("DOB", "DOB is required");
            }

            if (string.IsNullOrEmpty(model.AddressLine1))
            {
                ModelState.AddModelError("AddressLine1", "Address Line 1 is required");
            }
            else if (model.AddressLine1.Length > 150)
            {
                ModelState.AddModelError("AddressLine1", "Address Line 1 should not be greater than 150 characters");
            }

            if (string.IsNullOrEmpty(model.AddressLine2))
            {
                ModelState.AddModelError("AddressLine2", "Address Line 2 is required");
            }
            else if (model.AddressLine2.Length > 150)
            {
                ModelState.AddModelError("AddressLine2", "Address Line 2 should not be greater than 150 characters");
            }
            if (string.IsNullOrEmpty(model.EmergencyPhoneNo))
            {
                ModelState.AddModelError("EmergencyPhoneNo", "Emergency Phone No is required");
            }
            else if (model.EmergencyPhoneNo.Length > 10)
            {
                ModelState.AddModelError("EmergencyPhoneNo", "Emergency Phone No should not be greater than 10 digits");
            }
            if (string.IsNullOrEmpty(model.JobTitle))
            {
                ModelState.AddModelError("JobTitle", "Job Title is required");
            }
            else if (model.JobTitle.Length > 100)
            {
                ModelState.AddModelError("JobTitle", "Job Title should not be greater than 100 characters");
            }
            if (string.IsNullOrEmpty(model.WorkLocation))
            {
                ModelState.AddModelError("WorkLocation", "Work Location is required");
            }
            else if (model.WorkLocation.Length > 100)
            {
                ModelState.AddModelError("WorkLocation", "Work Location should not be greater than 100 characters");
            }

            if (string.IsNullOrEmpty(model.JoinDate))
            {
                ModelState.AddModelError("JoinDate", "Join Date is required");
            }
            //if (string.IsNullOrEmpty(model.Password))
            //{
            //    ModelState.AddModelError("Password", "Password is required");
            //}
            //else if (model.Password.Length > 30)
            //{
            //    ModelState.AddModelError("Password", "Password cannot be greater than 30 characters");
            //}
            #endregion

            if (ModelState.IsValid)
            {
                M_Users newUser = new M_Users
                {
                    AorE = model.AorE,
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNo = model.PhoneNo,
                    Emergency_PhoneNo = model.EmergencyPhoneNo,
                    Address_Line1 = model.AddressLine1,
                    Address_Line2 = model.AddressLine2,
                    DOB = Convert.ToDateTime(model.DOB),
                    JobTitle = model.JobTitle,
                    JoinDate = Convert.ToDateTime(model.JoinDate),
                    WorkLocation = model.WorkLocation,
                    Password = null,
                    isResignedYN = "N",
                    InsertedOn = DateTime.Now,
                    InsertedIP = GetIp()
                };
                _repository.M_Users.Add(newUser);
                _repository.SaveChanges();

                TempData["IsFailureMessage"] = false;
                TempData["Message"] = "Successfully Created.";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values
         .SelectMany(v => v.Errors)
         .Select(e => e.ErrorMessage)
         .ToList();

                ViewBag.ValidationErrors = errors;
                return View("Create"); // Ensure you return the view
            }
        }
        #endregion
        
        #region EDIT
        public ActionResult Edit(string Email)
        {

            M_Users m_Users = _repository.M_Users.FirstOrDefault(x => x.Email.ToUpper().Trim() == Email.ToUpper().Trim());
            if (m_Users == null)
            {
                return HttpNotFound();
            }
            Session["Emailfor_Edit"] = Email;
            ViewBag.FormattedDOB = m_Users.DOB?.ToString("dd-MMM-yyyy");
            return View(m_Users);
        }
        [HttpPost]
        public ActionResult Edit(M_Users model)
        {
            #region VALIDATIONS
            if (string.IsNullOrEmpty(model.FullName))
            {
                ModelState.AddModelError("FullName", "Full Name is required");
            }
            else if (model.FullName.Length > 100)
            {
                ModelState.AddModelError("FullName", "Full Name should not be greater than 100 characters");
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("Email", "Email-ID is required");
            }
            else if (model.Email.Length > 100)
            {
                ModelState.AddModelError("Email", "Email-ID should not be greater than 100 characters");
            }
            if (string.IsNullOrEmpty(model.PhoneNo))
            {
                ModelState.AddModelError("PhoneNo", "Phone No is required");
            }
            else if (model.PhoneNo.Length > 10)
            {
                ModelState.AddModelError("PhoneNo", "Phone No should not be greater than 10 digits");
            }
            if (string.IsNullOrEmpty(model.DOB.ToString()))
            {
                ModelState.AddModelError("DOB", "DOB is required");
            }

            if (string.IsNullOrEmpty(model.Address_Line1))
            {
                ModelState.AddModelError("Address_Line1", "Address Line 1 is required");
            }
            else if (model.Address_Line1.Length > 150)
            {
                ModelState.AddModelError("Address_Line1", "Address Line 1 should not be greater than 150 characters");
            }

            if (string.IsNullOrEmpty(model.Address_Line2))
            {
                ModelState.AddModelError("Address_Line2", "Address Line 2 is required");
            }
            else if (model.Address_Line2.Length > 150)
            {
                ModelState.AddModelError("Address_Line2", "Address Line 2 should not be greater than 150 characters");
            }
            if (string.IsNullOrEmpty(model.Emergency_PhoneNo))
            {
                ModelState.AddModelError("Emergency_PhoneNo", "Emergency Phone No is required");
            }
            else if (model.Emergency_PhoneNo.Length > 10)
            {
                ModelState.AddModelError("Emergency_PhoneNo", "Emergency Phone No should not be greater than 10 digits");
            }
            if (string.IsNullOrEmpty(model.JobTitle))
            {
                ModelState.AddModelError("JobTitle", "Job Title is required");
            }
            else if (model.JobTitle.Length > 100)
            {
                ModelState.AddModelError("JobTitle", "Job Title should not be greater than 100 characters");
            }
            if (string.IsNullOrEmpty(model.WorkLocation))
            {
                ModelState.AddModelError("WorkLocation", "Work Location is required");
            }
            else if (model.WorkLocation.Length > 100)
            {
                ModelState.AddModelError("WorkLocation", "Work Location should not be greater than 100 characters");
            }

            if (string.IsNullOrEmpty(model.JoinDate.ToString()))
            {
                ModelState.AddModelError("JoinDate", "Join Date is required");
            }
            //if (string.IsNullOrEmpty(model.Password))
            //{
            //    ModelState.AddModelError("Password", "Password is required");
            //}
            //else if (model.Password.Length > 30)
            //{
            //    ModelState.AddModelError("Password", "Password cannot be greater than 30 characters");
            //}
            #endregion

            if (ModelState.IsValid)
            {
                string unchangedEmailfor_Edit = Session["Emailfor_Edit"].ToString();
                var user = _repository.M_Users.FirstOrDefault(x => x.Email.ToUpper().Trim() == unchangedEmailfor_Edit.ToUpper().Trim());
                if (user != null)
                {
                    user.FullName = model.FullName;
                    user.Email = model.Email;
                    user.PhoneNo = model.PhoneNo;
                    user.DOB = model.DOB;
                    user.Address_Line1 = model.Address_Line1;
                    user.Address_Line2 = model.Address_Line2;
                    user.Emergency_PhoneNo = model.Emergency_PhoneNo;
                    user.JobTitle = model.JobTitle;
                    user.WorkLocation = model.WorkLocation;
                    user.JoinDate = model.JoinDate;
                    user.AorE = model.AorE;
                    _repository.SaveChanges();
                }

                TempData["IsFailureMessage"] = false;
                TempData["Message"] = "Edited Successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values
         .SelectMany(v => v.Errors)
         .Select(e => e.ErrorMessage)
         .ToList();

                ViewBag.ValidationErrors = errors;
                return View("Create"); // Ensure you return the view
            }
        }
        #endregion
    }
}