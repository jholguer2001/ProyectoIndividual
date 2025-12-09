// Add this temporarily to debug the issue

// Override console.log to show in the page
(function () {
    const debugDiv = $('<div id="debugConsole" style="position:fixed;bottom:0;right:0;width:400px;height:300px;background:#000;color:#0f0;font-family:monospace;font-size:11px;padding:10px;overflow-y:auto;z-index:99999;display:none;"></div>');
    $('body').append(debugDiv);

    // Toggle debug console with Ctrl+D
    $(document).keydown(function (e) {
        if (e.ctrlKey && e.key === 'd') {
            e.preventDefault();
            $('#debugConsole').toggle();
        }
    });

    const originalLog = console.log;
    console.log = function (...args) {
        originalLog.apply(console, args);
        const message = args.map(arg =>
            typeof arg === 'object' ? JSON.stringify(arg, null, 2) : String(arg)
        ).join(' ');
        $('#debugConsole').append($('<div>').text(new Date().toLocaleTimeString() + ': ' + message));
        $('#debugConsole').scrollTop($('#debugConsole')[0].scrollHeight);
    };
})();

// Debug: Log order items before submission
function debugOrderItems() {
    console.log('=== ORDER DEBUG INFO ===');
    console.log('Customer ID:', $('#customerId').val());
    console.log('Order Items Count:', orderItems.length);
    console.log('Order Items:', orderItems);
    console.log('Form Action:', $('#salesOrderForm').attr('action'));

    // Check if all required data is present
    orderItems.forEach((item, index) => {
        console.log(`Item ${index}:`, {
            productId: item.productId,
            quantity: item.quantity,
            unitPrice: item.unitPrice,
            hasAllData: !!(item.productId && item.quantity && item.unitPrice)
        });
    });
}

// Add debug button to the form (temporary)
$(document).ready(function () {
    const debugBtn = $('<button type="button" class="btn btn-info btn-sm" style="position:fixed;top:10px;right:10px;z-index:9999;">Debug Info (Ctrl+D)</button>');
    debugBtn.click(function () {
        $('#debugConsole').toggle();
        debugOrderItems();
    });
    $('body').append(debugBtn);
});