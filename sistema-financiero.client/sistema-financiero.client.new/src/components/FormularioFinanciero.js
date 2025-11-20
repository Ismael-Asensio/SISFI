import React from 'react';

const FormularioFinanciero = ({ onSubmit }) => {
  // Minimal placeholder form to allow the app to render if original form lost
  const handleSubmit = (e) => { e.preventDefault(); if (onSubmit) onSubmit(); };
  return (
    <form onSubmit={handleSubmit} style={{ padding: 10 }}>
      <p>Formulario financiero (restaurado - versión mínima).</p>
      <button type="submit">Calcular</button>
    </form>
  );
};

export default FormularioFinanciero;
