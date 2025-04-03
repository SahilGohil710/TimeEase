function validateDesignation(input) {
    // Allow only alphabets, numbers, and spaces
    input.value = input.value.replace(/[^a-zA-Z0-9\s]/g, '');
}

function validateAlphabetInput(input) {
    input.value = input.value.replace(/[^a-zA-Z\s]/g, ''); // Allow alphabets and spaces
}

function validateURL(input) {
    // Updated regex to be more inclusive of various URL formats
    const regex = /^(https?:\/\/)?(www\.)?[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)+([/?].*)?$/;
    return regex.test(input.value);
}

function validatePassword(password) {
    const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,30}$/;
    return regex.test(password);
}

function validateNumericInput(input) {
    input.value = input.value.replace(/[^0-9]/g, '');
}

function formatName(input) {
    // Capitalize the first letter and remove non-alphabet characters
    let value = input.value.replace(/[^a-zA-Z]/g, ''); // Remove non-alphabet characters
    value = value.charAt(0).toUpperCase() + value.slice(1).toLowerCase(); // Capitalize first letter
    input.value = value;
}

$(document).ready(function () {
    $('.select2').select2();

    $("#togglePassword").click(function () {
        var passwordField = $("#password");
        var passwordFieldType = passwordField.attr("type");

        if (passwordFieldType === "password") {
            passwordField.attr("type", "text");
            $(this).find("i").removeClass("fa-eye").addClass("fa-eye-slash");
        } else {
            passwordField.attr("type", "password");
            $(this).find("i").removeClass("fa-eye-slash").addClass("fa-eye");
        }
    });

    $("#toggleRePassword").click(function () {
        var passwordField = $("#retypePassword");
        var passwordFieldType = passwordField.attr("type");

        if (passwordFieldType === "password") {
            passwordField.attr("type", "text");
            $(this).find("i").removeClass("fa-eye").addClass("fa-eye-slash");
        } else {
            passwordField.attr("type", "password");
            $(this).find("i").removeClass("fa-eye-slash").addClass("fa-eye");
        }
    });

    $("#Designation").on("input", function () {
        validateDesignation(this);
    });
    $("#CompanyName").on("input", function () {
        validateDesignation(this);
    });

    //$("#Email").on("input", function () {
    $("#Email").on("change", function () {
        var email = $('#Email').val();
        var emailRegex = /^(?!.*\.\.)(?!.*\.$)(?!^\.)([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})$/;

        // Check if the email format is valid
        if (!emailRegex.test(email)) {
            $("#emailMsg").text("Please enter a valid email address.").show();
            return false;
        } else {
            $("#emailMsg").hide();

            // AJAX call to check for duplicate email
            $.ajax({
                //url: '@Url.Action("CheckDuplicateEmail", "YourController")', // Replace 'YourController' with your actual controller name
                url: "../Home/CheckDuplicateEmail",
                type: 'GET',
                data: { email: email },
                success: function (response) {
                    if (response.isDuplicate) {
                        $("#emailDup").show();
                        //alertify.error("This email is already in use. Please enter a different email or log in.");
                    } else {
                        $("#emailDup").hide();
                    }
                },
                error: function () {
                    //$("#emailDup").text("An error occurred while checking the email. Please try again.").show();
                    alertify.error("An error occurred while checking the email. Please contact administrator.");
                }
            });
        }
    });


    $("#CityName").on("input", function () {
        validateAlphabetInput(this);
    });

    $("#Website").on("input", function () {
        if (!validateURL(this)) {
            $("#websiteMsg").show();
        } else {
            $("#websiteMsg").hide();
        }
    });

    const numericFields = ['#PinCode', '#PhoneNumber', '#MobileNumber'];
    numericFields.forEach(field => {
        $(field).on('input', function () {
            validateNumericInput(this);
        });
    });

    const fields = ["#FirstName", "#LastName"];
    fields.forEach(field => {
        $(field).on("input", function () {
            formatName(this);
        });
    });

    //$('#DropDn_State').select2({
    //    theme: 'bootstrap4',
    //    placeholder: "Select"
    //});
    //$('#DropDn_BusinessType').select2({
    //    theme: 'bootstrap4',
    //    placeholder: "Select"
    //});

    const fields_forPassword = ["#password", "#retypePassword"];

    fields_forPassword.forEach(field => {
        $(field).on('input', function () {
            var password = $("#password").val();
            var retypePassword = $("#retypePassword").val();

            if (!validatePassword(password)) {
                $("#passwordMessage").hide();
                $("#pass_Validation").show();
                //$("#pass_Validation").show().text("Password must be 8-30 characters long, contain at least one uppercase letter, one lowercase letter, one number, one special character, and must not contain spaces..");
                return;
            }

            if (password === retypePassword && password !== "") {
                $("#passwordMessage").show();
                $("#passwordMismatch").hide();
                $("#pass_Validation").hide();
            } else {
                $("#passwordMessage").hide();
                if (retypePassword !== "") {
                    $("#passwordMismatch").show();
                    $("#pass_Validation").hide();
                } else {
                    $("#passwordMismatch").hide();
                    $("#pass_Validation").hide();
                }
            }
        });

        $(field).on('copy paste cut', function (e) {
            e.preventDefault();
        });
    });

});