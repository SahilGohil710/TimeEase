function confirmNavigation() {
    //if (confirm("Are you sure you want to return to the login page?")) {
    window.location.href = "../Home/Login";
    //    return true; // Allow the navigation
    //} else {
    //    return false; // Cancel the navigation
    //}
}

//document.getElementById('saveAndContinue').addEventListener('click', function () {
//    document.querySelector('.loader-overlay').style.display = 'flex';
//});

function checkEmailDuplication(emailField) {
    const currentEmail = emailField.value.trim();
    const prevEmail = emailField.dataset.prevValue || "";

    // Exit if the email hasn't changed
    if (currentEmail === prevEmail) {
        return;
    }

    var regex = /^(?!\.)([a-zA-Z0-9._%+-]+)@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    var validationMessage = document.getElementById("emailValidationMessage");

    // If the input value doesn't match the email pattern
    if (!regex.test(currentEmail) && currentEmail !== "") {
        validationMessage.textContent = "Please enter a valid email address.";
        return;
    } else {
        validationMessage.textContent = ""; // Clear the message if valid
    }

    // Update the previous value
    emailField.dataset.prevValue = currentEmail;

    // Show loading overlay
    $(".loader-overlay").show();

    // Make an AJAX call to your C# action
    fetch("../Home/CheckDupEmail", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email: currentEmail }),
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Network response was not ok.");
            }
            return response.json();
        })
        .then(data => {
            $(".loader-overlay").hide();
            if (data.isDuplicate) {
                alertify.error("Email already exists.");
                emailField.value = ""; // Clear the email field
                emailField.dataset.prevValue = ""; // Reset the previous value
            }
        })
        .catch(error => {
            $(".loader-overlay").hide();
            console.error("Error checking email:", error);
            alertify.error("Unable to verify the email. Please try again later.");
        });
}

function checkPhoneNoDuplication(PhoneNoField) {
    const currentPhone = PhoneNoField.value.trim();
    const prevPhone = PhoneNoField.dataset.prevValue || "";

    // Exit if the email hasn't changed
    if (currentPhone === prevPhone) {
        return;
    }

    // Update the previous value
    PhoneNoField.dataset.prevValue = currentPhone;

    // Show loading overlay
    $(".loader-overlay").show();

    // Make an AJAX call to your C# action
    fetch("../Home/ChkDupPhoneNo", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ PhoneNo: currentPhone }),
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Network response was not ok.");
            }
            return response.json();
        })
        .then(data => {
            $(".loader-overlay").hide();
            if (data.isDuplicate) {
                alertify.error("Phone Number already exists.");
                PhoneNoField.value = ""; // Clear the Phone Number field
                PhoneNoField.dataset.prevValue = ""; // Reset the previous value
            }
        })
        .catch(error => {
            $(".loader-overlay").hide();
            console.error("Error checking Phone Number:", error);
            alertify.error("Unable to verify the Phone Number. Please try again later.");
        });
}


document.addEventListener('DOMContentLoaded', function () {

    document.getElementById('toggle-password').addEventListener('click', function () {
        var passwordField = document.getElementById('Password');
        var icon = this.querySelector('i');

        // Toggle the password visibility
        if (passwordField.type === "password") {
            passwordField.type = "text";
            icon.classList.remove("fa-eye");
            icon.classList.add("fa-eye-slash");
        } else {
            passwordField.type = "password";
            icon.classList.remove("fa-eye-slash");
            icon.classList.add("fa-eye");
        }
    });
});

function validateName(input) {
    // Regular expression to allow only alphabets (both upper and lower case) and spaces
    var regex = /^[a-zA-Z\s]*$/;

    // If input does not match the allowed characters, remove invalid characters
    if (!regex.test(input.value)) {
        input.value = input.value.replace(/[^a-zA-Z\s]/g, '');
    }
}

function convertToUpperCase(input) {
    input.value = input.value.toUpperCase(); // Convert input value to uppercase
}

function validateEmail(input) {
    // Regular expression for validating an email address
    var regex = /^(?!\.)([a-zA-Z0-9._%+-]+)@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    var validationMessage = document.getElementById("emailValidationMessage");

    // If the input value doesn't match the email pattern
    if (!regex.test(input.value) && input.value !== "") {
        validationMessage.textContent = "Please enter a valid email address.";
    } else {
        validationMessage.textContent = ""; // Clear the message if valid
    }
}

function validatePhoneNo(input) {
    // Remove non-numeric characters
    input.value = input.value.replace(/[^0-9]/g, '');

    // Enforce maxlength of 10
    if (input.value.length > 10) {
        input.value = input.value.slice(0, 10);
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const body = document.body; // Append glimmers to the body for fixed positioning
    for (let i = 0; i < 50; i++) {
        const glimmer = document.createElement('div');
        glimmer.className = 'glimmer';
        glimmer.style.top = `${Math.random() * 100}vh`; // Use viewport height
        glimmer.style.left = `${Math.random() * 100}vw`; // Use viewport width
        glimmer.style.animationDelay = `${Math.random() * 5}s`;
        body.appendChild(glimmer);
    }
});

$(document).ready(function () {
    $('.select2').select2();

    $("#Password").on('input', function () {
        var password = $(this).val();
        var errorMessage = '';

        // Regular expression for password validation
        var capitalLetter = /[A-Z]/;
        var smallLetter = /[a-z]/;
        var number = /[0-9]/;
        var specialChar = /[!@#$%^&*(),.?":{}|<>]/;

        // Check password criteria
        if (password.length < 6 || password.length > 30) {
            errorMessage = 'Password must be between 6 and 30 characters.';
        } else if (!capitalLetter.test(password)) {
            errorMessage = 'Password must contain at least one uppercase letter.';
        } else if (!smallLetter.test(password)) {
            errorMessage = 'Password must contain at least one lowercase letter.';
        } else if (!number.test(password)) {
            errorMessage = 'Password must contain at least one number.';
        } else if (!specialChar.test(password)) {
            errorMessage = 'Password must contain at least one special character.';
        }

        // Show or hide error message
        if (errorMessage) {
            $("#password-error").text(errorMessage).show();
        } else {
            $("#password-error").text('').hide();
        }
    });

    //$("#Email").on("input", function () {
    //$("#Email").on("change", function () {
    //    var email = $('#Email').val();
    //    var emailRegex = /^(?!.*\.\.)(?!.*\.$)(?!^\.)([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})$/;

    //    // Check if the email format is valid
    //    if (!emailRegex.test(email)) {
    //        $("#emailMsg").text("Please enter a valid email address.").show();
    //        return false;
    //    } else {
    //        $("#emailMsg").hide();

    //        // AJAX call to check for duplicate email
    //        $.ajax({
    //            //url: '@Url.Action("CheckDuplicateEmail", "YourController")', // Replace 'YourController' with your actual controller name
    //            url: "../Home/CheckDuplicateEmail",
    //            type: 'GET',
    //            data: { email: email },
    //            success: function (response) {
    //                if (response.isDuplicate) {
    //                    $("#emailDup").show();
    //                    //alertify.error("This email is already in use. Please enter a different email or log in.");
    //                } else {
    //                    $("#emailDup").hide();
    //                }
    //            },
    //            error: function () {
    //                //$("#emailDup").text("An error occurred while checking the email. Please try again.").show();
    //                alertify.error("An error occurred while checking the email. Please contact administrator.");
    //            }
    //        });
    //    }
    //});
});