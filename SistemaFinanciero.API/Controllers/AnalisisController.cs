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
        public ActionResult<ResultadoAnalisisCompleto> Calcular([FromBody] AnalisisMultiPeriodoDTO d)
        {
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
    }
}