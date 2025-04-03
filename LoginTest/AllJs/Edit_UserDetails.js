function deleteAttachment(DocumentTypeId) {
    //// Convert DocumentTypeId to an integer
    //var documentTypeIdNumber = parseInt(DocumentTypeId, 10);
    //alert(documentTypeIdNumber);

    //// Check if the conversion resulted in a valid number and if it is greater than 0
    //if (isNaN(documentTypeIdNumber) || documentTypeIdNumber <= 0) {
    //    alertify.alert("Invalid Document Type ID.");
    //    return false;
    //}

    alertify.confirm("Are you sure you want to delete this record?",
        function () {
            var emailValue = $("#Email").val();

            $(".loader-overlay").show();
            $.ajax({
                cache: false,
                url: "../Users/DeleteAttachmentInEdit_UserDetails",
                type: "GET",
                data: { "DocumentTypeId": DocumentTypeId, "Email": emailValue },
                success: function (data) {
                    $(".loader-overlay").hide();
                    if (data.success == false) {
                        alertify.error(data.message);
                        return false;
                    }

                    if (data.length > 0)
                        $("#tbl_attachmentList").show();
                    else
                        $("#tbl_attachmentList").hide();
                    $("#tbl_attachmentList tbody").html("");
                    var temp = "";
                    $.each(data, function (count, item) {
                        temp += "<tr>";
                        temp += "<td>" + item.FileName + "</td>";
                        temp += "<td><a style='cursor:pointer;' href='GetStaffDocument?FileName=" + item.FileName + "&Email=" + emailValue + "'><span class='fa fa-download'></span></a></td>";
                        temp += "<td><a style='cursor:pointer;' onclick='deleteAttachment(" + item.FileName + ");'><span class='fa fa-trash'></span></a></td>";
                        temp += "</tr>";
                    });
                    $("#tbl_attachmentList").append(temp);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $(".loader-overlay").hide();
                    alertify.error(thrownError);
                }
            });
        }).setHeader('<span> Delete Document </span> ');
}

//document.getElementById('toggle-password').addEventListener('click', function () {
//    var passwordField = document.getElementById('Password');
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

$(document).ready(function () {
    $(".loader-overlay").show();
    var emailValue = $("#Email").val();

    $("#btn_staffAttachment").click(function () {
        var documentType = $("#F_DocumentType").val();
        var documentName = $("#F_DocumentType option:selected").text(); // Get selected document name
        var EmailID = $("#Email").val();

        if (documentType == null || documentType.trim() == "" || documentType == 0) {
            alertify.error("Please select Attachment Type.");
            return false;
        }

        var formData = new FormData($("#UsersForm")[0]);
        formData.append("DocumentName", documentName); // Pass document name to backend
        var totalFiles = document.getElementById("attachmentfile").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("attachmentfile").files[i];
            formData.append("achievementfile", file);
        }
        $(".loader-overlay").show();

        $.ajax({
            cache: false,
            url: "../Users/AddFilesInEdit_UserDetails",
            type: "POST",
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data) {
                $(".loader-overlay").hide();
                if (data.success === false) {
                    alertify.error(data.message);
                    return false;
                }

                if (data.length > 0) {
                    $("#tbl_UserDetails_attachments").show();
                } else {
                    $("#tbl_UserDetails_attachments").hide();
                }

                $("#attachmentfile").val('');
                $("#documentNo").val('');
                $("#F_DocumentType").val(null).trigger('change'); // Set to null and trigger change
                $("#NameAsPerDocument").val("");

                $("#tbl_UserDetails_attachments tbody").html("");
                var temp = "";
                $.each(data, function (count, item) {
                    temp += "<tr>";
                    temp += "<td>" + item.FileName + "</td>";
                    temp += "<td><i class='fa fa-file'></i></td>";
                    temp += "<td><a style='cursor:pointer;' onclick='deleteAttachment(\"" + item.FileName + "\");'><span class='fa fa-trash'></span></a></td>";
                    temp += "</tr>";
                });
                $("#tbl_UserDetails_attachments tbody").append(temp);
                $("#tbl_UserDetails_attachments").show();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $(".loader-overlay").hide();
                alertify.error(thrownError);
            }
        });
    });

    $.ajax({
        cache: false,
        url: "../Users/UserDetails_Attachments",
        type: "GET",
        data: { "Email": $("#Email").val() },
        success: function (data) {
            //$('#pleaseWaitDialog').modal('hide');
            $(".loader-overlay").hide();
            if (data.success == false) {
                alertify.error(message);
                return false;
            }
            if (data.length > 0)
                $("#tbl_UserDetails_attachments").show();
            else
                $("#tbl_UserDetails_attachments").hide();

            $("#attachmentfile").val('');
            $("#tbl_UserDetails_attachments tbody").html("");
            var temp = "";
            $.each(data, function (count, item) {
                temp += "<tr>";
                temp += "<td>" + item.AttachmentType + "</td>";
                temp += "<td><a style='cursor:pointer;' href='GetStaffDocument?AttachmentType=" + item.AttachmentType + "&Email=" + emailValue + "'><span class='fa fa-download'></span></a></td>";
                temp += "<td><a style='cursor:pointer;' onclick='deleteAttachment(\"" + item.AttachmentType + "\");'><span class='fa fa-trash'></span></a></td>";
                temp += "</tr>";
            });
            $("#tbl_UserDetails_attachments").append(temp);
            $("#tbl_UserDetails_attachments").show();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //$('#pleaseWaitDialog').modal('hide');
            $(".loader-overlay").hide();
            alertify.error(thrownError);
        }
    });
});