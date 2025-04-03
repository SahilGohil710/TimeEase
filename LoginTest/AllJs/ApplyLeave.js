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

    // Get the first day of the previous month
    var firstDayOfPreviousMonth = new Date(currentDate.getFullYear(), currentDate.getMonth() - 1, 1);

    // Get the last day of the current month
    var lastDayOfCurrentMonth = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);

    $(".datepicker").datepicker({
        dateFormat: "dd-M-yy",
        changeMonth: true,
        changeYear: true,
        minDate: firstDayOfPreviousMonth, // Allow from start of previous month
        maxDate: lastDayOfCurrentMonth, // Allow up to the last day of the current month
        autoclose: true
    }).on('change', function () {
        var formattedDate = moment($(this).val(), "DD-MMM-YYYY").format("DD-MMM-YYYY");
        $(this).val(formattedDate);
    });

    // Get Data on Page Load
    GetData($("#monthDropdown").val(), $("#yearDropdown").val());

    $("#btnShow").click(function () {
        var month = $("#monthDropdown").val();
        var year = $("#yearDropdown").val();

        if (month && year) {
            GetData(month, year);
        } else {
            alert("Please select both month and year.");
        }
    });

    $("#btnshowId").click(function () {

        var fromDate = $("#fromDateId").val();
        var toDate = $("#toDateId").val();
        var month = $("#monthDropdown").val();
        var year = $("#yearDropdown").val();

        if (toDate && fromDate) {
            var fromDateMoment = moment(fromDate, "DD-MMM-YYYY");
            var toDateMoment = moment(toDate, "DD-MMM-YYYY");

            if (toDateMoment.isBefore(fromDateMoment)) {
                alertify.error("To date should be greater than From date.");
                $("#fromDateId").val("");
                $("#toDateId").val("");
                return false;
            }
        }

        $("#hiddenInput").val("S");

        $('#tbl_ApplyLeaveList').DataTable().destroy();

        GetData(month, year);
    });

    function GetData(month, year) {
        if ($.fn.DataTable.isDataTable('#tbl_ApplyLeaveList')) {
            $('#tbl_ApplyLeaveList').DataTable().destroy();  // Destroy the existing instance
            $('#tbl_ApplyLeaveList').empty();  // Clear previous data
        }

        var table = $('#tbl_ApplyLeaveList').DataTable({
            "ajax": {
                "url": '../ApplyLeave/Index_GridData', // Your server-side data retrieval URL
                "type": 'POST',
                "datatype": "json",
                "data": function (d) {
                    d.month = month;
                    d.year = year;
                }
            },
            "columns": [
                { title: "Leave Type", data: "LeaveType" },
                {
                    title: "Leave From",
                    data: "LeaveFrom",
                    "render": function (data) {
                        return formatDate(data);
                    }
                },
                {
                    title: "Leave To",
                    data: "LeaveTo",
                    "render": function (data) {
                        return formatDate(data);
                    }
                },
                {
                    title: "Leave Posted On",
                    data: "LeavePostedOn",
                    "render": function (data) {
                        return formatDate(data);
                    }
                },
                { title: "Leave Accepted YN", data: "LeaveAcceptedYN" },
                {
                    title: "Leave Accepted On",
                    data: "LeaveAcceptedOn",
                    "render": function (data) {
                        return formatDate(data);
                    }
                }
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

    // Function to handle date formatting safely
    function formatDate(data) {
        if (!data) return ""; // Handle null or empty values

        // Check if the data is in ISO format (YYYY-MM-DD)
        if (/^\d{4}-\d{2}-\d{2}$/.test(data)) {
            return moment(data, "YYYY-MM-DD").format("DD-MMM-YYYY");
        }

        // Check if the data is in /Date(1711929600000)/ format
        let match = data.match(/\d+/); // Extract timestamp
        let timestamp = match ? parseInt(match[0]) : NaN;

        return isNaN(timestamp) ? "" : moment(timestamp).format("DD-MMM-YYYY");
    }

});
