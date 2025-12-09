using POS.DataAccess.Repositories;
using POS.DTOs;
using POS.Services.Interfaces;

namespace POS.Services
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly ISalesOrderRepository _salesOrderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;
        private const decimal TAX_RATE = 0.15m; // 15% IVA

        public SalesOrderService(
            ISalesOrderRepository salesOrderRepository,
            IProductRepository productRepository,
            ICustomerRepository customerRepository)
        {
            _salesOrderRepository = salesOrderRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
        }

        public async Task<TransactionResultDTO> CreateSalesOrderAsync(
    CreateSalesOrderDTO orderDto, string createdBy)
        {
            try
            {
                // ✅ Validación básica de cliente (rápida, no afecta concurrencia)
                var customer = await _customerRepository.GetByIdAsync(orderDto.CustomerId);
                if (customer == null || !customer.IsActive)
                {
                    return new TransactionResultDTO
                    {
                        Success = false,
                        Message = "Cliente seleccionado inválido o inactivo"
                    };
                }

                // ✅ COMENTARIO: Validaciones de stock y productos ahora se manejan 
                // atómicamente en el SP con bloqueos pesimistas para evitar race conditions

                // Preparar detalles de orden para el procedimiento almacenado
                var orderDetails = orderDto.OrderDetails
                    .Select(od => (od.ProductId, od.Quantity, od.UnitPrice))
                    .ToList();

                // Crear orden usando procedimiento almacenado (con transacción + bloqueo pesimista)
                var (orderId, orderNumber, errorMessage) = await _salesOrderRepository.CreateSalesOrderAsync(
                    orderDto.CustomerId,
                    orderDetails,
                    orderDto.Notes,
                    createdBy
                );

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return new TransactionResultDTO
                    {
                        Success = false,
                        Message = errorMessage  // ✅ Mensaje específico del SP (ej: "Stock insuficiente para: Laptop HP...")
                    };
                }

                return new TransactionResultDTO
                {
                    Success = true,
                    Message = "Orden de venta creada exitosamente",
                    OrderId = orderId,
                    OrderNumber = orderNumber
                };
            }
            catch (Exception ex)
            {
                return new TransactionResultDTO
                {
                    Success = false,
                    Message = $"Error al crear la orden de venta: {ex.Message}"
                };
            }
        }

        public async Task<SalesOrderViewModel> GetSalesOrderByIdAsync(int salesOrderId)
        {
            var order = await _salesOrderRepository.GetSalesOrderByIdAsync(salesOrderId);
            if (order == null)
                return null;

            return new SalesOrderViewModel
            {
                SalesOrderId = order.SalesOrderId,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Subtotal = order.Subtotal,
                TaxAmount = order.TaxAmount,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Notes = order.Notes ?? string.Empty,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer.FullName,
                CustomerIdentification = order.Customer.IdentificationNumber,
                CustomerEmail = order.Customer.Email ?? string.Empty,
                CustomerPhone = order.Customer.Phone ?? string.Empty,
                CustomerAddress = order.Customer.Address ?? string.Empty,
                OrderDetails = order.SalesOrderDetails.Select(sod => new SalesOrderDetailViewModel
                {
                    SalesOrderDetailId = sod.SalesOrderDetailId,
                    ProductId = sod.ProductId,
                    ProductCode = sod.Product.ProductCode,
                    ProductName = sod.Product.ProductName,
                    Quantity = sod.Quantity,
                    UnitPrice = sod.UnitPrice,
                    LineTotal = sod.LineTotal
                }).ToList()
            };
        }

        public async Task<SalesOrderSearchDTO> SearchSalesOrdersAsync(
            string searchTerm, int pageNumber, int pageSize)
        {
            try
            {
                var (results, totalRecords) = await _salesOrderRepository.SearchSalesOrdersAsync(
                    searchTerm, pageNumber, pageSize);

                return new SalesOrderSearchDTO
                {
                    SearchTerm = searchTerm,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    Results = results.Select(so => new SalesOrderSummaryDTO
                    {
                        SalesOrderId = so.SalesOrderId,
                        OrderNumber = so.OrderNumber,
                        OrderDate = so.OrderDate,
                        TotalAmount = so.TotalAmount,
                        Status = so.Status,
                        CustomerName = so.Customer?.FullName ?? "Desconocido"
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en SearchSalesOrdersAsync service: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<ApiResponse<decimal>> CalculateTotalsAsync(CreateSalesOrderDTO orderDto)
        {
            try
            {
                decimal subtotal = 0;
                decimal taxAmount = 0;

                foreach (var detail in orderDto.OrderDetails)
                {
                    var product = await _productRepository.GetByIdAsync(detail.ProductId);
                    if (product == null)
                    {
                        return ApiResponse<decimal>.ErrorResponse(
                            $"Producto con ID {detail.ProductId} no encontrado");
                    }

                    decimal lineTotal = detail.Quantity * detail.UnitPrice;
                    subtotal += lineTotal;

                    // Calcular impuesto solo para productos gravables
                    if (product.Taxable)
                    {
                        taxAmount += lineTotal * TAX_RATE;
                    }
                }

                decimal totalAmount = subtotal + taxAmount;

                return ApiResponse<decimal>.SuccessResponse(totalAmount, "Totales calculados exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<decimal>.ErrorResponse($"Error al calcular totales: {ex.Message}");
            }
        }

        public async Task<string> GetNextOrderNumberAsync()
        {
            return await _salesOrderRepository.GetNextOrderNumberAsync();
        }

    }
}