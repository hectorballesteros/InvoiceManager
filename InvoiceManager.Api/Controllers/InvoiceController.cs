using Microsoft.AspNetCore.Mvc;
using InvoiceManager.Application.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using InvoiceManager.Api.Models;
using System.Text.Json; // Usamos System.Text.Json

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
            var invoices = await _invoiceService.GetAllInvoices();

            return Ok(new
            {
                success = true,
                data = invoices
            });
        }

        // Endpoint para importar facturas desde un archivo JSON
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

            using (var stream = new StreamReader(file.OpenReadStream()))
            {
                var content = await stream.ReadToEndAsync();
                
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
    }
}
