function validatePassword(password) {
    const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,30}$/;
    return regex.test(password);
}

$(document).ready(function () {
    // Disable the submit button initially
    const $submitButton = $('.login-button');
    $submitButton.prop('disabled', true); // Disable the button at first

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

    $("#password, #retypePassword").on('input', function () {
        var password = $("#password").val();
        var retypePassword = $("#retypePassword").val();

        let isPasswordValid = validatePassword(password);
        let doPasswordsMatch = password === retypePassword && password !== "";

        // Show or hide validation messages
        if (!isPasswordValid) {
            $("#passwordMessage").hide();
            $("#pass_Validation").show();
        } else {
            $("#pass_Validation").hide();
        }

        if (doPasswordsMatch) {
            $("#passwordMessage").show();
            $("#passwordMismatch").hide();
        } else {
            $("#passwordMessage").hide();
            if (retypePassword !== "") {
                $("#passwordMismatch").show();
            } else {
                $("#passwordMismatch").hide();
            }
        }

        // Enable or disable the submit button based on validation
        if (isPasswordValid && doPasswordsMatch) {
            $submitButton.prop('disabled', false); // Enable the button
        } else {
            $submitButton.prop('disabled', true); // Disable the button
        }
    });

    // Prevent copy, paste, and cut
    $("#password, #retypePassword").on('copy paste cut', function (e) {
        e.preventDefault();
    });
});
