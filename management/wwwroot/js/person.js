// person.js

// Document ready function
$(document).ready(function () {
    // Handle form submission for creating a person
    $('#personForm').on('submit', function (e) {
        e.preventDefault(); // Prevent default form submission

        const idNumber = $('#IdNumber').val().trim();
        const name = $('#Name').val().trim();
        const surname = $('#Surname').val().trim();
        const successAlert = $('#successAlert');
        const failureAlert = $('#failureAlert');

        // Reset alerts
        successAlert.hide();
        failureAlert.hide();

        // Validate South African ID number
        if (!validateSouthAfricanID(idNumber)) {
            failureAlert.text('Invalid South African ID number. Please check your input.')
                .removeClass('d-none').show();
            return false;
        }

        // Perform AJAX POST request
        $.ajax({
            url: '/Person/Create',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ IdNumber: idNumber, Name: name, Surname: surname }),
            success: function (response) {
                successAlert.text(response).removeClass('d-none').show();
            },
            error: function (xhr) {
                failureAlert.text(xhr.responseText).removeClass('d-none').show();
            }
        });
    });
});

// Function to validate South African ID number
function validateSouthAfricanID(idNumber) {
    // Check if the ID number is 13 digits
    if (idNumber.length !== 13 || !/^\d+$/.test(idNumber)) {
        return false;
    }

    // Validate the date of birth (first 6 digits YYMMDD)
    const birthDate = idNumber.substring(0, 6);
    if (!isValidDate(birthDate)) {
        return false;
    }

    // Validate checksum using Luhn algorithm
    const checksum = calculateLuhnChecksum(idNumber.substring(0, 12));
    return checksum === parseInt(idNumber[12]);
}

// Helper function: Validate date format
function isValidDate(birthDate) {
    const year = parseInt(birthDate.substring(0, 2), 10) + 1900;
    const month = parseInt(birthDate.substring(2, 4), 10) - 1; // Month is zero-based
    const day = parseInt(birthDate.substring(4, 6), 10);
    const date = new Date(year, month, day);

    return date.getFullYear() === year && date.getMonth() === month && date.getDate() === day;
}

// Helper function: Luhn checksum validation
function calculateLuhnChecksum(number) {
    let sum = 0;
    let alternate = false;

    for (let i = number.length - 1; i >= 0; i--) {
        let digit = parseInt(number[i], 10);
        if (alternate) {
            digit *= 2;
            if (digit > 9) digit -= 9;
        }
        sum += digit;
        alternate = !alternate;
    }

    return (10 - (sum % 10)) % 10;
}