
$(document).ready(function () {
    var table = $('#tbl_UserDetails').DataTable({
        "ajax": {
            "url": '../Users/UserDetails',
            "type": 'POST',
            "datatype": "json",
            "data": function (d) {
                // Custom data logic (if any) can be added here
            }
        },
        "columns": [
            {
                title: "Email-ID",
                "render": function (data, type, row) {
                    return "<a href=../Users/Edit_UserDetails?Email=" + row.Email + "  id=" + row.Email + ">" + row.Email + "</a>"
                }
            },
            { title: "Company Name", data: "CompanyName" },
            { title: "Full Name", data: "FullName" },
            { title: "Mobile No.", data: "PhoneNo" },
            { title: "Designation", data: "Designation" },
            { title: "Owner/Employee", data: "OorE" }
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
        "initComplete": function (settings, json) {
            $(".dataTables_processing").css("background-color", "#007BFF"); // Blue
            $(".dataTables_processing").css("color", "#FFFFFF"); // White
        }
    });
});
