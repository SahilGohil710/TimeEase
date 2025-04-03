function updateAorE() {
    var checkbox = document.getElementById("cb3-8");
    var hiddenInput = document.getElementById("AorEHidden");
    hiddenInput.value = checkbox.checked ? "A" : "E";
    document.getElementById("AorEHidden").value = document.getElementById("cb3-8").checked ? "A" : "E";
}
//function updateAorE_forEdit() {
//    document.getElementById("AorEHidden").value = document.getElementById("cb3-8").checked ? "A" : "E";
//}


//document.getElementById('toggle-password').addEventListener('click', function () {
//    var passwordField = document.getElementById('password');
//    var icon = this.querySelector('i');

//    // Toggle the password visibility
//    if (passwordField.type === "password") {
//        passwordField.type = "text";
//        icon.classList.remove("fa-eye");
//        icon.classList.add("fa-eye-slash");
//    } else {
//        passwordField.type = "password";
//        icon.classList.remove("fa-eye-slash");
//        icon.classList.add("fa-eye");
//    }
//});

function validateJobTitle(input) {
    // Allow only alphabets, spaces, '&', and '-'
    input.value = input.value.replace(/[^A-Za-z\s&-]/g, '');

    // Capitalize first letter of each word
    input.value = input.value.replace(/\b\w/g, function (char) {
        return char.toUpperCase();
    });
}

function isNumeric(event) {
    let charCode = event.which ? event.which : event.keyCode;
    if (charCode < 48 || charCode > 57) {
        return false; // Prevent non-numeric input via keyboard
    }
    return true;
}

function validatePhoneLength(input) {
    input.value = input.value.replace(/\D/g, ""); // Remove non-numeric characters
    if (input.value.length > 10) {
        input.value = input.value.slice(0, 10); // Restrict to 10 digits
    }
}

function handlePhonePaste(event) {
    event.preventDefault();
    let pastedData = (event.clipboardData || window.clipboardData).getData("text");
    let numericData = pastedData.replace(/\D/g, "").slice(0, 10); // Remove non-numeric characters and limit to 10 digits
    event.target.value = numericData;
}

document.addEventListener("DOMContentLoaded", function () {
    //let phoneInput = document.getElementById("PhoneNo");
    //phoneInput.addEventListener("paste", function (e) {
    //    e.preventDefault();
    //    let pastedData = (e.clipboardData || window.clipboardData).getData("text");
    //    let numericData = pastedData.replace(/\D/g, ""); // Remove non-numeric characters

    //    if (numericData.length > 10) {
    //        numericData = numericData.slice(0, 10);
    //    }

    //    phoneInput.value = numericData;
    //});

    //let EmergencyPhoneNo = document.getElementById("EmergencyPhoneNo");
    //EmergencyPhoneNo.addEventListener("paste", function (e) {
    //    e.preventDefault();
    //    let pastedData = (e.clipboardData || window.clipboardData).getData("text");
    //    let numericData = pastedData.replace(/\D/g, ""); // Remove non-numeric characters

    //    if (numericData.length > 10) {
    //        numericData = numericData.slice(0, 10);
    //    }

    //    EmergencyPhoneNo.value = numericData;
    //});
    let phoneInput = document.getElementById("PhoneNo");
    let emergencyPhoneInput = document.getElementById("EmergencyPhoneNo");

    if (phoneInput) {
        phoneInput.addEventListener("paste", handlePhonePaste);
    }

    if (emergencyPhoneInput) {
        emergencyPhoneInput.addEventListener("paste", handlePhonePaste);
    }

    let input = document.getElementById("FullName");
    input.addEventListener("input", function () {
        this.value = this.value.replace(/\b\w/g, function (char) {
            return char.toUpperCase();
        });
    });

    let emailInput = document.getElementById("Email");
    emailInput.addEventListener("input", function () {
        let emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        if (!emailPattern.test(this.value)) {
            this.setCustomValidity("Please enter a valid email address.");
        } else {
            this.setCustomValidity("");
        }
    });
});

$(document).ready(function () {
    var errors = $("#validationErrors").val();

    try {
        errors = JSON.parse(errors);
    } catch (e) {
        errors = [];
        console.error("JSON Parsing Error:", e);
    }

    if (errors.length > 0) {
        var errorList = "";
        $.each(errors, function (index, value) {
            errorList += "<li>" + value + "</li>";
        });

        $("#errorList").html(errorList);
        $("#errorContainer").show();
    }

    var currentDate = new Date();
    var formattedDate = moment(currentDate).format("DD-MMM-YYYY"); // Format as DD-MMM-YYYY

    $("#joinDateID").datepicker({
        dateFormat: "dd-M-yy",
        changeMonth: true,
        changeYear: true,
        maxDate: currentDate, // Restrict selection beyond today
        autoclose: true,
        onSelect: function (dateText) {
            $(this).val(moment(dateText, "DD-MMM-YYYY").format("DD-MMM-YYYY")); // Format selected date
        }
    }).datepicker("setDate", currentDate); // Set default date to today

    $("#joinDateID").on("keydown paste", function (e) {
        e.preventDefault(); // Prevent manual typing and pasting
    });

    $("#joinDateID").val(formattedDate);

    var today = new Date();
    var minDOB = new Date();
    var maxDOB = new Date();

    minDOB.setFullYear(today.getFullYear() - 60); // Maximum age: 60 years old
    maxDOB.setFullYear(today.getFullYear() - 18); // Minimum age: 18 years old

    var today = new Date();
    var minDOB = new Date(today.getFullYear() - 60, today.getMonth(), today.getDate()); // Maximum age 60
    var maxDOB = new Date(today.getFullYear() - 18, today.getMonth(), today.getDate()); // Minimum age 18

    $("#dobID").datepicker({
        dateFormat: "dd-M-yy",
        changeMonth: true,
        changeYear: true,
        yearRange: minDOB.getFullYear() + ":" + maxDOB.getFullYear(), // Restrict year selection
        minDate: minDOB, // Prevent older than 60 years
        maxDate: maxDOB, // Prevent younger than 18 years
        autoclose: true
    }).on("keydown", function (e) {
        e.preventDefault(); // Prevent manual input via keyboard
    });

    $("#dobID").on("paste", function (e) {
        e.preventDefault(); // Prevent pasting text
    });

    $("#dobID").on("change", function () {
        var selectedDate = moment($(this).val(), "DD-MMM-YYYY").toDate();
        var minAllowedDate = new Date();
        var maxAllowedDate = new Date();
        minAllowedDate.setFullYear(today.getFullYear() - 60); // Maximum 60 years old
        maxAllowedDate.setFullYear(today.getFullYear() - 18); // Minimum 18 years old

        if (selectedDate > maxAllowedDate || selectedDate < minAllowedDate) {
            $("#dobError").text("Employee age must be between 18 and 60 years.");
            $(this).val(""); // Clear invalid selection
        } else {
            $("#dobError").text(""); // Clear error if valid
        }
    });

    $("#fromDateId").datepicker({
        dateFormat: "dd-M-yy",
        changeMonth: true,
        changeYear: true,
        maxDate: currentDate, // Restrict selection beyond today
        autoclose: true
    }).on('change', function () {
        var formattedDate = moment($(this).val(), "DD-MMM-YYYY").format("DD-MMM-YYYY");
        $(this).val(formattedDate);
    });

    $("#toDateId").datepicker({
        dateFormat: "dd-M-yy",
        changeMonth: true,
        changeYear: true,
        maxDate: currentDate, // Restrict selection beyond today
        autoclose: true
    }).on('change', function () {
        var formattedDate = moment($(this).val(), "DD-MMM-YYYY").format("DD-MMM-YYYY");
        $(this).val(formattedDate);
    });

    GetData(null, null, null);

    $("#btnshowId").click(function () {

        var fromDate = $("#fromDateId").val();
        var toDate = $("#toDateId").val();

        debugger;
        var fromDate = moment($('#fromDateId').val(), "DD-MMM-YYYY");
        var toDate = moment($('#toDateId').val(), "DD-MMM-YYYY");

        if (toDate.isBefore(fromDate)) {
            alertify.error("To date Should be greater than From Date.");
            // You can also clear the "To" date field if it's not valid:
            $("#fromDateId").val("");
            $("#toDateId").val("");
            return false;
        }

        else {
            $("#hiddenInput").val("S");
            //$('#tbl_AddEmpList').DataTable().destroy();

            $('#tbl_AddEmpList').DataTable().destroy();
            //table.destroy();
            var fromDate = $("#fromDateId").val();
            var toDate = $("#toDateId").val();
            var AppicationStatus = $("#AppicationStatus option:selected").val();
            GetData(fromDate, toDate, AppicationStatus);

        }

    });

    function GetData(fromDate, toDate, AppicationStatus) {
        var table = $('#tbl_AddEmpList').DataTable({
            "ajax": {
                "url": '../AddEmp/Index_GridData', // Your server-side data retrieval URL
                "type": 'POST',
                "datatype": "json",
                "data": function (d) {
                    d.fromDate = fromDate;
                    d.toDate = toDate;
                    d.AppicationStatus = AppicationStatus;
                }
            },
            "columns": [
                { title: "User-ID", data: "UserID" },
                 {
                     title: "Email ID",
                     "render": function (data, type, row) {
                         return "<a href=../AddEmp/Edit?Email=" + row.Email + "  id=" + row.Email + ">" + row.Email + "</a>"
                     }
                 },
                { title: "Admin/Employee", data: "AorE" },
                { title: "Full Name", data: "FullName" },
                { title: "Phone No", data: "PhoneNo" },
                { title: "Resignation Status", data: "isResignedYN" }
            ],
            "order": [[0, 'desc']],
            "bPaginate": true,
            "bFilter": true,
            "bInfo": true,
            "autoWidth": false,
            "bSort": true,
            "scrollX": true,
            "serverSide": true,
            "processing": true,
            "dom": 'Bfrtip',
            "language": {
                "processing": "Please wait...."
            },
            "initComplete": function (settings, json) {
                $(".dataTables_processing").css("background-color", "red");
                $(".dataTables_processing").css("color", "white");
            }
        });

        // Remove extra buttons
        table.buttons('.buttons-excel').remove();
        table.buttons('.buttons-copy').remove();
        table.buttons('.buttons-csv').remove();
        table.buttons('.buttons-pdf').remove();
        table.buttons('.buttons-print').remove();
    }

});