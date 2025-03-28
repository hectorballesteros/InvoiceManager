using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InvoiceManager.Application.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using InvoiceManager.Api.Models;
using Newtonsoft.Json;

namespace InvoiceManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceImportService _invoiceImportService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(InvoiceImportService invoiceImportService, ILogger<InvoiceController> logger)        {
            _invoiceImportService = invoiceImportService;
            _logger = logger;
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
                
                var request = JsonConvert.DeserializeObject<InvoiceImportRequest>(content);
                _logger.LogInformation("Contenido del archivo deserializado: {RequestContent}", JsonConvert.SerializeObject(request, Formatting.Indented));
                if (request == null || request.Invoices == null || request.Invoices.Count == 0)
                {
                    return BadRequest(new { success = false, message = "El archivo JSON está vacío o no contiene facturas." });
                }

                var result = await _invoiceImportService.ImportInvoicesService(request.Invoices);

                if (!result.Success)
                {
                    return BadRequest(new { success = false, message = result.Message });
                }

                return Ok(new { success = true, data = result.Data, invalidInvoices = result.InvalidInvoices });
            }
        }
    }
}
