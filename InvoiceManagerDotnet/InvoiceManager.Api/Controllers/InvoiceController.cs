using Microsoft.AspNetCore.Mvc;
using InvoiceManager.Application.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using InvoiceManager.Api.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;


namespace InvoiceManager.Api.Controllers
{
    /// <summary>
    /// API para gestionar operaciones relacionadas con facturas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService _invoiceService;

        public InvoiceController(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// Obtiene todas las facturas del sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint retorna una lista completa de facturas, sin filtros.
        /// </remarks>
        /// <returns>Una lista de objetos factura.</returns>
        /// <response code="200">Facturas obtenidas correctamente.</response>
        /// <response code="500">Error interno del servidor.</response>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            try
            {
                var invoices = await _invoiceService.GetAllInvoices();

                return Ok(new
                {
                    success = true,
                    data = invoices
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error al obtener las facturas."
                });
            }
        }

        /// <summary>
        /// Obtiene una factura por su número único.
        /// </summary>
        /// <param name="invoiceNumber">Número identificador de la factura.</param>
        /// <returns>La factura correspondiente.</returns>
        /// <response code="200">Factura encontrada exitosamente.</response>
        /// <response code="404">No se encontró una factura con el número proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [Authorize]
        [HttpGet("{invoiceNumber}")]
        public async Task<IActionResult> GetInvoiceByNumber(int invoiceNumber)
        {
            try
            {
                // Buscq la factura por el número
                var invoice = await _invoiceService.GetInvoiceByNumber(invoiceNumber);

                if (invoice == null)
                {
                    return NotFound(new { success = false, message = "Factura no encontrada." });
                }

                return Ok(new
                {
                    success = true,
                    data = invoice
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error al obtener la factura."
                });
            }
        }
        
        /// <summary>
        /// Filtra facturas por estado de emisión o de pago.
        /// </summary>
        /// <param name="invoiceStatus">Estado de la factura (Issued, Cancelled, etc).</param>
        /// <param name="paymentStatus">Estado del pago (Paid, Pending, Overdue).</param>
        /// <returns>Facturas que cumplen con los filtros aplicados.</returns>
        /// <response code="200">Facturas filtradas correctamente.</response>
        /// <response code="400">No se proporcionó ningún filtro.</response>
        /// <response code="500">Error interno del servidor.</response>
        [Authorize]
        [HttpGet("status")]
        public async Task<IActionResult> GetInvoicesByStatus([FromQuery] string? invoiceStatus, [FromQuery] string? paymentStatus)
        {
            try
            {
                // Validar que al menos uno de los parámetros de filtrado sea proporcionado
                if (invoiceStatus == null && paymentStatus == null)
                {
                    return BadRequest(new { success = false, message = "Debes proporcionar al menos un filtro (estado de factura o estado de pago)." });
                }

                // Filtrar las facturas
                var invoices = await _invoiceService.GetInvoicesByStatusAsync(invoiceStatus, paymentStatus);

                return Ok(new
                {
                    success = true,
                    data = invoices
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error al obtener las facturas por estado."
                });
            }
        }

        /// <summary>
        /// Importa facturas desde un archivo JSON (vía formulario).
        /// </summary>
        /// <remarks>
        /// El archivo debe tener formato JSON y seguir la estructura de <see cref="InvoiceImportRequest"/>.
        /// Este método omite facturas duplicadas o mal formateadas, y retorna errores detallados.
        /// </remarks>
        /// <param name="form">Formulario que contiene el archivo JSON.</param>
        /// <returns>Resultado del proceso de importación, incluyendo facturas válidas y errores.</returns>
        /// <response code="200">Importación completada con éxito, aunque puede incluir errores parciales.</response>
        /// <response code="400">El archivo no es válido, está vacío o mal estructurado.</response>
        /// <response code="500">Error interno al procesar el archivo.</response>
        [Authorize]
        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportInvoices([FromForm] ImportInvoiceForm form)
        {
            var file = form.File;

            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, message = "No se ha subido ningún archivo." });

            if (!file.FileName.EndsWith(".json"))
                return BadRequest(new { success = false, message = "El archivo debe ser un archivo JSON." });

            try
            {
                using var stream = new StreamReader(file.OpenReadStream());
                var content = await stream.ReadToEndAsync();

                var request = JsonSerializer.Deserialize<InvoiceImportRequest>(content);

                if (request == null || request.Invoices == null || request.Invoices.Count == 0)
                    return BadRequest(new { success = false, message = "El archivo JSON está vacío o no contiene facturas." });

                var result = await _invoiceService.ImportInvoices(request.Invoices);

                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                return Ok(new { success = true, message = result.Message, errors = result.Errors });
            }
            catch (JsonException)
            {
                return BadRequest(new { success = false, message = "El archivo JSON tiene un formato incorrecto." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al procesar el archivo." });
            }
        }

        /// <summary>
        /// Agrega una nota de crédito a una factura existente.
        /// </summary>
        /// <param name="invoiceNumber">Número de factura a la que se aplicará la nota de crédito.</param>
        /// <param name="request">Objeto con el monto de la nota de crédito.</param>
        /// <returns>Mensaje de confirmación o errores.</returns>
        /// <response code="200">Nota de crédito agregada exitosamente.</response>
        /// <response code="400">Error de validación: nota excede el monto restante o la factura no existe.</response>
        /// <response code="500">Error interno del servidor.</response>
        [Authorize]
        [HttpPost("{invoiceNumber}/credit-note")]
        public async Task<IActionResult> AddCreditNoteToInvoice(int invoiceNumber, [FromBody] CreditNoteRequest request)
        {
            try
            {
                var result = await _invoiceService.AddCreditNoteToInvoice(invoiceNumber, (int)request.CreditNoteAmount);

                if (!result.Success)
                {
                    return BadRequest(new { success = false, message = result.Message, errors = result.Errors });
                }

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al agregar la nota de crédito.", errors = new List<string> { ex.Message } });
            }
        }

    }
}
