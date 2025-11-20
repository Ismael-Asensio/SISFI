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

        public ResultadosRazones CalcularRazones(DatosPeriodoDTO d)
        {
            var r = new ResultadosRazones();
            if (d.PasivoCorriente > 0) r.RazonCorriente = d.ActivoCorriente / d.PasivoCorriente;
            if (d.ActivoTotal > 0) { r.NivelEndeudamiento = (d.PasivoTotal / d.ActivoTotal) * 100m; r.Roa = (d.UtilidadNeta / d.ActivoTotal) * 100m; }
            if (d.Patrimonio > 0) r.Roe = (d.UtilidadNeta / d.Patrimonio) * 100m;
            if (d.Ventas > 0) r.MargenNeto = (d.UtilidadNeta / d.Ventas) * 100m;
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