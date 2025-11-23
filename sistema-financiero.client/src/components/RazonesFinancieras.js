import React from 'react';

const RazonesFinancieras = ({ datos }) => {
  if (!datos) return null;

  const toNumber = (v) => {
    const n = Number(v);
    return Number.isFinite(n) ? n : 0;
  };

  const fmtNum = (val, decimals = 2) => {
    const n = toNumber(val);
    return n.toLocaleString('es-NI', { minimumFractionDigits: decimals, maximumFractionDigits: decimals });
  };

  const fmtMaybe = (val, decimals = 2, suffix = '') => {
    const n = toNumber(val);
    if (!Number.isFinite(n) || n === 0) return 'N/A';
    return `${n.toLocaleString('es-NI', { minimumFractionDigits: decimals, maximumFractionDigits: decimals })}${suffix}`;
  };

  const fmtPct = (val, decimals = 2) => {
    const n = toNumber(val * 100);
    return `${n.toLocaleString('es-NI', { minimumFractionDigits: decimals, maximumFractionDigits: decimals })}%`;
  };

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
            <td>Razón Rápida (Acid Test)</td>
            <td className="text-bold">{fmtMaybe(datos.rapida)}</td>
            <td>Activos líquidos excluyendo inventario</td>
          </tr>
          <tr>
            <td>Nivel Endeudamiento</td>
            <td className="text-bold">{fmtPct(datos.nivelEndeudamiento)}</td>
            <td>% Activos financiados por deuda</td>
          </tr>
          <tr>
            <td>Pasivo / Capital</td>
            <td className="text-bold">{fmtMaybe(datos.pasivoSobreCapital)}</td>
            <td>Proporción deuda vs patrimonio</td>
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
          <tr>
            <td>Rotación de Inventarios</td>
            <td className="text-bold">{fmtMaybe(datos.rotacionInventarios, 2, 'x')}</td>
            <td>Eficiencia en venta/reposición de inventarios</td>
          </tr>
          <tr>
            <td>Rotación Cuentas por Cobrar</td>
            <td className="text-bold">{fmtMaybe(datos.rotacionCuentasPorCobrar, 2, 'x')}</td>
            <td>Eficacia de cobros</td>
          </tr>
          <tr>
            <td>Periodo Promedio de Cobro</td>
            <td className="text-bold">{(() => {
              const n = Number(datos.periodoPromedioCobro);
              if (!Number.isFinite(n) || n <= 0 || n > 10000) return 'N/A';
              return `${n.toLocaleString('es-NI', { maximumFractionDigits: 1 })} días`;
            })()}</td>
            <td>Tiempo promedio de cobro</td>
          </tr>
          <tr>
            <td>Rotación Activos Fijos</td>
            <td className="text-bold">{fmtMaybe(datos.rotacionActivosFijos, 2, 'x')}</td>
            <td>Eficiencia uso activos fijos</td>
          </tr>
          <tr>
            <td>Rotación Activos Totales</td>
            <td className="text-bold">{fmtMaybe(datos.rotacionActivosTotales, 2, 'x')}</td>
            <td>Eficiencia uso de activos totales</td>
          </tr>
          <tr>
            <td>Cobertura de Intereses</td>
            <td className="text-bold">{fmtMaybe(datos.coberturaIntereses, 2, 'x')}</td>
            <td>Capacidad para pagar intereses</td>
          </tr>
        </tbody>
      </table>
    </div>
  );
};

export default RazonesFinancieras;