$(document).ready(function () {
    var table = $('#tbl_LeaveDetails').DataTable({
        "ajax": {
            "url": '../LeaveApproval/Index_GridData',
            "type": 'POST',
            "datatype": "json"
        },
        "columns": [
            { title: "LeaveType", data: "LeaveType" },
            { title: "FullName", data: "FullName" },
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
                title: "Action",
                "data": null,
                "render": function (data, type, row) {
                    return `
                        <button class='btn btn-success approveBtn' data-id='${row.AutoCode}'>Approve</button>
                        <button class='btn btn-danger rejectBtn' data-id='${row.AutoCode}'>Reject</button>
                    `;
                },
                "orderable": false
            }
        ],
        "order": [[1, 'desc']],
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

    // Select/Deselect all checkboxes
    $('#selectAll').on('change', function () {
        $('.leaveCheckbox').prop('checked', this.checked);
    });

    // Approve button click event
    $('#tbl_LeaveDetails').on('click', '.approveBtn', function () {
        var leaveId = $(this).data('id');
        processLeaveRequest(leaveId, 'Approve');
    });

    // Reject button click event
    $('#tbl_LeaveDetails').on('click', '.rejectBtn', function () {
        var leaveId = $(this).data('id');
        processLeaveRequest(leaveId, 'Reject');
    });

    function processLeaveRequest(leaveId, action) {
        $.ajax({
            url: '../LeaveApproval/ProcessLeave',
            type: 'POST',
            data: { leaveId: leaveId, action: action },
            success: function (response) {
                alert(response.message);
                table.ajax.reload();
            },
            error: function () {
                alert('An error occurred. Please try again.');
            }
        });
    }

    function formatDate(data) {
        if (!data) return "";

        if (/^\d{4}-\d{2}-\d{2}$/.test(data)) {
            return moment(data, "YYYY-MM-DD").format("DD-MMM-YYYY");
        }

        let match = data.match(/\d+/);
        let timestamp = match ? parseInt(match[0]) : NaN;

        return isNaN(timestamp) ? "" : moment(timestamp).format("DD-MMM-YYYY");
    }
});
