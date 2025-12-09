// File: wwwroot/js/sales-order-create.js
// Sales Order Creation - Complete Implementation

let orderItems = [];
let currentEditIndex = -1;
const TAX_RATE = 0.15;

// ============================================================================
// INITIALIZATION
// ============================================================================

$(document).ready(function () {
    initializeEventHandlers();
    displayOrderDate();
    loadNextOrderNumber();
});

function initializeEventHandlers() {
    // Customer search button
    $('#btnSearchCustomer').click(function () {
        openCustomerSearchModal();
    });

    // Add product button
    $('#btnAddProduct').click(function () {
        openProductSearchModal();
    });

    // Customer search input with debounce
    let customerSearchTimeout;
    $('#customerSearchInput').on('input', function () {
        clearTimeout(customerSearchTimeout);
        const searchTerm = $(this).val();
        customerSearchTimeout = setTimeout(() => searchCustomers(searchTerm, 1), 500);
    });

    // Product search input with debounce
    let productSearchTimeout;
    $('#productSearchInput').on('input', function () {
        clearTimeout(productSearchTimeout);
        const searchTerm = $(this).val();
        productSearchTimeout = setTimeout(() => searchProducts(searchTerm, 1), 500);
    });

    // Form submission validation
    $('#salesOrderForm').submit(function (e) {
        e.preventDefault();

        console.log('Form submitted');

        // Disable button immediately
        const $submitBtn = $('#btnSubmitOrder');
        $submitBtn.prop('disabled', true);
        $submitBtn.html('<span class="spinner-border spinner-border-sm me-2"></span>Procesando...');

        // Validate form
        if (!validateForm()) {
            console.log('Validation failed');
            // Re-enable button if validation fails
            $submitBtn.prop('disabled', false);
            $submitBtn.html('<i class="fas fa-save"></i> Crear Orden');
            return false;
        }

        console.log('Validation passed, adding items to form');
        addOrderItemsToForm();

        console.log('Submitting order...');
        submitOrder();

        return false;
    });
}

function loadNextOrderNumber() {
    $.ajax({
        url: '/SalesOrder/GetNextOrderNumber',
        type: 'GET',
        success: function (response) {
            if (response.success) {
                $('#orderNumberDisplay').text(response.orderNumber);
                console.log('✅ Número de orden cargado:', response.orderNumber);
            } else {
                $('#orderNumberDisplay').text('Error al cargar');
                console.error('❌ Error al obtener número de orden:', response.message);
            }
        },
        error: function (xhr, status, error) {
            $('#orderNumberDisplay').text('Error al cargar');
            console.error('❌ Error AJAX al obtener número de orden:', error);
        }
    });
}

// ============================================================================
// CUSTOMER SEARCH FUNCTIONALITY
// ============================================================================

function openCustomerSearchModal() {
    console.log('Opening customer search modal...');

    // Clear input first
    $('#customerSearchInput').val('');

    // Show loading state
    $('#customerSearchResults').html(`
        <div class="text-center p-4">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Cargando...</span>
            </div>
            <p class="mt-2 text-muted">Cargando clientes...</p>
        </div>
    `);

    // Get modal element
    const modalElement = $('#customerSearchModal');

    // Remove previous event handlers to avoid duplicates
    modalElement.off('shown.bs.modal');

    // Wait for modal to be fully shown, then load data
    modalElement.one('shown.bs.modal', function () {
        console.log('Customer modal is now visible, loading full list...');
        searchCustomers('', 1); // Load page 1 with empty search term
        setTimeout(() => $('#customerSearchInput').focus(), 300);
    });

    // Show modal
    modalElement.modal('show');
}

function searchCustomers(searchTerm, pageNumber) {
    console.log('searchCustomers called:', { searchTerm, pageNumber });

    $.ajax({
        url: '/Customer/Search',
        type: 'GET',
        data: { searchTerm: searchTerm, pageNumber: pageNumber },
        beforeSend: function () {
            console.log('Sending request to /Customer/Search');
        },
        success: function (response) {
            console.log('Customer search success, response length:', response.length);
            $('#customerSearchResults').html(response);
            attachCustomerPaginationHandlers();
        },
        error: function (xhr, status, error) {
            console.error('Customer search error:', { status, error, xhr });
            $('#customerSearchResults').html(`
                <div class="alert alert-danger">
                    <i class="fas fa-exclamation-circle"></i> 
                    Error al cargar clientes. Por favor intente de nuevo.
                    <br><small>Error: ${error || 'Unknown error'}</small>
                </div>
            `);
        }
    });
}

function attachCustomerPaginationHandlers() {
    $('.customer-pagination a').click(function (e) {
        e.preventDefault();
        const pageNumber = $(this).data('page');
        const searchTerm = $('#customerSearchInput').val();
        searchCustomers(searchTerm, pageNumber);
    });
}

function selectCustomer(customerId, fullName, identification, email, phone, address) {
    // 1. Guardar en campos ocultos
    $('#customerId').val(customerId);
    $('#customerDisplay').val(fullName);

    // 2. Renderizar la nueva tarjeta
    renderCustomerCard(customerId, fullName, identification, email, phone, address);

    // 3. Cerrar modal
    $('#customerSearchModal').modal('hide');

    console.log('✅ Nueva tarjeta cliente renderizada');
    return false;
}

function renderCustomerCard(customerId, fullName, identification, email, phone, address) {
    const html = `
        <div class="card border-info shadow-sm" id="customerInfoCard">
            <div class="card-header bg-info text-white">
                <h6 class="mb-0"><i class="fas fa-user-check"></i> Cliente Seleccionado</h6>
            </div>
            <div class="card-body">
                <h5 class="mb-1">${escapeHtml(fullName)}</h5>
                <hr class="my-2">
                <div class="row">
                    <div class="col-6 text-muted small">Cédula:</div>
                    <div class="col-6 fw-bold">${escapeHtml(identification)}</div>

                    <div class="col-6 text-muted small">Email:</div>
                    <div class="col-6">${escapeHtml(email || 'N/A')}</div>

                    <div class="col-6 text-muted small">Teléfono:</div>
                    <div class="col-6">${escapeHtml(phone || 'N/A')}</div>

                    <div class="col-6 text-muted small">Dirección:</div>
                    <div class="col-6">${escapeHtml(address || 'N/A')}</div>
                </div>
            </div>
        </div>`;

    $('#newCustomerCardWrapper').html(html);
}

// ============================================================================
// PRODUCT SEARCH FUNCTIONALITY
// ============================================================================

function openProductSearchModal() {
    console.log('Opening product search modal...');

    // Clear input
    $('#productSearchInput').val('');

    // Show loading state
    $('#productSearchResults').html(`
        <div class="text-center p-4">
            <div class="spinner-border text-success" role="status">
                <span class="visually-hidden">Cargando...</span>
            </div>
            <p class="mt-2 text-muted">Cargando productos...</p>
        </div>
    `);

    // Get modal element
    const modalElement = $('#productSearchModal');

    // Remove previous event handlers to avoid duplicates
    modalElement.off('shown.bs.modal');

    // Wait for modal to be fully shown, then load data
    modalElement.one('shown.bs.modal', function () {
        console.log('Product modal is now visible, loading full list...');
        searchProducts('', 1); // Load page 1 with empty search term
        setTimeout(() => $('#productSearchInput').focus(), 300);
    });

    // Show modal
    modalElement.modal('show');
}

function searchProducts(searchTerm, pageNumber) {
    console.log('searchProducts called:', { searchTerm, pageNumber });

    $.ajax({
        url: '/Product/Search',
        type: 'GET',
        data: { searchTerm: searchTerm, pageNumber: pageNumber },
        beforeSend: function () {
            console.log('Sending request to /Product/Search');
        },
        success: function (response) {
            console.log('Product search success, response length:', response.length);
            $('#productSearchResults').html(response);
            attachProductPaginationHandlers();
        },
        error: function (xhr, status, error) {
            console.error('Product search error:', { status, error, xhr });
            $('#productSearchResults').html(`
                <div class="alert alert-danger">
                    <i class="fas fa-exclamation-circle"></i> 
                    Error al cargar productos. Por favor intente de nuevo.
                    <br><small>Error: ${error || 'Unknown error'}</small>
                </div>
            `);
        }
    });
}

function attachProductPaginationHandlers() {
    $('.product-pagination a').click(function (e) {
        e.preventDefault();
        const pageNumber = $(this).data('page');
        const searchTerm = $('#productSearchInput').val();
        searchProducts(searchTerm, pageNumber);
    });
}

// ============================================================================
// ORDER ITEM MANAGEMENT
// ============================================================================

// Called from product search modal (wrapper function for view compatibility)
function selectProductForOrder(productId, productCode, productName, unitPrice, stockQuantity, taxable) {
    console.log("📌 selectProductForOrder() recibió:");
    console.log("Code:", productCode);
    console.log("Name:", productName);
    console.log("Price:", unitPrice);
    console.log("Stock:", stockQuantity);
    console.log("Taxable:", taxable);

    if (typeof currentEditIndex !== 'undefined' && currentEditIndex >= 0) {
        updateProductInOrder(currentEditIndex, productId, productCode, productName, unitPrice, stockQuantity, taxable);
    } else {
        selectProduct(productId, productCode, productName, unitPrice, stockQuantity, taxable);
    }
}


function selectProduct(productId, productCode, productName, unitPrice, stockQuantity, taxable) {
    // REQUIREMENT 3: Check if product already exists in order
    const existingIndex = orderItems.findIndex(item => item.productId === productId);

    if (existingIndex !== -1) {
        showError('Este producto ya esta en la orden. Por favor actualice la cantidad.');
        return;
    }

    // Check if we're updating an existing item
    if (currentEditIndex !== -1) {
        updateProductInOrder(currentEditIndex, productId, productCode, productName, unitPrice, stockQuantity, taxable);
        return;
    }

    // Add new product with quantity 1
    const quantity = 1;
    const lineTotal = unitPrice * quantity;

    const item = {
        productId: productId,
        productCode: productCode,
        productName: productName,
        unitPrice: unitPrice,
        quantity: quantity,
        lineTotal: lineTotal,
        stockQuantity: stockQuantity,
        taxable: taxable
    };

    orderItems.push(item);
    renderOrderItems();
    calculateTotals();

    $('#productSearchModal').modal('hide');
    showSuccess('Producto añadido exitosamente');
}

function renderOrderItems() {
    const tbody = $('#orderItemsBody');
    tbody.empty();
    if (orderItems.length === 0) {
        tbody.append(`
            <tr id="emptyRow">
                <td colspan="6" class="text-center text-muted">
                    <i class="fas fa-inbox"></i> Aun no se han añadido productos
                </td>
            </tr>
        `);
        return;
    }
    // Sort items by lineTotal (descending - mayor a menor)
    const sortedItems = [...orderItems].sort((a, b) => b.lineTotal - a.lineTotal);
    sortedItems.forEach((item) => {
        // Find the original index in orderItems array (not sorted)
        const originalIndex = orderItems.findIndex(i =>
            i.productId === item.productId &&
            i.productCode === item.productCode
        );
        const row = `
            <tr>
                <td>${escapeHtml(item.productCode)}</td>
                <td>${escapeHtml(item.productName)}</td>
                <td class="text-end">$${item.unitPrice.toFixed(2)}</td>
                <td class="text-end">
                    <div class="d-flex flex-column align-items-end">
                        <input type="number" class="form-control form-control-sm quantity-input text-end" 
                               data-index="${originalIndex}" 
                               value="${item.quantity}" 
                               min="0" 
                               max="${item.stockQuantity}"
                               style="width: 100px;" />
                        <small class="text-muted">Stock: ${item.stockQuantity}</small>
                    </div>
                </td>
                <td class="text-end fw-bold pe-4">$${item.lineTotal.toFixed(2)}</td>
                <td>
                    <button type="button" class="btn btn-sm btn-warning btn-update me-1" data-index="${originalIndex}">
                        <i class="fas fa-edit"></i> Actualizar
                    </button>
                    <button type="button" class="btn btn-sm btn-danger btn-remove" data-index="${originalIndex}">
                        <i class="fas fa-trash"></i> Eliminar
                    </button>
                </td>
            </tr>
        `;
        tbody.append(row);
        console.log('📦 Producto en fila:', {
            productCode: item.productCode,
            productName: item.productName,
            lineTotal: item.lineTotal,
            stockQuantity: item.stockQuantity
        });
    });
    attachItemHandlers();
}

function attachItemHandlers() {
    // Quantity change handler with validation
    $('.quantity-input').on('change', function () {
        const index = $(this).data('index');
        const newQuantity = parseInt($(this).val());
        const item = orderItems[index];

        // REQUIREMENT 9: Validate stock
        if (newQuantity > item.stockQuantity) {
            showError(`No puede superar el stock disponible (${item.stockQuantity})`);
            $(this).val(item.quantity);
            return;
        }

        // Allow 0 quantity (will be validated on form submission)
        if (newQuantity < 0 || isNaN(newQuantity)) {
            $(this).val(0);
            return;
        }

        item.quantity = newQuantity;
        item.lineTotal = item.unitPrice * newQuantity;
        renderOrderItems();
        calculateTotals();
    });

    // Update button (REQUIREMENT 11: Replace product)
    $('.btn-update').click(function () {
        const index = $(this).data('index');
        currentEditIndex = index;
        openProductSearchModal();
    });

    // Remove button (REQUIREMENT 10)
    $('.btn-remove').click(function () {
        const index = $(this).data('index');
        if (confirm('Estas seguro que quieres eliminar este productos?')) {
            orderItems.splice(index, 1);
            renderOrderItems();
            calculateTotals();
            showSuccess('Producto eliminado exitosamente');
        }
    });
}

// REQUIREMENT 11: Update product (replace with another)
function updateProductInOrder(index, productId, productCode, productName, unitPrice, stockQuantity, taxable) {
    // Check if new product already exists (except current position)
    const existingIndex = orderItems.findIndex((item, idx) =>
        item.productId === productId && idx !== index
    );

    if (existingIndex !== -1) {
        showError('Este producto ya esta en la orden.');
        return;
    }

    const oldQuantity = orderItems[index].quantity;
    const newQuantity = Math.min(oldQuantity, stockQuantity);

    orderItems[index] = {
        productId: productId,
        productCode: productCode,
        productName: productName,
        unitPrice: unitPrice,
        quantity: newQuantity,
        lineTotal: unitPrice * newQuantity,
        stockQuantity: stockQuantity,
        taxable: taxable
    };

    renderOrderItems();
    calculateTotals();
    $('#productSearchModal').modal('hide');
    showSuccess('Producto actualizado exitosamente');
    currentEditIndex = -1;
}

// ============================================================================
// CALCULATIONS & VALIDATION
// ============================================================================

// REQUIREMENT 4: Calculate totals, subtotals, and tax
function calculateTotals() {
    let subtotal = 0;
    let taxAmount = 0;

    orderItems.forEach(item => {
        subtotal += item.lineTotal;
        if (item.taxable) {
            taxAmount += item.lineTotal * TAX_RATE;
        }
    });

    const total = subtotal + taxAmount;

    $('#subtotalDisplay').text(`$${subtotal.toFixed(2)}`);
    $('#taxDisplay').text(`$${taxAmount.toFixed(2)}`);
    $('#totalDisplay').text(`$${total.toFixed(2)}`);
}


// Form validation (REQUIREMENT 6)
function validateForm() {
    const customerId = $('#customerId').val();

    if (!customerId || customerId === '0') {
        showError('Por favor seleccione un cliente');
        return false;
    }

    if (orderItems.length === 0) {
        showError('Por favor, añada al menos un producto al pedido');
        return false;
    }

    // Validate that all items have quantity > 0
    const itemsWithZeroQuantity = orderItems.filter(item => item.quantity === 0 || item.quantity < 1);
    if (itemsWithZeroQuantity.length > 0) {
        const productNames = itemsWithZeroQuantity.map(item => item.productName).join(', ');
        showError(`Los siguientes productos tienen cantidad 0 o inválida: ${productNames}. Por favor ingrese una cantidad válida.`);
        return false;
    }

    return true;
}

// ============================================================================
// FORM SUBMISSION
// ============================================================================

function addOrderItemsToForm() {
    // Remove existing hidden inputs
    $('input[name^="OrderDetails"]').remove();

    console.log('Adding order items to form:', orderItems);

    // Ensure Notes field has a value (even if empty)
    const notesField = $('#Notes');
    if (notesField.length && !notesField.val()) {
        notesField.val(''); // Empty field but present
    }

    // Add order items as hidden inputs - WITH ALL REQUIRED FIELDS
    orderItems.forEach((item, index) => {
        // ProductId
        $('<input>').attr({
            type: 'hidden',
            name: `OrderDetails[${index}].ProductId`,
            value: item.productId
        }).appendTo('#salesOrderForm');

        // Quantity
        $('<input>').attr({
            type: 'hidden',
            name: `OrderDetails[${index}].Quantity`,
            value: item.quantity
        }).appendTo('#salesOrderForm');

        // UnitPrice
        $('<input>').attr({
            type: 'hidden',
            name: `OrderDetails[${index}].UnitPrice`,
            value: item.unitPrice
        }).appendTo('#salesOrderForm');

        // ProductCode (IMPORTANT FOR SERVER)
        $('<input>').attr({
            type: 'hidden',
            name: `OrderDetails[${index}].ProductCode`,
            value: item.productCode
        }).appendTo('#salesOrderForm');

        // ProductName (IMPORTANT FOR SERVER)
        $('<input>').attr({
            type: 'hidden',
            name: `OrderDetails[${index}].ProductName`,
            value: item.productName
        }).appendTo('#salesOrderForm');

        // Taxable (CRITICAL FOR TAX CALCULATION!)
        $('<input>').attr({
            type: 'hidden',
            name: `OrderDetails[${index}].Taxable`,
            value: item.taxable || false
        }).appendTo('#salesOrderForm');

        console.log(`Item ${index} - Taxable: ${item.taxable}`);
    });

    console.log('Hidden inputs added. Form data:', $('#salesOrderForm').serialize());
}

// Submit order function - REDIRECTS ON SUCCESS, STAYS ON FAILURE
function submitOrder() {
    const $submitBtn = $('#btnSubmitOrder');
    // Note: Button is already disabled by form submit handler

    console.log('========== SUBMIT ORDER STARTED ==========');
    console.log('Timestamp:', new Date().toISOString());

    // Get form data
    const formData = $('#salesOrderForm').serialize();
    console.log('Form data to submit:', formData);
    console.log('Order items array:', JSON.stringify(orderItems, null, 2));

    // Submit via AJAX
    $.ajax({
        url: $('#salesOrderForm').attr('action'),
        type: 'POST',
        data: formData,
        dataType: 'json',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        success: function (response) {
            console.log('========== SUCCESS CALLBACK ==========');
            console.log('Response received:', JSON.stringify(response, null, 2));

            // Verify if it was truly successful
            if (response.success === true) {
                console.log('✅ ORDER SAVED SUCCESSFULLY!');
                console.log('Order ID:', response.orderId);
                console.log('Order Number:', response.orderNumber);

                showSuccess(`Order ${response.orderNumber || ''} creado exitosamente! Abriendo factura...`);

                // Open PDF invoice in new tab and redirect to search
                setTimeout(function () {
                    window.open('/SalesOrder/PrintPdf/' + response.orderId, '_blank');
                    window.location.href = '/SalesOrder/Search';
                }, 1500);

            } else {
                // Server returned an error - STAY ON PAGE
                console.error('❌ SERVER RETURNED ERROR');
                console.error('Error message:', response.message);
                showError(response.message || 'Error al crear orden de venta');

                // Re-enable button to try again
                $submitBtn.prop('disabled', false);
                $submitBtn.html('<i class="fas fa-save"></i> Crear Orden');
            }
        },
        error: function (xhr, status, error) {
            console.error('========== ERROR CALLBACK ==========');
            console.error('❌ AJAX Error:', error);
            console.error('Status:', status);
            console.error('Status Code:', xhr.status);
            console.error('Response Text:', xhr.responseText);
            console.error('Response JSON:', xhr.responseJSON);

            let errorMessage = 'Error al crear orden de venta. Por favor intente de nuevo.';

            // Try to extract error message
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMessage = xhr.responseJSON.message;
            } else if (xhr.responseJSON && xhr.responseJSON.success === false) {
                errorMessage = xhr.responseJSON.message || errorMessage;
            } else if (xhr.responseText) {
                // Try to parse HTML validation errors
                try {
                    const parser = new DOMParser();
                    const doc = parser.parseFromString(xhr.responseText, 'text/html');
                    const validationSummary = doc.querySelector('.validation-summary-errors');
                    if (validationSummary) {
                        errorMessage = validationSummary.textContent.trim();
                    }
                } catch (e) {
                    console.error('Error parsing response:', e);
                }
            }

            showError(errorMessage);

            // Re-enable button to try again
            $submitBtn.prop('disabled', false);
            $submitBtn.html('<i class="fas fa-save"></i> Crear Orden');
        },
        complete: function () {
            console.log('========== AJAX REQUEST COMPLETED ==========');
        }
    });
}

// ============================================================================
// UTILITY FUNCTIONS
// ============================================================================

function displayOrderDate() {
    const today = new Date();
    const options = {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        weekday: 'long'
    };
    const formattedDate = today.toLocaleDateString('es-EC', options);
    $('#orderDate').text(formattedDate);
}

function showSuccess(message) {
    console.log('✅ Success:', message);

    // Remove previous alerts
    $('.alert').remove();

    const alert = `
        <div class="alert alert-success alert-dismissible fade show position-relative" role="alert" style="font-size: 1.1rem; padding: 1.5rem; border-width: 2px;">
            <i class="fas fa-check-circle me-2" style="font-size: 1.5rem;"></i>
            <strong>Exito!</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    $('.card-body').first().prepend(alert);

    // Scroll to top to show the success message
    //window.scrollTo({ top: 0, behavior: 'smooth' });

    // NO auto-fadeout so user can verify
}

function showError(message) {
    console.log('❌ Error:', message);

    // Remove previous alerts
    $('.alert').remove();

    const alert = `
        <div class="alert alert-danger alert-dismissible fade show position-relative" role="alert" style="font-size: 1.1rem; padding: 1.5rem; border-width: 2px;">
            <i class="fas fa-exclamation-circle me-2" style="font-size: 1.5rem;"></i>
            <strong>Error!</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    $('.card-body').first().prepend(alert);

    // Scroll to top to show the error
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

function escapeHtml(text) {
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.replace(/[&<>"']/g, m => map[m]);
}