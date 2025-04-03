document.addEventListener('DOMContentLoaded', function () {
    const container = document.querySelector('.container');
    for (let i = 0; i < 50; i++) {
        const glimmer = document.createElement('div');
        glimmer.className = 'glimmer';
        glimmer.style.top = `${Math.random() * 100}%`;
        glimmer.style.left = `${Math.random() * 100}%`;
        glimmer.style.animationDelay = `${Math.random() * 5}s`;
        container.appendChild(glimmer);
    }
});

$(document).ready(function () {
    // Hide the loader-overlay initially
    $(".loader-overlay").hide();

    $('#submitBtn').click(function () {
        var email = $('input[name="Email"]').val();
        if (email === '') {
            alertify.error('Please enter an email address.');
            return;
        }

        // Show the loader-overlay when the request starts
        $(".loader-overlay").show();

        $.ajax({
            url: "../Home/ForgotPassword",
            type: 'POST',
            data: { email: email },
            success: function (data) {
                if (data.success) {
                    alertify.success(data.message); // Show success message
                    // Optionally, redirect or clear the form
                } else {
                    alertify.error(data.message); // Show error message
                }
            },
            error: function () {
                alertify.error('An error occurred. Please try again.');
            },
            complete: function () {
                // Hide the loader-overlay after the request finishes
                $(".loader-overlay").hide();
            }
        });
    });

    $("#Email").on("input", function () {
        var email = $('#Email').val();
        var emailRegex = /^(?!.*\.\.)(?!.*\.$)(?!^\.)([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})$/;

        // Check if the email format is valid
        if (!emailRegex.test(email)) {
            $("#emailMsg").show();
            return false;
        } else {
            $("#emailMsg").hide();
        }
    });
});
