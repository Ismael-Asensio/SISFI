# Sistema Integral de Análisis Financiero (MIPYMES)

Un proyecto full-stack para realizar análisis financieros (DuPont, razones, análisis vertical/horizontal y flujo de efectivo) pensado para MIPYMES. Incluye un backend en .NET 8 y un frontend React (Create React App).

**Estado:** Desarrollo — cambios recientes: mejores validaciones, tolerancia de JSON, indicadores extendidos y visualización en frontend.

---

**Índice rápido**

- **Descripción**
- **Requisitos**
- **Estructura del repositorio**
- **Ejecutar localmente (backend)**
- **Ejecutar localmente (frontend)**
- **API: contrato y ejemplo**
- **Pruebas**
- **Notas de desarrollo y decisiones técnicas**
- **Solución de problemas comunes**
- **Siguientes pasos sugeridos**

---

**Descripción**

Este proyecto provee:
- Un API REST en .NET 8 que calcula múltiples análisis financieros a partir de dos periodos (actual y anterior).
- Un cliente React que permite introducir los datos, llamar al API y visualizar resultados (tablas, gráficos y análisis).

Está pensado para ayudar a pequeñas y medianas empresas a obtener indicadores financieros clave con un workflow sencillo.

---

**Requisitos (local)**

- .NET SDK 8.x
- Node.js (16+ recomendable) y npm (o pnpm/yarn)
- Git
- PowerShell (en Windows, las instrucciones aquí usan `pwsh`)

---

**Estructura del repositorio (resumen)**

- `SistemaFinanciero.API/` — backend (.NET 8)
  - `Program.cs` — arranque, CORS, registro DI y Swagger
  - `Controllers/AnalisisController.cs` — punto de entrada `POST /api/analisis/completo`
  - `Services/CalculosService.cs` — lógica de negocio (DuPont, Razones, Vertical, Horizontal, Flujo)
- `sistema-financiero.client/` — frontend (React, CRA)
  - `src/components` — componentes UI (Formulario, Dashboard, Razones, Vertical, Horizontal, Flujo)
  - `src/services/apiService.js` — cliente axios con `REACT_APP_API_URL` configurable
- `tests/` — pruebas unitarias (CalculosService)

---

**Ejecutar backend (local)**

1. Desde la raíz del repo (PowerShell):

```pwsh
# Compilar
dotnet build SistemaFinanciero.API\SistemaFinanciero.API.csproj

# Ejecutar (escucha por defecto en http://localhost:5272)
dotnet run --project SistemaFinanciero.API
```

Notes:
- `Program.cs` configura `ASPNETCORE_URLS` por defecto a `http://localhost:5272`. Si necesitas otro puerto, exporta `ASPNETCORE_URLS` antes de ejecutar.
- Swagger está activado en `Development` y accesible cuando corres el API en modo desarrollo.

---

**Ejecutar frontend (local)**

```pwsh
cd sistema-financiero.client
npm install   # solo si aún no has instalado dependencias
npm start
```

- La app en desarrollo se conecta a `http://localhost:5272/api` por defecto. Si tu API corre en otra URL, crea un `.env` en `sistema-financiero.client` con:

```
REACT_APP_API_URL=http://localhost:5272/api
```

---

**API: endpoint principal**

- POST `/api/analisis/completo`
  - Input: JSON con `PeriodoActual` y `PeriodoAnterior` (ambos objetos con campos numéricos).
  - El backend acepta nombres en PascalCase o camelCase (es tolerante y case-insensitive).
  - Campos esperados (sugeridos):
    - `ventas`, `utilidadNeta`, `depreciacion`, `efectivo`, `cuentasPorCobrar`, `inventario`, `activoCorriente`, `cuentasPorPagar`, `pasivoCorriente`, `activoTotal`, `pasivoTotal`, `patrimonio`
  - Campos opcionales (mejoran ratios): `costoBienesVendidos`, `ventasCredito`, `utilidadOperativa`, `gastoIntereses`

Ejemplo mínimo de body:

```json
{
  "PeriodoActual": { "Ventas": 100000, "UtilidadNeta": 8000, "Depreciacion": 500, "Efectivo": 12000, "CuentasPorCobrar": 8000, "Inventario": 15000, "ActivoCorriente": 40000, "CuentasPorPagar": 3000, "PasivoCorriente": 15000, "ActivoTotal": 120000, "PasivoTotal": 70000, "Patrimonio": 50000 },
  "PeriodoAnterior": { "Ventas": 90000, "UtilidadNeta": 7000, "Depreciacion": 450, "Efectivo": 10000, "CuentasPorCobrar": 7500, "Inventario": 14000, "ActivoCorriente": 38000, "CuentasPorPagar": 2800, "PasivoCorriente": 14000, "ActivoTotal": 110000, "PasivoTotal": 65000, "Patrimonio": 45000 }
}
```

- Output: `ResultadoAnalisisCompleto` con secciones:
  - `duPont` — margen, rotación, apalancamiento, roe
  - `razones` — razonCorriente, rapida, margen, roa, roe, rotaciones y otros
  - `vertical` — porcentajes por cuenta sobre base (activo total o pasivo+patrimonio según sea aplicable)
  - `horizontal` — lista `variaciones[{cuenta, valorActual, valorAnterior, variacionAbsoluta, variacionPorcentual}]`
  - `flujoEfectivo` — componentes del flujo (operativo, inversión, financiamiento)

---

**Pruebas**

Desde la raíz:

```pwsh
dotnet test
```

Hay pruebas unitarias para `CalculosService` en `tests/` que cubren DuPont, Razones, Vertical, Horizontal y Flujo.

---

**Decisiones técnicas importantes**

- Uso de `decimal` en C# para todo el cálculo financiero (mejor precisión en finanzas que `double`).
- Serialización JSON: `System.Text.Json` con `PropertyNamingPolicy = CamelCase` y `PropertyNameCaseInsensitive = true` para tolerancia entre frontend y backend.
- `CalculosService` contiene toda la lógica de dominio; el controller es responsable de parseo tolerante del body y composición del resultado.
- Frontend: React + axios; normalizamos claves recibidas a `camelCase` antes de usar.

---

**Solución de problemas comunes**

- Error `address already in use` al arrancar API: parar procesos que estén escuchando en el puerto (ej. `Get-NetTCPConnection -LocalPort 5272` en PowerShell) y matar el PID con `Stop-Process -Id <pid> -Force`.
- Build failing due to locked `apphost.exe` / `SistemaFinanciero.API.exe`: detener la instancia previa del API antes de compilar.
- Frontend muestra `N/A` en ratios: significa que faltan inputs suficientes (por ejemplo, `activoTotal=0` o `inventario=0`). Rellena ambos periodos con valores razonables.

---

**Buenas prácticas para desarrolladores**

- Mantén `decimal` en todo el backend para evitar problemas de redondeo en cálculos financieros.
- Valida en frontend que los inputs numéricos no estén vacíos antes de enviar; la app ya convierte strings a `parseFloat`.
- Añade tests unitarios cuando introduzcas nuevas métricas en `CalculosService`.
- Evita commitear `bin/`, `obj/`, o logs. Añade/actualiza `.gitignore` si aún no cubre `api_run*.log` y `"Any CPU"` dirs.

---

**Siguientes pasos sugeridos**

- Añadir validación de formularios en el frontend para advertir al usuario cuando faltan campos clave.
- Añadir tests unitarios para los nuevos ratios implementados (inventarios, cobros, activos fijos, cobertura de intereses).
- Añadir un endpoint para exportar el reporte a PDF/Excel si se requiere.
- Añadir CI (GitHub Actions) para build + test + lint en PRs.

---

Si quieres, puedo:
- Ejecutar un test E2E con valores de ejemplo y pegar la respuesta completa del API.
- Añadir `.gitignore` y excluir artefactos bin/obj y logs que se generaron en el repo.
- Generar una colección Postman / Insomnia con ejemplos.

Gracias — dime qué quieres que haga a continuación y lo hago.