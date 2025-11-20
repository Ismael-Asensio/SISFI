const API_BASE = process.env.REACT_APP_API_URL || 'http://localhost:5272/api';

export async function postAnalisis(body) {
  const res = await fetch(`${API_BASE}/analisis/completo`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  });
  if (!res.ok) throw new Error('API request failed: ' + res.status);
  return res.json();
}

export default { postAnalisis };
