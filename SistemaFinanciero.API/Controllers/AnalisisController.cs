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
        public async Task<ActionResult<ResultadoAnalisisCompleto>> Calcular()
        {
            // Read the raw request body and parse it ourselves (avoid model-binding consuming the stream)
            try { Request.EnableBuffering(); } catch { }
            string rawBody;
            using (var reader = new System.IO.StreamReader(Request.Body, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true))
            {
                rawBody = await reader.ReadToEndAsync();
                Request.Body.Position = 0;
            }

            if (string.IsNullOrWhiteSpace(rawBody))
            {
                return BadRequest("Faltan datos del período actual o anterior (body vacío).");
            }

            AnalisisMultiPeriodoDTO parsed = null;
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(rawBody);
                var root = doc.RootElement;

                bool TryGetProp(System.Text.Json.JsonElement el, string name, out System.Text.Json.JsonElement found)
                {
                    foreach (var p in el.EnumerateObject())
                    {
                        if (string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))
                        {
                            found = p.Value;
                            return true;
                        }
                    }
                    found = default;
                    return false;
                }

                decimal ReadDecimal(System.Text.Json.JsonElement parent, string name)
                {
                    if (TryGetProp(parent, name, out var v))
                    {
                        try
                        {
                            if (v.ValueKind == System.Text.Json.JsonValueKind.Number)
                            {
                                try { return v.GetDecimal(); }
                                catch
                                {
                                    try { return Convert.ToDecimal(v.GetDouble()); } catch { return 0m; }
                                }
                            }
                            if (v.ValueKind == System.Text.Json.JsonValueKind.String)
                            {
                                if (decimal.TryParse(v.GetString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var r))
                                    return r;
                            }
                        }
                        catch { }
                    }
                    return 0m;
                }

                DatosPeriodoDTO ReadPeriodo(System.Text.Json.JsonElement rootEl, string periodoName)
                {
                    var pDto = new DatosPeriodoDTO();
                    if (TryGetProp(rootEl, periodoName, out var pEl))
                    {
                        pDto.Ventas = ReadDecimal(pEl, "ventas");
                        pDto.UtilidadNeta = ReadDecimal(pEl, "utilidadNeta");
                        pDto.Depreciacion = ReadDecimal(pEl, "depreciacion");
                        pDto.Efectivo = ReadDecimal(pEl, "efectivo");
                        pDto.CuentasPorCobrar = ReadDecimal(pEl, "cuentasPorCobrar");
                        pDto.Inventario = ReadDecimal(pEl, "inventario");
                        pDto.ActivoCorriente = ReadDecimal(pEl, "activoCorriente");
                        pDto.CuentasPorPagar = ReadDecimal(pEl, "cuentasPorPagar");
                        pDto.PasivoCorriente = ReadDecimal(pEl, "pasivoCorriente");
                        pDto.ActivoTotal = ReadDecimal(pEl, "activoTotal");
                        pDto.PasivoTotal = ReadDecimal(pEl, "pasivoTotal");
                        pDto.Patrimonio = ReadDecimal(pEl, "patrimonio");
                        // Optional fields
                        pDto.CostoBienesVendidos = ReadDecimal(pEl, "costoBienesVendidos");
                        pDto.VentasCredito = ReadDecimal(pEl, "ventasCredito");
                        pDto.UtilidadOperativa = ReadDecimal(pEl, "utilidadOperativa");
                        pDto.GastoIntereses = ReadDecimal(pEl, "gastoIntereses");
                    }
                    return pDto;
                }

                parsed = new AnalisisMultiPeriodoDTO
                {
                    PeriodoActual = ReadPeriodo(root, "periodoActual"),
                    PeriodoAnterior = ReadPeriodo(root, "periodoAnterior")
                };
            }
            catch (System.Text.Json.JsonException)
            {
                return BadRequest("JSON inválido en el cuerpo de la solicitud.");
            }

            if (parsed == null || parsed.PeriodoActual == null || parsed.PeriodoAnterior == null)
            {
                return BadRequest("Faltan datos del período actual o anterior (parsing).");
            }

            // Log the parsed DTO for debugging
            try { Console.WriteLine("[AnalisisController] Parsed DTO: " + System.Text.Json.JsonSerializer.Serialize(parsed)); } catch { }

            // Calcular paso a paso y registrar en consola para depuración
            var dupont = _service.CalcularDuPont(parsed.PeriodoActual);
            var razones = _service.CalcularRazones(parsed.PeriodoActual, parsed.PeriodoAnterior);
            var vertical = _service.CalcularVertical(parsed.PeriodoActual);
            var horizontal = _service.CalcularHorizontal(parsed.PeriodoActual, parsed.PeriodoAnterior);
            var flujo = _service.CalcularFlujo(parsed.PeriodoActual, parsed.PeriodoAnterior);

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
                Console.WriteLine("[AnalisisController] Entrada: " + JsonSerializer.Serialize(parsed));
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