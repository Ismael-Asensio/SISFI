import React from 'react';

const RazonesFinancieras = ({ datos }) => {
  if (!datos) return null;

  const safe = (v) => (v ?? 0);
  const fmtPct = (val) => `${Number(safe(val)).toFixed(2)}%`;
  const fmtNum = (val) => Number(safe(val)).toFixed(2);

  return (
    <div className="card">
      <h3>📈 Razones Financieras Clave</h3>
      <table>
        <thead>
          <tr>
            <th>Indicador</th>
            <th>Resultado</th>
            <th>Interpretación</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>Razón Corriente</td>
            <td className="text-bold">{fmtNum(datos.razonCorriente)}</td>
            <td>Capacidad pago corto plazo</td>
          </tr>
          <tr>
            <td>Nivel Endeudamiento</td>
            <td className="text-bold">{fmtPct(datos.nivelEndeudamiento)}</td>
            <td>% Activos financiados por deuda</td>
          </tr>
          <tr>
            <td>ROE (Rentabilidad Patrimonio)</td>
            <td className="text-bold text-success">{fmtPct(datos.roe)}</td>
            <td>Retorno para los accionistas</td>
          </tr>
          <tr>
            <td>ROA (Rentabilidad Activos)</td>
            <td className="text-bold">{fmtPct(datos.roa)}</td>
            <td>Eficiencia de activos totales</td>
          </tr>
          <tr>
            <td>Margen Neto</td>
            <td className="text-bold">{fmtPct(datos.margenNeto)}</td>
            <td>Ganancia por cada $ vendido</td>
          </tr>
        </tbody>
      </table>
    </div>
  );
};

export default RazonesFinancieras;
