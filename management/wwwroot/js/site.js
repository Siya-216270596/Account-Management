// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).on('submit', '.ajax-form', function (e) {
    e.preventDefault(); // Prevent default form submission

    const $form = $(this); // Get the current form
    const actionUrl = $form.attr('action'); // Form action URL
    const method = $form.attr('method'); // Form HTTP method (GET or POST)
    const alertContainer = $form.data('alert-container'); // Custom alert container (optional)

    $.ajax({
        url: actionUrl,
        type: method || 'POST', // Default to POST if not specified
        data: $form.serialize(), // Serialize form data
        success: function (response) {
            if (response.success) {
                // Success alert
                if (alertContainer) {
                    $(`#${alertContainer}`).removeClass('d-none alert-danger').addClass('alert-success').text(response.message || 'Action completed successfully!');
                } else {
                    alert('Action completed successfully!'); // Fallback
                }

                // Optionally reset the form
                $form[0].reset();

                // Auto-hide the success alert
                if (alertContainer) {
                    setTimeout(function () {
                        $(`#${alertContainer}`).addClass('d-none');
                    }, 3000);
                }
            } else {
                // Failure alert
                if (alertContainer) {
                    $(`#${alertContainer}`).removeClass('d-none alert-success').addClass('alert-danger').text(response.message || 'An error occurred!');
                } else {
                    alert('An error occurred!'); // Fallback
                }
            }
        },
        error: function () {
            // General server error alert
            if (alertContainer) {
                $(`#${alertContainer}`).removeClass('d-none alert-success').addClass('alert-danger').text('An unexpected error occurred. Please try again.');
            } else {
                alert('An unexpected error occurred. Please try again.'); // Fallback
            }
        }
    });
});
