import React, { useState } from 'react';
import './index.css'; // Importamos los estilos globales

// Importación de componentes modulares
import FormularioFinanciero from './components/FormularioFinanciero';
import Dashboard from './components/Dashboard';
import RazonesFinancieras from './components/RazonesFinancieras';
import AnalisisVertical from './components/AnalisisVertical';
import AnalisisHorizontal from './components/AnalisisHorizontal';
import FlujoDeEfectivo from './components/FlujoDeEfectivo';

// Importación del servicio API
import { calcularAnalisisCompleto } from './services/apiService';

const App = () => {
  // Estado inicial vacío
  const emptyData = {
    ventas: '', utilidadNeta: '', depreciacion: '', efectivo: '',
    cuentasPorCobrar: '', inventario: '', activoCorriente: '',
    cuentasPorPagar: '', pasivoCorriente: '', activoTotal: '',
    pasivoTotal: '', patrimonio: ''
  };

  const [formData, setFormData] = useState({
    periodoActual: { ...emptyData },
    periodoAnterior: { ...emptyData }
  });

  const [resultados, setResultados] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // Manejar cambios en los inputs del formulario
  const handleInputChange = (periodo, campo, valor) => {
    setFormData(prev => ({
      ...prev,
      [periodo]: { ...prev[periodo], [campo]: valor }
    }));
  };

  // Enviar datos a la API
  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    // Convertir strings a números
    const parsePeriodo = (p) => {
      const obj = {};
      for (const key in p) obj[key] = parseFloat(p[key] || 0);
      return obj;
    };

    try {
      const payload = {
        periodoActual: parsePeriodo(formData.periodoActual),
        periodoAnterior: parsePeriodo(formData.periodoAnterior)
      };

      const response = await calcularAnalisisCompleto(payload);
      setResultados(response.data);
    } catch (err) {
      console.error(err);
      setError("Error al conectar con el servidor. Verifique que la API (.NET) esté corriendo en el puerto 7237.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container">
      <header style={{ textAlign: 'center', marginBottom: '40px' }}>
        <h1>🚀 Sistema Integral de Análisis Financiero</h1>
        <p style={{ color: '#64748b' }}>Herramienta para MIPYMES: Vertical, Horizontal, DuPont y Flujo de Efectivo</p>
      </header>

      <div className="grid-layout">
        {/* Columna Izquierda: Formulario */}
        <div className="col-form">
          <FormularioFinanciero 
            formData={formData} 
            onChange={handleInputChange} 
            onSubmit={handleSubmit} 
            loading={loading} 
          />
          {error && (
            <div className="card" style={{ backgroundColor: '#fee2e2', borderColor: '#ef4444', color: '#b91c1c' }}>
              ⚠️ {error}
            </div>
          )}
        </div>

        {/* Columna Derecha: Resultados */}
        <div className="col-results">
          {!resultados ? (
            <div className="card" style={{ textAlign: 'center', color: '#94a3b8', padding: '60px' }}>
              <h3>Esperando datos...</h3>
              <p>Complete la información financiera y presione "Calcular" para ver el reporte completo.</p>
            </div>
          ) : (
            <>
              <Dashboard datos={resultados.duPont} />
              
              <div className="grid-2">
                <RazonesFinancieras datos={resultados.razones} />
                <AnalisisVertical datos={resultados.vertical} />
              </div>

              <AnalisisHorizontal datos={resultados.horizontal} />
              
              <FlujoDeEfectivo datos={resultados.flujoEfectivo} />
            </>
          )}
        </div>
      </div>
    </div>
  );
};

export default App;