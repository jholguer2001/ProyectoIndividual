using Microsoft.AspNetCore.Mvc;
using POS.DTOs;
using POS.Services.Interfaces;

namespace POS.Web.Controllers
{
    public class SalesOrderController : Controller
    {
        private readonly ISalesOrderService _salesOrderService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;

        public SalesOrderController(
            ISalesOrderService salesOrderService,
            ICustomerService customerService,
            IProductService productService)
        {
            _salesOrderService = salesOrderService;
            _customerService = customerService;
            _productService = productService;
        }

        // GET: SalesOrder/Create
        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateSalesOrderDTO();
            return View(model);
        }

        // POST: SalesOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSalesOrderDTO model)
        {
            try
            {
                // ✅ NORMALIZAR Notes a null si está vacío (ANTES de validar)
                if (string.IsNullOrWhiteSpace(model.Notes))
                {
                    model.Notes = null;
                }

                if (!ModelState.IsValid)
                {
                    // ✅ Remover error de Notes si existe (ya que es opcional)
                    if (ModelState.ContainsKey("Notes"))
                    {
                        ModelState.Remove("Notes");
                    }

                    // Verificar nuevamente después de remover error de Notes
                    if (!ModelState.IsValid)
                    {
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            var errors = ModelState
                                .Where(x => x.Value.Errors.Count > 0)
                                .ToDictionary(
                                    x => x.Key,
                                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                );

                            return Json(new
                            {
                                success = false,
                                message = "Por favor corrija los errores de validación.",
                                errors = errors
                            });
                        }

                        TempData["ErrorMessage"] = "Por favor corrija los errores de validación";
                        return View(model);
                    }
                }

                if (model.OrderDetails == null || !model.OrderDetails.Any())
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = false, message = "Por favor agregue al menos un producto a la orden." });

                    TempData["ErrorMessage"] = "Por favor agregue al menos un producto a la orden";
                    return View(model);
                }

                // ✅ Llamar al servicio con Notes ya normalizado
                var result = await _salesOrderService.CreateSalesOrderAsync(
                    model,
                    User.Identity?.Name ?? "System"
                );

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new
                    {
                        success = result.Success,
                        message = result.Message,
                        orderId = result.OrderId,
                        orderNumber = result.OrderNumber
                    });
                }

                if (result.Success)
                {
                    TempData["SuccessMessage"] = $"¡Orden de venta {result.OrderNumber} creada exitosamente!";
                    return RedirectToAction(nameof(Details), new { id = result.OrderId });
                }

                TempData["ErrorMessage"] = result.Message;
                return View(model);
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = $"Ocurrió un error: {ex.Message}" });

                TempData["ErrorMessage"] = $"Ocurrió un error: {ex.Message}";
                return View(model);
            }
        }

        // GET: SalesOrder/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _salesOrderService.GetSalesOrderByIdAsync(id);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Orden de venta no encontrada";
                return RedirectToAction(nameof(Search));
            }

            return View(order);
        }

        // GET: SalesOrder/Search
        [HttpGet]
        public IActionResult Search()
        {
            return View(new SalesOrderSearchDTO());
        }

        // GET: SalesOrder/SearchOrders
        [HttpGet]
        public async Task<IActionResult> SearchOrders(string searchTerm, int pageNumber = 1)
        {
            try
            {
                Console.WriteLine($"SearchOrders llamado - Término: '{searchTerm}', Página: {pageNumber}");

                const int pageSize = 10;
                var result = await _salesOrderService.SearchSalesOrdersAsync(searchTerm ?? "", pageNumber, pageSize);

                Console.WriteLine($"Búsqueda completada - Se encontraron {result.TotalRecords} registros");

                return PartialView("_SearchOrdersResults", result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en SearchOrders: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Retornar vista parcial de error
                return Content($"<div class='alert alert-danger'>Error: {ex.Message}</div>", "text/html");
            }
        }

        // GET: SalesOrder/PrintPdf/5
        [HttpGet]
        public async Task<IActionResult> PrintPdf(int id)
        {
            var order = await _salesOrderService.GetSalesOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View("PrintInvoice", order);
        }

        // API Methods for AJAX calls

        // POST: SalesOrder/ValidateProduct
        /// <summary>
        /// ⚠️ ADVERTENCIA: Esta validación es solo para mejorar la experiencia del usuario.
        /// La validación REAL de stock ocurre en el SP con bloqueos pesimistas.
        /// Los valores aquí pueden quedar obsoletos en milisegundos en entornos concurrentes.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ValidateProduct([FromBody] ValidateProductRequest request)
        {
            var product = await _productService.GetProductByIdAsync(request.ProductId);

            if (product == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Producto no encontrado o inactivo"
                });
            }

            var hasStock = product.StockQuantity >= request.Quantity;

            return Json(new
            {
                success = hasStock,
                message = hasStock
                    ? "Stock disponible (sujeto a cambios)"
                    : $"Stock insuficiente. Disponible: {product.StockQuantity}",
                currentStock = product.StockQuantity,
                warning = "La validación final de stock ocurre durante la creación de la orden"
            });
        }

        // POST: SalesOrder/CalculateTotals
        [HttpPost]
        public async Task<IActionResult> CalculateTotals([FromBody] CreateSalesOrderDTO orderDto)
        {
            var result = await _salesOrderService.CalculateTotalsAsync(orderDto);
            return Json(result);
        }

        // GET: SalesOrder/GetNextOrderNumber
        [HttpGet]
        public async Task<IActionResult> GetNextOrderNumber()
        {
            try
            {
                var nextOrderNumber = await _salesOrderService.GetNextOrderNumberAsync();
                return Json(new { success = true, orderNumber = nextOrderNumber });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    // Request models for API endpoints
    public class ValidateProductRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

}