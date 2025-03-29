using Microsoft.AspNetCore.Mvc;
using InvoiceManager.Application.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using InvoiceManager.Api.Models;
using System.Text.Json;

namespace InvoiceManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService _invoiceService;

        public InvoiceController(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

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

        [HttpPost("import")]
        public async Task<IActionResult> ImportInvoices([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { success = false, message = "No se ha subido ningún archivo." });
            }

            if (!file.FileName.EndsWith(".json"))
            {
                return BadRequest(new { success = false, message = "El archivo debe ser un archivo JSON." });
            }

            try
            {
                using (var stream = new StreamReader(file.OpenReadStream()))
                {
                    var content = await stream.ReadToEndAsync();
                    
                    // Intentamos deserializar el JSON
                    var request = JsonSerializer.Deserialize<InvoiceImportRequest>(content);

                    if (request == null || request.Invoices == null || request.Invoices.Count == 0)
                    {
                        return BadRequest(new { success = false, message = "El archivo JSON está vacío o no contiene facturas." });
                    }

                    var result = await _invoiceService.ImportInvoices(request.Invoices);

                    if (!result.Success)
                    {
                        return BadRequest(new { success = false, message = result.Message });
                    }

                    return Ok(new { success = true, message = result.Message, errors = result.Errors });
                }
            }
            catch (JsonException ex) // Capturamos la excepción específica de JSON
            {
                // Aquí puedes personalizar el mensaje de error
                return BadRequest(new { success = false, message = "El archivo JSON tiene un formato incorrecto." });
            }
            catch (Exception ex)
            {
                // Captura cualquier otro error inesperado
                return StatusCode(500, new { success = false, message = "Ocurrió un error al procesar el archivo." });
            }
        }

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
