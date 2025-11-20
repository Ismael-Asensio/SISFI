import React from 'react';
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';
import { Doughnut } from 'react-chartjs-2';

ChartJS.register(ArcElement, Tooltip, Legend);

const Dashboard = ({ datos }) => {
  if (!datos) return null;

  const safe = (v) => (v ?? 0);
  const safePct = (v) => Number(safe(v)).toFixed(2);
  const safeNum = (v) => Number(safe(v)).toFixed(2);

  const chartData = {
    labels: ['Margen Utilidad', 'Rotación Activos', 'Apalancamiento'],
    datasets: [
      {
        data: [
          Number(safe(datos.margenUtilidad)),
          Number(safe(datos.rotacionActivos)),
          Number(safe(datos.apalancamientoFinanciero))
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
          <p><strong>ROE Total:</strong> <span style={{ fontSize: '1.5em', color: '#2563eb' }}>{safePct(datos.roe)}%</span></p>
          <ul style={{ listStyle: 'none', padding: 0 }}>
            <li>🔹 Margen: {safePct(datos.margenUtilidad)}%</li>
            <li>🔹 Rotación: {safeNum(datos.rotacionActivos)}x</li>
            <li>🔹 Apalancamiento: {safeNum(datos.apalancamientoFinanciero)}</li>
          </ul>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
