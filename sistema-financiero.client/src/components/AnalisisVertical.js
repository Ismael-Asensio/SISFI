import React from 'react';

const AnalisisVertical = ({ datos }) => {
  if (!datos) return null;

  const fmt = (val) => {
    const n = Number(val ?? 0);
    return `${Number.isFinite(n) ? n.toFixed(2) : '0.00'}%`;
  };

  return (
    <div className="card">
      <h3>📊 Análisis Vertical (Estructura)</h3>
      <div className="grid-2">
        <div>
          <h4>Activos (Sobre Activo Total)</h4>
          <ul>
            <li>Activo Corriente: <strong>{fmt(datos.pct_ActivoCorriente)}</strong></li>
            <li>Efectivo: {fmt(datos.pct_Efectivo)}</li>
            <li>Cuentas por Cobrar: {fmt(datos.pct_CuentasPorCobrar)}</li>
            <li>Inventario: {fmt(datos.pct_Inventario)}</li>
          </ul>
        </div>
        <div>
          <h4>Pasivos y Patrimonio</h4>
          <ul>
            <li>Pasivo Total: <strong>{fmt(datos.pct_PasivoTotal)}</strong></li>
            <li>Patrimonio: <strong>{fmt(datos.pct_Patrimonio)}</strong></li>
          </ul>
        </div>
      </div>
    </div>
  );
};

export default AnalisisVertical;
