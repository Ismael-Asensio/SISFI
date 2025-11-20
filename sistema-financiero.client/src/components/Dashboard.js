import React from 'react';
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';
import { Doughnut } from 'react-chartjs-2';

ChartJS.register(ArcElement, Tooltip, Legend);

const Dashboard = ({ datos }) => {
  if (!datos) return null;

  const toNumber = (v) => {
    const n = Number(v);
    return Number.isFinite(n) ? n : 0;
  };

  const fmtNum = (v, decimals = 2) => toNumber(v).toFixed(decimals);
  const fmtPct = (v, decimals = 2) => `${toNumber(v).toFixed(decimals)}%`;

  const margen = toNumber(datos.margenUtilidad) * 100;
  const rotacion = toNumber(datos.rotacionActivos);
  const apalancamiento = toNumber(datos.apalancamientoFinanciero);
  const roe = toNumber(datos.roe) * 100;

  const chartData = {
    labels: ['Margen Utilidad', 'Rotación Activos', 'Apalancamiento'],
    datasets: [
      {
        data: [
          margen,
          rotacion,
          apalancamiento
        ],
        backgroundColor: [
          'rgba(37, 99, 235, 0.8)', // Azul
          'rgba(16, 185, 129, 0.8)', // Verde
          'rgba(245, 158, 11, 0.8)', // Naranja
        ],
        borderWidth: 1,
      },
    ],
  };

  return (
    <div className="card">
      <h3>🧬 Sistema DuPont (Desglose del ROE)</h3>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-around', flexWrap: 'wrap' }}>
        <div style={{ width: '250px', height: '250px' }}>
          <Doughnut data={chartData} options={{ maintainAspectRatio: false }} />
        </div>
        <div>
          <p><strong>ROE Total:</strong> <span style={{ fontSize: '1.5em', color: '#2563eb' }}>{fmtPct(roe)}</span></p>
          <ul style={{ listStyle: 'none', padding: 0 }}>
            <li>🔹 Margen: {fmtPct(margen)}</li>
            <li>🔹 Rotación: {fmtNum(rotacion)}x</li>
            <li>🔹 Apalancamiento: {fmtNum(apalancamiento)}</li>
          </ul>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;