
$(document).ready(function () {
    $("#clear").click(function () {
        location.reload();
    })

    $("#file").change(function () {
        if ($(this).find("#file").is(":disabled")) {
            return false;
        }
        var val = $(this).val();
        var file_type = val.substr(val.lastIndexOf('.')).toLowerCase();
        //check for double extension
        var ExtensionCount = (val.split('.').length - 1);
        if (ExtensionCount > 1) {
            alertify.error("File can't have double extension.");
            $('#file').val('');
            return false;
        }
        if (file_type != '.xls' && file_type != '.xlsx') {
            alertify.error("Attachment must be excel file.");
            $('#file').val('');
            return false;
        }
        var formData = new FormData($("#AttendanceBulkUpload")[0]);
        var totalFiles = document.getElementById("file").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("file").files[i];
            formData.append("file", file);
        }

        $(".loader-overlay").show();
        $.ajax({
            cache: false,
            url: "../AddAttendance/CreateAttendanceHashGrid",
            type: "POST",
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data) {
                //$('#pleaseWaitDialog').modal('hide');
                //alert(data.data);
                $(".loader-overlay").hide();
                if (data.success == false) {
                    $('#file').val('');
                    alertify.error(data.message);
                }
                else {
                    $("#totalAttendanceData").html(data.totalAttendanceData);
                    initializeDataTable();
                    $(".icn").show();
                    if (data.errCount > 0)
                        $("#importExcel").hide();
                    else
                        $("#importExcel").show();
                    $('input#file').prop('disabled', true);

                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $(".loader-overlay").hide();
                $('#file').val('');
                alertify.error(xhr.responseText);
            }
        });
    });


    $("#importExcel").click(function () {
        $("#importExcel").hide();
        //var formData = new FormData($("#OutsourceStaffform")[0]);
        var formData = new FormData($("#AttendanceBulkUpload")[0]);
        var totalFiles = document.getElementById("file").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("file").files[i];
            formData.append("file", file);
        }
        //$('#pleaseWaitDialog').modal();
        $(".loader-overlay").show();
        $.ajax({
            cache: false,
            url: "../AddAttendance/ImportedAttendanceFromExcel",
            type: "POST",
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data) {
                //$('#pleaseWaitDialog').modal('hide');
                $(".loader-overlay").hide();
                if (data.success == false) {
                    $("#emplhashGrid").html('');
                    $("#emplhashGrid").css({ "display": "block" });
                    alertify.error(data.message);
                    return false;
                }
                else {
                    $('#AttendanceBulkUploadTable').DataTable().destroy();
                    $(".icn").show();
                    $("#totalAttendanceData").html(data.totalAttendanceData);
                    initializeDataTable();
                    $('input#file').prop('disabled', false);
                    $("#file").val('');
                    //$("#emplhashGrid").html('');
                    //$("#emplhashGrid").html(data.html);
                    //$("#emplhashGrid").css({ "display": "block" });
                    alertify.success("Successfully uploaded.");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //$('#pleaseWaitDialog').modal('hide');
                $(".loader-overlay").hide();
                alertify.error(thrownError);
            }
        });
    })
    //initializeDataTable();
});

function initializeDataTable() {

    var table = $('#AttendanceBulkUploadTable').DataTable({
        "ajax": {
            "url": '../AddAttendance/LoadAttendanceHashGrid', // Your server-side data retrieval URL
            "type": 'POST',
            "datatyepe": "json",
        },
        "columns": [
              { title: "Uploading Remark", data: "ErrorRemark", className: "ErrorRemark-cell" }
              , { title: "User ID", data: "UserID" }
             , { title: "In Time", data: "InTime" }
             , { title: "Out Time", data: "OutTime" }
             , {
                 title: "Attendance Date", data: "AttendanceDate",
                 "render": function (data, type, row) {
                     // Format the date using a library like Moment.js
                     if (data) {
                         var formattedDate = moment(data).format('DD-MMM-yyyy');
                         return formattedDate;
                     }
                     return "";
                 }
             }
        ],
        "bPaginate": true,
        "bFilter": true,
        "bInfo": true,
        "autoWidth": false,
        "bSort": true,
        "scrollX": true,
        //"scrollCollapse": true,
        //"scrollY": '200px',
        "serverSide": true,
        "processing": true,
        "dom": 'Bfrtip',
        "language": {
            "processing": "Please wait...."
        },
        "columnDefs": [
            {
                "width": "100%",
                "targets": "0",
                "className": "text-right"
            },
            {
                "width": "2px",
                "targets": "1",
                "className": "text-right"
            }
        ],
        "createdRow": function (row, data, dataIndex) {
            var isRed = data.IsRed;
            var isGreen = data.IsGreen;

            if (isRed) {
                $(row).find('.ErrorRemark-cell').css('color', 'red');
            }
            else {
                $(row).find('.ErrorRemark-cell').css('color', 'green');
            }
        },
        buttons: [
        ]

    });
}