using Microsoft.AspNetCore.Mvc;
using SistemaFinanciero.API.Services;
using System.Text.Json;

namespace SistemaFinanciero.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalisisController : ControllerBase
    {
        private readonly CalculosService _service;

        // Inyección de dependencias: El constructor recibe el servicio registrado en Program.cs
        public AnalisisController(CalculosService service)
        {
            _service = service;
        }

        // Endpoint único que devuelve TODOS los análisis
        [HttpPost("completo")]
        public async Task<ActionResult<ResultadoAnalisisCompleto>> Calcular([FromBody] AnalisisMultiPeriodoDTO d)
        {
            // Model binding sometimes fails depending on Content-Type or client behavior.
            // If binding produced null or missing periods, try to read and deserialize the raw body
            if (d == null || d.PeriodoActual == null || d.PeriodoAnterior == null)
            {
                try
                {
                    Request.Body.Position = 0;
                }
                catch { }

                using var reader = new System.IO.StreamReader(Request.Body);
                var raw = await reader.ReadToEndAsync();
                if (!string.IsNullOrWhiteSpace(raw))
                {
                    try
                    {
                        var opts = new System.Text.Json.JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        d = System.Text.Json.JsonSerializer.Deserialize<AnalisisMultiPeriodoDTO>(raw, opts);
                    }
                    catch
                    {
                        // ignore and return error below
                    }
                }
            }

            if (d == null || d.PeriodoActual == null || d.PeriodoAnterior == null)
            {
                return BadRequest("Faltan datos del período actual o anterior.");
            }

            // Calcular paso a paso y registrar en consola para depuración
            var dupont = _service.CalcularDuPont(d.PeriodoActual);
            var razones = _service.CalcularRazones(d.PeriodoActual);
            var vertical = _service.CalcularVertical(d.PeriodoActual);
            var horizontal = _service.CalcularHorizontal(d.PeriodoActual, d.PeriodoAnterior);
            var flujo = _service.CalcularFlujo(d.PeriodoActual, d.PeriodoAnterior);

            var resultado = new ResultadoAnalisisCompleto
            {
                DuPont = dupont,
                Razones = razones,
                Vertical = vertical,
                Horizontal = horizontal,
                FlujoEfectivo = flujo
            };

            try
            {
                Console.WriteLine("[AnalisisController] Entrada: " + JsonSerializer.Serialize(d));
                Console.WriteLine("[AnalisisController] Resultado: " + JsonSerializer.Serialize(resultado));
            }
            catch { }

            return Ok(resultado);
        }

        // Debug endpoint: devuelve el body tal como llegó (útil para ver el JSON recibido)
        [HttpPost("completo-debug")]
        public async Task<IActionResult> CalcularDebug()
        {
            using var reader = new System.IO.StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            return Ok(new { received = body });
        }
    }
}