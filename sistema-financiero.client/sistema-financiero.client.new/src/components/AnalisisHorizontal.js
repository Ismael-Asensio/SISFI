import React from 'react';

const AnalisisHorizontal = ({ datos }) => {
  if (!datos || !datos.variaciones) return null;

  const safe = (v) => (v ?? 0);
  const fmtNum = (val) => new Intl.NumberFormat('es-NI', { style: 'currency', currency: 'NIO' }).format(safe(val));
  const fmtPct = (val) => `${Number(safe(val)).toFixed(2)}%`;

  return (
    <div className="card">
      <h3>↔️ Análisis Horizontal (Tendencias)</h3>
      <table>
        <thead>
          <tr>
            <th>Cuenta</th>
            <th>Año Actual</th>
            <th>Año Anterior</th>
            <th>Variación ($)</th>
            <th>Variación (%)</th>
          </tr>
        </thead>
        <tbody>
          {datos.variaciones.map((item, index) => (
            <tr key={index}>
              <td>{item.cuenta}</td>
              <td>{fmtNum(item.valorActual)}</td>
              <td>{fmtNum(item.valorAnterior)}</td>
              <td className={item.variacionAbsoluta >= 0 ? 'text-success' : 'text-danger'}>
                {fmtNum(item.variacionAbsoluta)}
              </td>
              <td className={item.variacionPorcentual >= 0 ? 'text-success' : 'text-danger'}>
                {fmtPct(item.variacionPorcentual)}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default AnalisisHorizontal;
