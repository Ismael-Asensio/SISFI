import axios from 'axios';

const API_BASE = process.env.REACT_APP_API_URL || 'http://localhost:5272/api';

const apiClient = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' }
});

export const calcularAnalisisCompleto = (datos) => apiClient.post('/analisis/completo', datos);

export default apiClient;
