import React from 'react';

const FormularioFinanciero = ({ formData, onChange, onSubmit, loading }) => {
  
  // Lista de campos requeridos por el backend
  const campos = [
    { id: 'ventas', label: 'Ventas Netas' },
    { id: 'utilidadNeta', label: 'Utilidad Neta' },
    { id: 'depreciacion', label: 'Depreciación' },
    { id: 'efectivo', label: 'Efectivo y Equivalentes' },
    { id: 'cuentasPorCobrar', label: 'Cuentas por Cobrar' },
    { id: 'inventario', label: 'Inventarios' },
    { id: 'activoCorriente', label: 'Total Activo Corriente' },
    { id: 'cuentasPorPagar', label: 'Cuentas por Pagar' },
    { id: 'pasivoCorriente', label: 'Total Pasivo Corriente' },
    { id: 'activoTotal', label: 'Total Activo' },
    { id: 'pasivoTotal', label: 'Total Pasivo' },
    { id: 'patrimonio', label: 'Total Patrimonio' },
  ];

  const renderInputs = (periodo) => (
    <div>
      <h4 style={{ borderBottom: '2px solid #e2e8f0', paddingBottom: '8px', marginBottom: '16px' }}>
        {periodo === 'periodoActual' ? 'Año Actual (N)' : 'Año Anterior (N-1)'}
      </h4>
      {campos.map((campo) => (
        <div key={`${periodo}-${campo.id}`} style={{ marginBottom: '12px' }}>
          <label style={{ display: 'block', fontSize: '0.9rem', marginBottom: '4px', color: '#64748b' }}>
            {campo.label}
          </label>
          <input
            type="number"
            step="0.01"
            style={{ width: '100%', padding: '8px', borderRadius: '6px', border: '1px solid #cbd5e1' }}
            value={formData[periodo][campo.id]}
            onChange={(e) => onChange(periodo, campo.id, e.target.value)}
            placeholder="0.00"
            required
          />
        </div>
      ))}
    </div>
  );

  return (
    <div className="card">
      <h3>📝 Ingreso de Datos</h3>
      <form onSubmit={onSubmit}>
        <div className="grid-2">
          {renderInputs('periodoActual')}
          {renderInputs('periodoAnterior')}
        </div>
        <button type="submit" className="btn-primary" style={{ marginTop: '20px' }} disabled={loading}>
          {loading ? 'Procesando...' : 'Calcular Análisis Completo'}
        </button>
      </form>
    </div>
  );
};

export default FormularioFinanciero;