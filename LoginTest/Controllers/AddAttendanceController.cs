using LoginTest.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace LoginTest.Controllers
{
    [CheckUserLogin]
    public class AddAttendanceController : Controller
    {
        // GET: AddAttendance
        private readonly VSMS_AmeeShreeEntities _repository;
        public AddAttendanceController(VSMS_AmeeShreeEntities repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }
            _repository = repository;
        }

        static void CountOccurrence(IDictionary<string, int> dictionary, string key)
        {
            if (!string.IsNullOrWhiteSpace(key) && key != "NA")
            {
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key]++;
                }
                else
                {
                    dictionary[key] = 1;
                }
            }
        }


        public ActionResult Index()
        {

            return View();
        }

        public ActionResult DSASampleFormatExcel()
        {
            // Set the license context for EPPlus 5+
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Create a new Excel package
            using (var package = new ExcelPackage())
            {
                // Add a new worksheet to the Excel package
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Define the column headers
                string[] columnHeaders = { "User ID", "Attendance Date", "In Time", "Out Time" };

                // Write the column headers to the first row of the worksheet
                for (int i = 0; i < columnHeaders.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = columnHeaders[i];
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                // Sample data for the second row
                worksheet.Cells[2, 1].Value = "1"; // User ID
                worksheet.Cells[2, 2].Value = DateTime.Parse("01-Mar-2024"); // Attendance Date
                worksheet.Cells[2, 3].Value = TimeSpan.FromHours(9.5);  // In Time (09:30)
                worksheet.Cells[2, 4].Value = TimeSpan.FromHours(18.5); // Out Time (18:30)

                // Apply date format for Attendance Date
                worksheet.Cells[2, 2, 100, 2].Style.Numberformat.Format = "dd-MMM-yyyy";

                // Apply time format for In Time and Out Time
                worksheet.Cells[2, 3, 100, 3].Style.Numberformat.Format = "hh:mm";
                worksheet.Cells[2, 4, 100, 4].Style.Numberformat.Format = "hh:mm";

                // Auto-fit columns for better readability
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save the Excel package to a memory stream
                MemoryStream stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                // Return the file for download
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Upload Attendance.xlsx");
            }
        }


        [HttpPost]
        public ActionResult CreateAttendanceHashGrid()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Add this line

            var file = Request.Files["file"];
            if (file.ContentLength > 0)
            {
                string[] sizes = { "B", "KB" };
                double len = file.ContentLength;
                int order = 0;
                while (len >= 1024 && ++order < sizes.Length)
                {
                    len = len / 1024;
                }
                if (len > 1000)
                    return Json(new { success = false, message = "File size should be up to 1 MB." }, JsonRequestBehavior.AllowGet);
            }
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    AddAttendanceModel ModelObj = new AddAttendanceModel();
                    IList<AddAttendanceModel> AttUpload_List = new List<AddAttendanceModel>();
                    IDictionary<string, int> AttDateCount = new Dictionary<string, int>();
                    HashSet<string> uniqueUserDatePairs = new HashSet<string>(); // Track unique (UserID, Attendance Date)

                    using (var stream = file.InputStream)
                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                            throw new Exception("No template found.");

                        var properties = new[]
                        {
                      "User ID"
                    , "Attendance Date"
                    , "In Time"
                    , "Out Time"
                };

                        bool checkHeader = false;
                        for (var i = 1; i <= properties.Length; i++)
                        {
                            if (worksheet.Cells[1, i].Value?.ToString().Trim() != properties[i - 1].Trim())
                            {
                                checkHeader = true;
                                break;
                            }
                        }
                        if (checkHeader)
                        {
                            throw new Exception("Please upload a valid template. Header is not in the correct format.");
                        }

                        int startRow = 2; // Data starts from row 2 (excluding the header)
                        int endRow = worksheet.Dimension.End.Row;

                        for (int row = startRow; row <= endRow; row++)
                        {
                            var userID = worksheet.Cells[row, 1].Value?.ToString().Trim();
                            var attDate = worksheet.Cells[row, 2].Value?.ToString().Trim();

                            if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(attDate))
                            {
                                continue; // Skip rows with missing UserID or Attendance Date
                            }

                            string uniqueKey = $"{userID}_{attDate}";

                            if (uniqueUserDatePairs.Contains(uniqueKey))
                            {
                                return Json(new { success = false, message = $"Duplicate entry found for User ID '{userID}' on Attendance Date '{attDate}'." }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                uniqueUserDatePairs.Add(uniqueKey);
                            }

                            CountOccurrence(AttDateCount, userID);
                        }
                        for (int row = startRow; row <= endRow; row++)
                        {

                            // Check if the row is empty
                            bool isRowEmpty = true;
                            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                            {
                                if (worksheet.Cells[row, col].Value != null &&
                                    !string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Value.ToString()))
                                {
                                    isRowEmpty = false;
                                    break;
                                }
                            }

                            // Skip empty rows
                            if (isRowEmpty)
                            {
                                continue;
                            }

                            // Read data from the current row
                            var rowData = new List<object>();
                            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                            {
                                rowData.Add(worksheet.Cells[row, col].Value);
                            }

                            // Process rowData (e.g., validate, store in database, etc.)
                            IList<AddAttendanceModel> processedRowData = ProcessBulkUploadExcelRow(rowData);

                            // Add the processed data to the main list
                            foreach (var item in processedRowData)
                            {
                                AttUpload_List.Add(item);
                            }
                        }
                    }

                    TempData["AttendanceUpload_TempList"] = AttUpload_List;

                    int totalErrorCount = 0;
                    foreach (var item in AttUpload_List)
                    {
                        if (item.IsRed)
                        {
                            totalErrorCount++;
                        }
                    }

                    if (AttUpload_List.Count == 0)
                        return Json(new { success = false, message = "Your Excel sheet has no data to display." }, JsonRequestBehavior.AllowGet);

                    bool errCount = false;
                    if (Convert.ToInt32(TempData["errCount"]) != 0)
                        errCount = true;

                    return Json(new { errCount = totalErrorCount, OutsourceSatffList = "Total : " + AttUpload_List.Count() }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = false, message = "Please upload a file." }, JsonRequestBehavior.AllowGet);
            }
        }

        public IList<AddAttendanceModel> ProcessBulkUploadExcelRow(List<object> Rowdata)
        {
            IList<AddAttendanceModel> attList = new List<AddAttendanceModel>();
            int iRow = 2;
            int errCount = 0;
            int Staffkey = 0;
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            var model = new AddAttendanceModel();
            model.IsRed = false;
            model.IsGreen = false;
            IList<string> MobileNoArr = new List<string>();
            IList<string> EmailIdArr = new List<string>();


            string userID = Rowdata[0]?.ToString()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(userID) || userID == "NA")
            {
                sb.Append("User ID is required,");
                model.IsRed = true;
                errCount++;
            }
            else
            {
                int parsedUserID;
                if (!int.TryParse(userID, out parsedUserID))
                {
                    sb.Append("Invalid User ID,");
                    model.IsRed = true;
                    errCount++;
                }

                var userE = _repository.M_Users.Where(x => x.UserID == parsedUserID).FirstOrDefault();
                if (userE == null)
                {
                    sb.Append("User ID not found in system,");
                    model.IsRed = true;
                    errCount++;
                }
            }

            string attDate = Rowdata[1]?.ToString()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(attDate))
            {
                sb.Append("Attendance Date is required,");
                model.IsRed = true;
                errCount++;
            }
            else
            {
                DateTime parsedDate;
                double excelDateNumber;

                // Trim and remove extra spaces
                attDate = attDate.Trim();

                // ✅ Remove time portion if present
                if (attDate.Contains(" "))
                {
                    attDate = attDate.Split(' ')[0]; // Keep only the date part
                }

                // ✅ Check if `attDate` is a numeric Excel serial date (common in Excel)
                if (double.TryParse(attDate, out excelDateNumber))
                {
                    parsedDate = DateTime.FromOADate(excelDateNumber); // Convert serial number to DateTime
                }
                else
                {
                    // ✅ Allow both "dd-MMM-yyyy" and "dd-MMM-yy" to handle Excel variations
                    if (!DateTime.TryParseExact(attDate, new[] { "dd-MMM-yyyy", "dd-MMM-yy" },
                                                CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                    {
                        sb.Append("Attendance Date must be in 'dd-MMM-yyyy' format (e.g., 01-Jan-2025),");
                        model.IsRed = true;
                        errCount++;
                    }
                }

                // ✅ Convert to "dd-MMM-yyyy" format after ensuring time is removed
                attDate = parsedDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
            }

            //string inTime = Rowdata[2]?.ToString()?.Trim() ?? string.Empty;
            //if (string.IsNullOrEmpty(inTime))
            //{
            //    //sb.Append("In Time is required,");
            //    //model.IsRed = true;
            //    //errCount++;
            //}
            //else
            //{
            //    // Validate that input contains only numbers and at most one decimal point
            //    if (!Regex.IsMatch(inTime, @"^\d+(\.\d+)?$"))
            //    {
            //        sb.Append("In Time must contain only numbers with at most one decimal point (e.g., 9.30 or 10.00),");
            //        model.IsRed = true;
            //        errCount++;
            //    }
            //    else
            //    {
            //        inTime = NormalizeTime(inTime);

            //        // Regex to validate time format as H:mm or HH:mm (e.g., 9:30 or 09:30)
            //        string timePattern = @"^\d{1,2}:\d{2}$";

            //        if (!Regex.IsMatch(inTime, timePattern))
            //        {
            //            sb.Append("In Time must be in H:mm or HH:mm format (e.g., 9:30 or 09:30) and contain only numbers with a colon (:),");
            //            model.IsRed = true;
            //            errCount++;
            //        }
            //    }

            //}

            //string outTime = Rowdata[3]?.ToString()?.Trim() ?? string.Empty;
            //if (string.IsNullOrEmpty(outTime))
            //{
            //    //sb.Append("Out Time is required,");
            //    //model.IsRed = true;
            //    //errCount++;
            //}
            //else
            //{
            //    // Validate that input contains only numbers and at most one decimal point
            //    if (!Regex.IsMatch(outTime, @"^\d+(\.\d+)?$"))
            //    {
            //        sb.Append("Out Time must contain only numbers with at most one decimal point (e.g., 9.30 or 10.00),");
            //        model.IsRed = true;
            //        errCount++;
            //    }
            //    else
            //    {
            //        outTime = NormalizeTime(outTime);

            //        // Regex to validate time format as H:mm or HH:mm (e.g., 9:30 or 09:30)
            //        string timePattern = @"^\d{1,2}:\d{2}$";

            //        if (!Regex.IsMatch(outTime, timePattern))
            //        {
            //            sb.Append("In Time must be in H:mm or HH:mm format (e.g., 9:30 or 09:30) and contain only numbers with a colon (:),");
            //            model.IsRed = true;
            //            errCount++;
            //        }
            //    }

            //}

            string inTime = Rowdata[2]?.ToString()?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(inTime))
            {
                double excelTime;
                if (double.TryParse(inTime, out excelTime))
                {
                    TimeSpan time = TimeSpan.FromDays(excelTime);
                    inTime = time.ToString(@"hh\:mm");
                }
                else
                {
                    sb.Append("Invalid In Time format,");
                    model.IsRed = true;
                    errCount++;
                }
            }

            string outTime = Rowdata[3]?.ToString()?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(outTime))
            {
                double excelTime;
                if (double.TryParse(outTime, out excelTime))
                {
                    TimeSpan time = TimeSpan.FromDays(excelTime);
                    outTime = time.ToString(@"hh\:mm");
                }
                else
                {
                    sb.Append("Invalid Out Time format,");
                    model.IsRed = true;
                    errCount++;
                }
            }



            model.UserID = userID == null ? null : userID.Trim();
            model.AttendanceDate = attDate == null ? null : attDate.Trim();
            model.InTime = inTime == null ? null : inTime.Trim();
            model.OutTime = outTime == null ? null : outTime.Trim();

            model.ErrorRemark = ((sb.Length > 0) ? sb.ToString() : "No Error Found.");
            if (model.ErrorRemark == "No Error Found.")
            {
                model.IsGreen = true;
            }
            attList.Add(model);
            iRow++;
            sb.Append("</ul>");
            TempData["errCount"] = errCount;
            return attList;
        }


        [HttpPost]
        public ActionResult ImportedAttendanceFromExcel()
        {
            var file = Request.Files["file"];
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    AddAttendanceModel ModelObj = new AddAttendanceModel();

                    var DSASatffList = SaveDatainDatabase();

                    TempData["AttendanceUpload_TempList"] = DSASatffList;
                    if (DSASatffList.Count() > 0)
                    {
                        return Json(new { success = true, IsError = false, errCount = false, DSASatffList = "Total : " + DSASatffList.Count() }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, IsError = false, errCount = false, DSASatffList = "Total : " + DSASatffList.Count() }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
                }
            }

            else
            {
                TempData["IsFailureMessage"] = true;
                TempData["Message"] = "Please upload tamplate.";
                return RedirectToAction("Index");
            }
        }

        string[] timeFormats = { "H:m", "HH:mm" }; // ✅ Remove dot-based formats
        string NormalizeTime(string time)
        {
            if (string.IsNullOrWhiteSpace(time)) return null;

            if (!time.Contains(":")) return time + ":00"; // Append ":00" if missing

            string[] parts = time.Split(':');
            if (parts.Length == 2)
            {
                string hours = parts[0].PadLeft(2, '0');   // Ensure two-digit hours
                string minutes = parts[1].PadRight(2, '0'); // Ensure two-digit minutes
                return $"{hours}:{minutes}";
            }
            return time;
        }

        public IList<AddAttendanceModel> SaveDatainDatabase()
        {
            int Counter = 1;
            string firstHalf;
            string secondHalf;
            IList<AddAttendanceModel> attList = new List<AddAttendanceModel>();
            var model1 = (List<AddAttendanceModel>)TempData["AttendanceUpload_TempList"];

            if (model1 != null)
            {
                foreach (var item in model1)
                {
                    try
                    {
                        // Convert AttendanceDate to DateTime (Extract only Date)
                        DateTime date = DateTime.ParseExact(item.AttendanceDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture).Date;

                        // Define possible formats
                        string inTimeStr = NormalizeTime(item.InTime);
                        string outTimeStr = NormalizeTime(item.OutTime);

                        TimeSpan? inTime = string.IsNullOrWhiteSpace(inTimeStr)
                            ? (TimeSpan?)null
                            : DateTime.ParseExact(inTimeStr, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None).TimeOfDay;

                        TimeSpan? outTime = string.IsNullOrWhiteSpace(outTimeStr)
                            ? (TimeSpan?)null
                            : DateTime.ParseExact(outTimeStr, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None).TimeOfDay;

                        // Fetch Shift Details from Database
                        var shiftDetails = _repository.M_Shift
                            .Where(s => s.ShiftID == 2)
                            .Select(s => new
                            {
                                ShiftStart = s.Start_Time,
                                ShiftEnd = s.End_Time,
                                LunchStart = s.LunchStart_Time,
                                LunchEnd = s.LunchEnd_Time
                            })
                            .FirstOrDefault();

                        // Convert Shift Times from Decimal? to TimeSpan
                        TimeSpan shiftStart = TimeSpan.FromMinutes((double)(shiftDetails?.ShiftStart ?? 0) * 60);
                        TimeSpan shiftEnd = TimeSpan.FromMinutes((double)(shiftDetails?.ShiftEnd ?? 0) * 60);
                        TimeSpan lunchStart = TimeSpan.FromMinutes((double)(shiftDetails?.LunchStart ?? 0) * 60);
                        TimeSpan lunchEnd = TimeSpan.FromMinutes((double)(shiftDetails?.LunchEnd ?? 0) * 60);

                        // Calculate WorkHours
                        decimal workHours = 0;
                        bool isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

                        if (inTime.HasValue && outTime.HasValue)
                        {
                            TimeSpan effectiveInTime = inTime.Value < shiftStart ? shiftStart : inTime.Value;
                            TimeSpan effectiveOutTime = outTime.Value > shiftEnd ? shiftEnd : outTime.Value;

                            int totalMinutes = (int)(effectiveOutTime - effectiveInTime).TotalMinutes;

                            // Correctly convert minutes to decimal hours
                            workHours = Math.Round(totalMinutes / 60.0M, 2);

                            // Extract whole number part
                            int wholeNumber = (int)Math.Truncate(workHours);
                            // Extract decimal part
                            decimal decimalPart = workHours - wholeNumber;
                            // Convert decimal part to an integer
                            int decimalAsInteger = (int)(decimalPart * 100);
                            // Only adjust if the decimal part is greater than 60
                            if (decimalAsInteger > 60)
                            {
                                int minutes = (int)(decimalPart * 60); // Convert to actual minutes
                                workHours = wholeNumber + (minutes / 100.0M); // Convert back to decimal
                            }
                            firstHalf = inTime.HasValue && inTime.Value > lunchStart ? "A" : "P";
                            secondHalf = outTime.HasValue && outTime.Value < lunchEnd ? "A" : "P";
                        }
                        else
                        {
                            if (isWeekend)
                            {
                                firstHalf = "WO"; secondHalf = "WO";
                            }
                            else 
                            {
                                firstHalf = "A"; secondHalf = "A";
                            }
                        }

                        // Set First Half & Second Half Attendance
                      

                        // Half-Day Calculation
                        //if (firstHalf == 'A' && secondHalf == 'A')
                        //    workHours = 0;
                        //else if (firstHalf == 'A' || secondHalf == 'A')
                        //    workHours = 0;

                        M_Attendance attendance = new M_Attendance
                        {
                            F_UserID = int.Parse(item.UserID),
                            Date = date,

                            //InTime = inTime.HasValue ? (DateTime?)date.Date.Add(inTime.Value) : null,
                            //OutTime = outTime.HasValue ? (DateTime?)date.Date.Add(outTime.Value) : null,
                            InTime = inTime,
                            OutTime = outTime,

                            WorkHours = workHours,
                            FirstHalf = firstHalf.ToString(),
                            SecondHalf = secondHalf.ToString(),

                            InsertedOn = DateTime.Now,
                            InsertedIP = "192.168.1.1",
                            InsertedBy = "Admin"
                        };


                        // Add to Database Context
                        _repository.M_Attendance.Add(attendance);

                        // Prepare Return List
                        attList.Add(new AddAttendanceModel
                        {
                            UserID = item.UserID,
                            AttendanceDate = item.AttendanceDate,
                            InTime = item.InTime,
                            OutTime = item.OutTime,
                            workHours = workHours
                        });

                        _repository.SaveChanges();
                        Counter++;
                    }
                    catch (DbEntityValidationException ve)
                    {
                        var error = ve.EntityValidationErrors.First().ValidationErrors.First();
                        var msg = $"Validation Error : {error.PropertyName} - {error.ErrorMessage}";
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(msg));
                        throw new Exception(msg);
                    }
                }
            }

            return attList;
        }
        [HttpPost]
        public ActionResult LoadAttendanceHashGrid()
        {
            int start = Convert.ToInt32(Request["Start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            //Find Order Column
            int sortColumnIndex = int.Parse(Request["order[0][column]"]);
            //string sortColumnName = Request[$"columns[{sortColumnIndex}][name]"];
            //string sortColumnName1 = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];
            string[] columnMappings = new string[]
            {
                "ErrorRemark"
               , "UserID"
               , "InTime"
               , "OutTime"
               , "AttendanceDate"
            };
            string sortColumnName = columnMappings[sortColumnIndex];

            IEnumerable<AddAttendanceModel> DSASatffList = new List<AddAttendanceModel>();
            DSASatffList = (IEnumerable<AddAttendanceModel>)TempData["AttendanceUpload_TempList"];
            TempData["AttendanceUpload_TempList"] = DSASatffList;
            int totalRows = DSASatffList.Count();
            //searching
            if (!string.IsNullOrEmpty(searchValue))
            {
                // Convert the search value to upper case for case-insensitive comparison
                var upperSearchValue = searchValue.ToUpper();

                DSASatffList = DSASatffList.Where(x =>
                                                       x.UserID.ToUpper().Contains(upperSearchValue) ||
                                                       x.InTime.ToUpper().Contains(upperSearchValue) ||
                                                       x.OutTime.ToUpper().Contains(upperSearchValue) ||
                                                       x.AttendanceDate.ToUpper().Contains(upperSearchValue)
                                                   ).ToList(); // || x.IsResigned == ISRESIGNED
            }

            int totalRowsAfterFilter = DSASatffList.Count();
            var filteredData = DSASatffList.Skip(start).Take(length).ToList();
            // sorting
            if (!string.IsNullOrEmpty(sortColumnName) && !string.IsNullOrEmpty(sortDirection))
            {
                var propertyInfo = typeof(AddAttendanceModel).GetProperty(sortColumnName);
                if (propertyInfo != null)
                {
                    DSASatffList = (sortDirection.ToLower() == "asc")
                        ? DSASatffList.OrderBy(x => propertyInfo.GetValue(x, null)).ToList()
                        : DSASatffList.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();
                }
            }

            //paging
            DSASatffList = DSASatffList.Skip(start).Take(length).ToList();


            return Json(new
            {
                data = DSASatffList,
                draw = Request["draw"],
                recordsTotal = totalRows,
                recordsFiltered = totalRowsAfterFilter
            }, JsonRequestBehavior.AllowGet);
        }
    }
}