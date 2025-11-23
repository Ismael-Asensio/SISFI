using System;
using System.Collections.Generic;

namespace SistemaFinanciero.API.Services
{
    // --- DTOs (Estructuras de Datos de Entrada) ---
    public class DatosPeriodoDTO
    {
        public decimal Ventas { get; set; }
        public decimal UtilidadNeta { get; set; }
        public decimal Depreciacion { get; set; }
        public decimal Efectivo { get; set; }
        public decimal CuentasPorCobrar { get; set; }
        public decimal Inventario { get; set; }
        public decimal ActivoCorriente { get; set; }
        public decimal CuentasPorPagar { get; set; }
        public decimal PasivoCorriente { get; set; }
        public decimal ActivoTotal { get; set; }
        public decimal PasivoTotal { get; set; }
        public decimal Patrimonio { get; set; }
        // Optional inputs that improve ratio calculations when provided
        public decimal CostoBienesVendidos { get; set; }
        public decimal VentasCredito { get; set; }
        public decimal UtilidadOperativa { get; set; }
        public decimal GastoIntereses { get; set; }
    }

    public class AnalisisMultiPeriodoDTO
    {
        public DatosPeriodoDTO PeriodoActual { get; set; }
        public DatosPeriodoDTO PeriodoAnterior { get; set; }
    }

    // --- Modelos de Resultados ---
    public class AnalisisDuPont
    {
        public decimal MargenUtilidad { get; set; }
        public decimal RotacionActivos { get; set; }
        public decimal ApalancamientoFinanciero { get; set; }
        public decimal Roe { get; set; }
    }

    public class ResultadosRazones
    {
        public decimal RazonCorriente { get; set; }
        public decimal NivelEndeudamiento { get; set; }
        public decimal Roe { get; set; }
        public decimal Roa { get; set; }
        public decimal MargenNeto { get; set; }
        // Additional ratios
        public decimal Rapida { get; set; }
        public decimal RotacionInventarios { get; set; }
        public decimal RotacionCuentasPorCobrar { get; set; }
        public decimal PeriodoPromedioCobro { get; set; }
        public decimal RotacionActivosFijos { get; set; }
        public decimal RotacionActivosTotales { get; set; }
        public decimal PasivoSobreCapital { get; set; }
        public decimal CoberturaIntereses { get; set; }
    }

    public class ResultadoAnalisisVertical
    {
        public decimal Pct_ActivoCorriente { get; set; }
        public decimal Pct_Efectivo { get; set; }
        public decimal Pct_CuentasPorCobrar { get; set; }
        public decimal Pct_Inventario { get; set; }
        public decimal Pct_PasivoTotal { get; set; }
        public decimal Pct_Patrimonio { get; set; }
    }

    public class VariacionCuenta
    {
        public string Cuenta { get; set; }
        public decimal ValorActual { get; set; }
        public decimal ValorAnterior { get; set; }
        public decimal VariacionAbsoluta { get; set; }
        public decimal VariacionPorcentual { get; set; }
    }

    public class ResultadoAnalisisHorizontal { public List<VariacionCuenta> Variaciones { get; set; } = new List<VariacionCuenta>(); }

    public class ResultadoFlujoEfectivo
    {
        public decimal UtilidadNeta { get; set; }
        public decimal Depreciacion { get; set; }
        public decimal VariacionCuentasPorCobrar { get; set; }
        public decimal VariacionInventario { get; set; }
        public decimal VariacionCuentasPorPagar { get; set; }
        public decimal FlujoOperativo { get; set; }
        public decimal FlujoInversion { get; set; }
        public decimal FlujoFinanciamiento { get; set; }
        public decimal CambioNetoEnEfectivo { get; set; }
        public decimal EfectivoInicial { get; set; }
        public decimal EfectivoFinal { get; set; }
    }

    public class ResultadoAnalisisCompleto
    {
        public AnalisisDuPont DuPont { get; set; }
        public ResultadosRazones Razones { get; set; }
        public ResultadoAnalisisVertical Vertical { get; set; }
        public ResultadoAnalisisHorizontal Horizontal { get; set; }
        public ResultadoFlujoEfectivo FlujoEfectivo { get; set; }
    }

    // --- Lógica de Negocio ---
    public class CalculosService
    {
        public AnalisisDuPont CalcularDuPont(DatosPeriodoDTO d)
        {
            var r = new AnalisisDuPont();
            // Calcular en decimales y luego convertir a porcentajes donde corresponde
            decimal margen = 0m;
            decimal rotacion = 0m;
            decimal apalanc = 0m;
            if (d.Ventas > 0) margen = d.UtilidadNeta / d.Ventas; // decimal (e.g., 0.25)
            if (d.ActivoTotal > 0) rotacion = d.Ventas / d.ActivoTotal; // veces
            if (d.Patrimonio > 0) apalanc = d.ActivoTotal / d.Patrimonio; // ratio

            // Exponer Margen y ROE en porcentajes
            r.MargenUtilidad = margen * 100m;
            r.RotacionActivos = rotacion;
            r.ApalancamientoFinanciero = apalanc;
            r.Roe = (margen * rotacion * apalanc) * 100m;
            return r;
        }

        // Compute liquidity, activity, leverage and profitability ratios.
        // If 'ant' (previous period) is provided, averages will be used where appropriate
        public ResultadosRazones CalcularRazones(DatosPeriodoDTO d, DatosPeriodoDTO ant = null)
        {
            var r = new ResultadosRazones();
            // Liquidity
            if (d.PasivoCorriente > 0) r.RazonCorriente = d.ActivoCorriente / d.PasivoCorriente;
            // Quick ratio (acid test)
            if (d.PasivoCorriente > 0) r.Rapida = (d.ActivoCorriente - d.Inventario) / d.PasivoCorriente;

            // Leverage & profitability
            if (d.ActivoTotal > 0) { r.NivelEndeudamiento = (d.PasivoTotal / d.ActivoTotal) * 100m; r.Roa = (d.UtilidadNeta / d.ActivoTotal) * 100m; }
            if (d.Patrimonio > 0) r.Roe = (d.UtilidadNeta / d.Patrimonio) * 100m;
            if (d.Ventas > 0) r.MargenNeto = (d.UtilidadNeta / d.Ventas) * 100m;

            // Pasivo / Capital
            if (d.Patrimonio > 0) r.PasivoSobreCapital = d.PasivoTotal / d.Patrimonio;

            // Activity ratios that can use previous period averages when provided
            // Inventory turnover = COGS / Average Inventory. If COGS not provided, fall back to Sales.
            decimal cogs = d.CostoBienesVendidos;
            if (cogs <= 0) cogs = d.Ventas; // fallback
            if (ant != null)
            {
                var avgInv = (d.Inventario + ant.Inventario) / 2m;
                if (avgInv > 0) r.RotacionInventarios = cogs / avgInv;

                // Receivables turnover: use VentasCredito if present, else Ventas
                var ventasCred = d.VentasCredito > 0 ? d.VentasCredito : d.Ventas;
                var avgCxc = (d.CuentasPorCobrar + ant.CuentasPorCobrar) / 2m;
                if (avgCxc > 0) r.RotacionCuentasPorCobrar = ventasCred / avgCxc;
                if (r.RotacionCuentasPorCobrar > 0) r.PeriodoPromedioCobro = 360m / r.RotacionCuentasPorCobrar;

                // Fixed assets turnover: sales / average fixed assets. Compute fixed assets = activoTotal - activoCorriente
                var actFijosAct = d.ActivoTotal - d.ActivoCorriente;
                var actFijosAnt = ant.ActivoTotal - ant.ActivoCorriente;
                var avgActFijos = (actFijosAct + actFijosAnt) / 2m;
                if (avgActFijos > 0) r.RotacionActivosFijos = d.Ventas / avgActFijos;

                // Total asset turnover: sales / average total assets
                var avgActTotal = (d.ActivoTotal + ant.ActivoTotal) / 2m;
                if (avgActTotal > 0) r.RotacionActivosTotales = d.Ventas / avgActTotal;
            }
            else
            {
                // Without previous period we compute simple versions where possible
                if (d.Inventario > 0) r.RotacionInventarios = (cogs > 0 && d.Inventario > 0) ? (cogs / d.Inventario) : 0m;
                if (d.CuentasPorCobrar > 0) { var ventasCred = d.VentasCredito > 0 ? d.VentasCredito : d.Ventas; if (ventasCred > 0) r.RotacionCuentasPorCobrar = ventasCred / d.CuentasPorCobrar; if (r.RotacionCuentasPorCobrar > 0) r.PeriodoPromedioCobro = 360m / r.RotacionCuentasPorCobrar; }
                var actFijos = d.ActivoTotal - d.ActivoCorriente; if (actFijos > 0) r.RotacionActivosFijos = d.Ventas / actFijos;
                if (d.ActivoTotal > 0) r.RotacionActivosTotales = d.Ventas / d.ActivoTotal;
            }

            // Interest coverage (operating profit / interest expense) if data present
            if (d.GastoIntereses > 0 && d.UtilidadOperativa > 0) r.CoberturaIntereses = d.UtilidadOperativa / d.GastoIntereses;
            return r;
        }

        public ResultadoAnalisisVertical CalcularVertical(DatosPeriodoDTO d)
        {
            var r = new ResultadoAnalisisVertical();
            if (d.ActivoTotal > 0)
            {
                r.Pct_ActivoCorriente = (d.ActivoCorriente / d.ActivoTotal) * 100;
                r.Pct_Efectivo = (d.Efectivo / d.ActivoTotal) * 100;
                r.Pct_CuentasPorCobrar = (d.CuentasPorCobrar / d.ActivoTotal) * 100;
                r.Pct_Inventario = (d.Inventario / d.ActivoTotal) * 100;
                decimal basePP = d.PasivoTotal + d.Patrimonio;
                if (basePP > 0) { r.Pct_PasivoTotal = (d.PasivoTotal / basePP) * 100; r.Pct_Patrimonio = (d.Patrimonio / basePP) * 100; }
            }
            return r;
        }

        public ResultadoAnalisisHorizontal CalcularHorizontal(DatosPeriodoDTO act, DatosPeriodoDTO ant)
        {
            var r = new ResultadoAnalisisHorizontal();
            string[] nombres = { "Ventas", "Utilidad Neta", "Activo Total", "Pasivo Total", "Patrimonio" };
            decimal[] vAct = { act.Ventas, act.UtilidadNeta, act.ActivoTotal, act.PasivoTotal, act.Patrimonio };
            decimal[] vAnt = { ant.Ventas, ant.UtilidadNeta, ant.ActivoTotal, ant.PasivoTotal, ant.Patrimonio };

            for (int i = 0; i < nombres.Length; i++)
            {
                var v = new VariacionCuenta { Cuenta = nombres[i], ValorActual = vAct[i], ValorAnterior = vAnt[i] };
                v.VariacionAbsoluta = vAct[i] - vAnt[i];
                v.VariacionPorcentual = vAnt[i] != 0 ? (v.VariacionAbsoluta / vAnt[i]) * 100 : 0;
                r.Variaciones.Add(v);
            }
            return r;
        }

        public ResultadoFlujoEfectivo CalcularFlujo(DatosPeriodoDTO act, DatosPeriodoDTO ant)
        {
            var r = new ResultadoFlujoEfectivo();
            // Operativo
            r.UtilidadNeta = act.UtilidadNeta;
            r.Depreciacion = act.Depreciacion;
            r.VariacionCuentasPorCobrar = -(act.CuentasPorCobrar - ant.CuentasPorCobrar);
            r.VariacionInventario = -(act.Inventario - ant.Inventario);
            r.VariacionCuentasPorPagar = act.CuentasPorPagar - ant.CuentasPorPagar;
            r.FlujoOperativo = r.UtilidadNeta + r.Depreciacion + r.VariacionCuentasPorCobrar + r.VariacionInventario + r.VariacionCuentasPorPagar;

            // Inversión (Estimación CAPEX)
            decimal capex = (act.ActivoTotal - act.ActivoCorriente) - (ant.ActivoTotal - ant.ActivoCorriente) + act.Depreciacion;
            r.FlujoInversion = -capex;

            // Financiamiento (Plug)
            r.EfectivoInicial = ant.Efectivo;
            r.EfectivoFinal = act.Efectivo;
            r.CambioNetoEnEfectivo = r.EfectivoFinal - r.EfectivoInicial;
            r.FlujoFinanciamiento = r.CambioNetoEnEfectivo - r.FlujoOperativo - r.FlujoInversion;
            return r;
        }
    }
}