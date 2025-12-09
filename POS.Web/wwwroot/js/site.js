/**
 * Global utility functions for the POS System
 */

// Initialize on document ready
$(document).ready(function () {
    initializeGlobalFeatures();
});

/**
 * Initialize global features
 */
function initializeGlobalFeatures() {
    // Auto-dismiss alerts after 5 seconds
    setTimeout(function () {
        $('.alert:not(.alert-permanent)').fadeOut('slow');
    }, 5000);

    // Initialize tooltips if Bootstrap tooltips are needed
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Add loading indicator to buttons on form submit
    $('form').on('submit', function () {
        var $submitBtn = $(this).find('button[type="submit"]');
        if (!$submitBtn.hasClass('no-loading')) {
            $submitBtn.prop('disabled', true);
            $submitBtn.html('<span class="spinner-border spinner-border-sm me-2"></span>Processing...');
        }
    });

    // Prevent double submission
    $('form').on('submit', function (e) {
        var $form = $(this);
        if ($form.data('submitted') === true) {
            e.preventDefault();
            return false;
        }
        $form.data('submitted', true);
    });
}

/**
 * Format currency with dollar sign
 * @param {number} amount - The amount to format
 * @returns {string} - Formatted currency string
 */
function formatCurrency(amount) {
    return '$' + parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}

/**
 * Format date to MM/DD/YYYY
 * @param {Date|string} date - The date to format
 * @returns {string} - Formatted date string
 */
function formatDate(date) {
    var d = new Date(date);
    var month = '' + (d.getMonth() + 1);
    var day = '' + d.getDate();
    var year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [month, day, year].join('/');
}

/**
 * Format date and time
 * @param {Date|string} date - The date to format
 * @returns {string} - Formatted date and time string
 */
function formatDateTime(date) {
    var d = new Date(date);
    return d.toLocaleString('en-US', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        hour12: true
    });
}

/**
 * Show success message
 * @param {string} message - The message to display
 * @param {number} duration - Display duration in milliseconds (default: 3000)
 */
function showSuccessMessage(message, duration = 3000) {
    var alert = $('<div class="alert alert-success alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3" role="alert" style="z-index: 9999; min-width: 300px;">')
        .html('<i class="fas fa-check-circle me-2"></i>' + message +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>');

    $('body').append(alert);

    setTimeout(function () {
        alert.fadeOut('slow', function () {
            $(this).remove();
        });
    }, duration);
}

/**
 * Show error message
 * @param {string} message - The message to display
 * @param {number} duration - Display duration in milliseconds (default: 5000)
 */
function showErrorMessage(message, duration = 5000) {
    var alert = $('<div class="alert alert-danger alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3" role="alert" style="z-index: 9999; min-width: 300px;">')
        .html('<i class="fas fa-exclamation-circle me-2"></i>' + message +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>');

    $('body').append(alert);

    setTimeout(function () {
        alert.fadeOut('slow', function () {
            $(this).remove();
        });
    }, duration);
}

/**
 * Show info message
 * @param {string} message - The message to display
 * @param {number} duration - Display duration in milliseconds (default: 3000)
 */
function showInfoMessage(message, duration = 3000) {
    var alert = $('<div class="alert alert-info alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3" role="alert" style="z-index: 9999; min-width: 300px;">')
        .html('<i class="fas fa-info-circle me-2"></i>' + message +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>');

    $('body').append(alert);

    setTimeout(function () {
        alert.fadeOut('slow', function () {
            $(this).remove();
        });
    }, duration);
}

/**
 * Show warning message
 * @param {string} message - The message to display
 * @param {number} duration - Display duration in milliseconds (default: 4000)
 */
function showWarningMessage(message, duration = 4000) {
    var alert = $('<div class="alert alert-warning alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3" role="alert" style="z-index: 9999; min-width: 300px;">')
        .html('<i class="fas fa-exclamation-triangle me-2"></i>' + message +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>');

    $('body').append(alert);

    setTimeout(function () {
        alert.fadeOut('slow', function () {
            $(this).remove();
        });
    }, duration);
}

/**
 * Show loading overlay
 * @param {string} message - Optional loading message
 */
function showLoading(message = 'Loading...') {
    var overlay = $('<div id="loadingOverlay" class="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center" style="background-color: rgba(0,0,0,0.5); z-index: 9998;">')
        .html('<div class="text-center"><div class="spinner-border text-light" style="width: 3rem; height: 3rem;" role="status"></div><p class="text-light mt-3">' + message + '</p></div>');

    $('body').append(overlay);
}

/**
 * Hide loading overlay
 */
function hideLoading() {
    $('#loadingOverlay').fadeOut('fast', function () {
        $(this).remove();
    });
}

/**
 * Confirm dialog
 * @param {string} message - The confirmation message
 * @param {function} callback - Callback function if confirmed
 */
function confirmDialog(message, callback) {
    if (confirm(message)) {
        if (typeof callback === 'function') {
            callback();
        }
        return true;
    }
    return false;
}

/**
 * Validate email format
 * @param {string} email - Email address to validate
 * @returns {boolean} - True if valid, false otherwise
 */
function isValidEmail(email) {
    var regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}

/**
 * Validate phone number (simple validation)
 * @param {string} phone - Phone number to validate
 * @returns {boolean} - True if valid, false otherwise
 */
function isValidPhone(phone) {
    var regex = /^[\d\s\-\(\)\+]+$/;
    return regex.test(phone) && phone.replace(/\D/g, '').length >= 10;
}

/**
 * Validate Ecuadorian identification number (cédula)
 * @param {string} cedula - ID number to validate
 * @returns {boolean} - True if valid, false otherwise
 */
function isValidEcuadorianId(cedula) {
    if (!cedula || cedula.length !== 10) {
        return false;
    }

    // Check if all characters are digits
    if (!/^\d+$/.test(cedula)) {
        return false;
    }

    // Province code validation (01-24)
    var provinceCode = parseInt(cedula.substring(0, 2));
    if (provinceCode < 1 || provinceCode > 24) {
        return false;
    }

    // Third digit must be less than 6 for natural persons
    var thirdDigit = parseInt(cedula.charAt(2));
    if (thirdDigit >= 6) {
        return false;
    }

    // Validate check digit
    var coefficients = [2, 1, 2, 1, 2, 1, 2, 1, 2];
    var sum = 0;

    for (var i = 0; i < 9; i++) {
        var digit = parseInt(cedula.charAt(i));
        var product = digit * coefficients[i];

        if (product >= 10) {
            product -= 9;
        }

        sum += product;
    }

    var checkDigit = (10 - (sum % 10)) % 10;
    var lastDigit = parseInt(cedula.charAt(9));

    return checkDigit === lastDigit;
}

/**
 * Escape HTML to prevent XSS
 * @param {string} text - Text to escape
 * @returns {string} - Escaped text
 */
function escapeHtml(text) {
    var map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.replace(/[&<>"']/g, function (m) { return map[m]; });
}

/**
 * Debounce function to limit function calls
 * @param {function} func - Function to debounce
 * @param {number} wait - Wait time in milliseconds
 * @returns {function} - Debounced function
 */
function debounce(func, wait) {
    var timeout;
    return function executedFunction() {
        var context = this;
        var args = arguments;
        var later = function () {
            timeout = null;
            func.apply(context, args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

/**
 * Parse JSON safely
 * @param {string} jsonString - JSON string to parse
 * @param {*} defaultValue - Default value if parsing fails
 * @returns {*} - Parsed object or default value
 */
function safeJsonParse(jsonString, defaultValue = null) {
    try {
        return JSON.parse(jsonString);
    } catch (e) {
        console.error('JSON parse error:', e);
        return defaultValue;
    }
}

/**
 * Print element content
 * @param {string} elementId - ID of element to print
 */
function printElement(elementId) {
    var content = document.getElementById(elementId).innerHTML;
    var printWindow = window.open('', '', 'height=600,width=800');

    printWindow.document.write('<html><head><title>Print</title>');
    printWindow.document.write('<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />');
    printWindow.document.write('</head><body>');
    printWindow.document.write(content);
    printWindow.document.write('</body></html>');

    printWindow.document.close();
    printWindow.focus();

    setTimeout(function () {
        printWindow.print();
        printWindow.close();
    }, 250);
}

/**
 * Copy text to clipboard
 * @param {string} text - Text to copy
 */
function copyToClipboard(text) {
    var textarea = document.createElement('textarea');
    textarea.value = text;
    textarea.style.position = 'fixed';
    textarea.style.opacity = '0';
    document.body.appendChild(textarea);
    textarea.select();

    try {
        document.execCommand('copy');
        showSuccessMessage('Copied to clipboard!');
    } catch (err) {
        showErrorMessage('Failed to copy to clipboard');
    }

    document.body.removeChild(textarea);
}

/**
 * Generate random ID
 * @param {number} length - Length of ID (default: 8)
 * @returns {string} - Random ID
 */
function generateRandomId(length = 8) {
    var chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var result = '';
    for (var i = 0; i < length; i++) {
        result += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return result;
}

// Export functions to window object for global access
window.posUtils = {
    formatCurrency: formatCurrency,
    formatDate: formatDate,
    formatDateTime: formatDateTime,
    showSuccessMessage: showSuccessMessage,
    showErrorMessage: showErrorMessage,
    showInfoMessage: showInfoMessage,
    showWarningMessage: showWarningMessage,
    showLoading: showLoading,
    hideLoading: hideLoading,
    confirmDialog: confirmDialog,
    isValidEmail: isValidEmail,
    isValidPhone: isValidPhone,
    isValidEcuadorianId: isValidEcuadorianId,
    escapeHtml: escapeHtml,
    debounce: debounce,
    safeJsonParse: safeJsonParse,
    printElement: printElement,
    copyToClipboard: copyToClipboard,
    generateRandomId: generateRandomId
};