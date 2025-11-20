using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using SistemaFinanciero.API.Services;

namespace SistemaFinanciero.Tests
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<SistemaFinanciero.API.Program>>
    {
        private readonly WebApplicationFactory<SistemaFinanciero.API.Program> _factory;

        public IntegrationTests(WebApplicationFactory<SistemaFinanciero.API.Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Post_Completo_ReturnsComputedResult()
        {
            var client = _factory.CreateClient();

            var payload = new AnalisisMultiPeriodoDTO
            {
                PeriodoActual = new DatosPeriodoDTO
                {
                    Ventas = 100m,
                    UtilidadNeta = 400m,
                    Depreciacion = 12m,
                    Efectivo = 900m,
                    CuentasPorCobrar = 1000m,
                    Inventario = 12m,
                    ActivoCorriente = 12m,
                    CuentasPorPagar = 12m,
                    PasivoCorriente = 2300m,
                    ActivoTotal = 12000m,
                    PasivoTotal = 130000m,
                    Patrimonio = 890000m
                },
                PeriodoAnterior = new DatosPeriodoDTO
                {
                    Ventas = 200m,
                    UtilidadNeta = 200m,
                    Depreciacion = 32m,
                    Efectivo = 20m,
                    CuentasPorCobrar = 90000m,
                    Inventario = 12m,
                    ActivoCorriente = 23m,
                    CuentasPorPagar = 12m,
                    PasivoCorriente = 500m,
                    ActivoTotal = 300m,
                    PasivoTotal = 1290m,
                    Patrimonio = 20000m
                }
            };

            var response = await client.PostAsJsonAsync("/api/analisis/completo", payload);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ResultadoAnalisisCompleto>();
            Assert.NotNull(result);
            Assert.NotNull(result.DuPont);
            Assert.NotNull(result.Razones);
            Assert.NotNull(result.Vertical);
            Assert.NotNull(result.Horizontal);
            Assert.NotNull(result.FlujoEfectivo);

            // Basic sanity: horizontal rows count and flujo operativo calculation
            Assert.Equal(5, result.Horizontal.Variaciones.Count);
            // Compute expected flujo operativo and compare
            var expectedOper = result.FlujoEfectivo.UtilidadNeta + result.FlujoEfectivo.Depreciacion + result.FlujoEfectivo.VariacionCuentasPorCobrar + result.FlujoEfectivo.VariacionInventario + result.FlujoEfectivo.VariacionCuentasPorPagar;
            Assert.Equal(expectedOper, result.FlujoEfectivo.FlujoOperativo);
        }
    }
}
