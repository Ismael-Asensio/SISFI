import React from 'react';

const FlujoDeEfectivo = ({ datos }) => {
  if (!datos) return null;

  const fmt = (val) => {
    const n = Number(val ?? 0);
    return new Intl.NumberFormat('es-NI', { style: 'currency', currency: 'NIO' }).format(Number.isFinite(n) ? n : 0);
  };

  const Fila = ({ label, valor, bold = false, color = 'inherit' }) => (
    <div style={{ display: 'flex', justifyContent: 'space-between', padding: '8px 0', borderBottom: '1px dashed #eee', fontWeight: bold ? 'bold' : 'normal', color }}>
      <span>{label}</span>
      <span>{fmt(valor)}</span>
    </div>
  );

  return (
    <div className="card" style={{ backgroundColor: '#f8fff9', borderColor: '#bbf7d0' }}>
      <h3>💵 Estado de Flujo de Efectivo (Método Indirecto)</h3>
      
      <h4 style={{ marginTop: '15px', color: '#15803d' }}>Actividades Operativas</h4>
      <Fila label="Utilidad Neta" valor={datos.utilidadNeta} />
      <Fila label="+ Depreciación" valor={datos.depreciacion} />
      <Fila label="(Δ) Cuentas por Cobrar" valor={datos.variacionCuentasPorCobrar} />
      <Fila label="(Δ) Inventario" valor={datos.variacionInventario} />
      <Fila label="(Δ) Cuentas por Pagar" valor={datos.variacionCuentasPorPagar} />
      <Fila label="Flujo Operativo Neto" valor={datos.flujoOperativo} bold />

      <h4 style={{ marginTop: '15px', color: '#15803d' }}>Actividades de Inversión</h4>
      <Fila label="CAPEX (Inversión Activos Fijos)" valor={datos.flujoInversion} />
      
      <h4 style={{ marginTop: '15px', color: '#15803d' }}>Actividades de Financiamiento</h4>
      <Fila label="Financiamiento Neto (Plug)" valor={datos.flujoFinanciamiento} />

      <div style={{ marginTop: '20px', padding: '15px', backgroundColor: '#dcfce7', borderRadius: '8px' }}>
        <Fila label="Variación Neta en Efectivo" valor={datos.cambioNetoEnEfectivo} bold />
        <Fila label="Efectivo Inicial" valor={datos.efectivoInicial} />
        <Fila label="Efectivo Final Calculado" valor={datos.efectivoFinal} bold color="#166534" />
      </div>
    </div>
  );
};

export default FlujoDeEfectivo;
