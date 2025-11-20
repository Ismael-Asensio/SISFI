using SistemaFinanciero.API.Services;
using Xunit;

namespace SistemaFinanciero.Tests
{
    public class CalculosServiceTests
    {
        private readonly CalculosService _svc = new CalculosService();

        private DatosPeriodoDTO SampleActual() => new DatosPeriodoDTO
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
        };

        private DatosPeriodoDTO SampleAnterior() => new DatosPeriodoDTO
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
        };

        [Fact]
        public void DuPont_ComputesExpectedValues()
        {
            var a = SampleActual();
            var r = _svc.CalcularDuPont(a);
            // MargenUtilidad returned as percent: (400/100) * 100 = 400
            Assert.Equal(400m, decimal.Round(r.MargenUtilidad, 6));
            // RotacionActivos = 100/12000 = 0.008333...
            Assert.InRange(r.RotacionActivos, 0.008333m - 0.000001m, 0.008334m);
            // Apalancamiento = 12000/890000
            var expectedApal = 12000m / 890000m;
            Assert.Equal(decimal.Round(expectedApal, 8), decimal.Round(r.ApalancamientoFinanciero, 8));
            // ROE should be product
            var expectedRoe = r.MargenUtilidad * r.RotacionActivos * r.ApalancamientoFinanciero;
            Assert.Equal(decimal.Round(expectedRoe, 12), decimal.Round(r.ROE, 12));
        }

        [Fact]
        public void Razones_ComputesExpectedValues()
        {
            var a = SampleActual();
            var r = _svc.CalcularRazones(a);
            Assert.InRange(r.RazonCorriente, 0.0052m, 0.006m);
            Assert.Equal(decimal.Round((a.PasivoTotal / a.ActivoTotal) * 100m, 6), decimal.Round(r.NivelEndeudamiento, 6));
            Assert.Equal(decimal.Round((a.UtilidadNeta / a.ActivoTotal) * 100m, 6), decimal.Round(r.ROA, 6));
            Assert.Equal(decimal.Round((a.UtilidadNeta / a.Patrimonio) * 100m, 12), decimal.Round(r.ROE, 12));
            Assert.Equal(decimal.Round(400m, 6), decimal.Round(r.MargenNeto, 6));
        }

        [Fact]
        public void Vertical_ComputesPercentages()
        {
            var a = SampleActual();
            var v = _svc.CalcularVertical(a);
            Assert.Equal(decimal.Round((a.ActivoCorriente / a.ActivoTotal) * 100m, 6), decimal.Round(v.Pct_ActivoCorriente, 6));
            Assert.Equal(decimal.Round((a.Efectivo / a.ActivoTotal) * 100m, 6), decimal.Round(v.Pct_Efectivo, 6));
        }

        [Fact]
        public void Horizontal_ComputesVariations()
        {
            var a = SampleActual();
            var b = SampleAnterior();
            var h = _svc.CalcularHorizontal(a, b);
            // Ventas: 100 vs 200 -> -100 absolute
            var ventas = h.Variaciones.Find(x => x.Cuenta == "Ventas");
            Assert.Equal(100m, ventas.ValorActual);
            Assert.Equal(200m, ventas.ValorAnterior);
            Assert.Equal(-100m, ventas.VariacionAbsoluta);
            Assert.Equal(-50m, ventas.VariacionPorcentual);
        }

        [Fact]
        public void Flujo_ComputesExpected()
        {
            var a = SampleActual();
            var b = SampleAnterior();
            var f = _svc.CalcularFlujo(a, b);
            // FlujoOperativo = utilidad + dep + -(act.cxc - ant.cxc) + -(act.inv - ant.inv) + (act.cxp - ant.cxp)
            var expectedOper = a.UtilidadNeta + a.Depreciacion + (-(a.CuentasPorCobrar - b.CuentasPorCobrar)) + (-(a.Inventario - b.Inventario)) + (a.CuentasPorPagar - b.CuentasPorPagar);
            Assert.Equal(expectedOper, f.FlujoOperativo);
            // Cambio neto en efectivo
            Assert.Equal(a.Efectivo - b.Efectivo, f.CambioNetoEnEfectivo);
        }
    }
}
