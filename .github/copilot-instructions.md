<!--
Guidance for AI coding agents working on ProyectoFinancieroMIPYMES.
Keep this file concise and actionable. Refer to the specific files listed below.
-->

# Copilot / AI agent instructions — ProyectoFinancieroMIPYMES

Quick orientation
- This repo contains a .NET 8 Web API (back-end) in `SistemaFinanciero.API/` and a React front-end in `sistema-financiero.client/`.
- Primary interaction: React (localhost:3000) calls the API POST endpoint `POST /api/analisis/completo` to run financial analyses.

What to read first
- `SistemaFinanciero.API/Program.cs` — app startup, DI registrations, CORS policy (allows React on http://localhost:3000), and Swagger registration.
- `SistemaFinanciero.API/Controllers/AnalisisController.cs` — single controller exposing `api/analisis/completo`. Shows validation and how service results are composed.
- `SistemaFinanciero.API/Services/CalculosService.cs` — contains DTOs, result models, and all calculation logic (DuPont, ratios, vertical/horizontal analysis, cash flow).

Key patterns & project-specific conventions
- DTOs and result model classes are declared inside `CalculosService.cs` (file-scoped). When adding new DTOs or result types, keep them colocated with calculation logic unless they become shared across controllers.
- Numeric precision: calculations use C# `decimal` throughout — preserve `decimal` for financial math to avoid precision loss.
- Percentage semantics:
  - `CalcularVertical` returns percentages (multiplies by 100). `CalcularHorizontal` computes percent change when previous value != 0.
  - Keep these semantics when consuming or formatting results in the front-end.
- Error handling: controllers validate required DTO presence and return `BadRequest` when input is missing. `CalculosService` methods are pure and do not throw for zero/empty inputs — they defensively check denominators.
- Dependency injection: `CalculosService` is registered as scoped in `Program.cs`. Register any new services in `Program.cs` under `builder.Services` using the same pattern and namespace `SistemaFinanciero.API.Services`.

Endpoints & dataflow
- POST /api/analisis/completo
  - Input: `AnalisisMultiPeriodoDTO` (see `CalculosService.DatosPeriodoDTO` shape). Provide `PeriodoActual` and `PeriodoAnterior`.
  - Output: `ResultadoAnalisisCompleto` object containing DuPont, Razones, Vertical, Horizontal, FlujoEfectivo.

Example request body (numbers are decimals):
```
{
  "PeriodoActual": { "Ventas": 100000, "UtilidadNeta": 8000, "Depreciacion": 500, "Efectivo": 12000, "CuentasPorCobrar": 8000, "Inventario": 15000, "ActivoCorriente": 40000, "CuentasPorPagar": 3000, "PasivoCorriente": 15000, "ActivoTotal": 120000, "PasivoTotal": 70000, "Patrimonio": 50000 },
  "PeriodoAnterior": { "Ventas": 90000, "UtilidadNeta": 7000, "Depreciacion": 450, "Efectivo": 10000, "CuentasPorCobrar": 7500, "Inventario": 14000, "ActivoCorriente": 38000, "CuentasPorPagar": 2800, "PasivoCorriente": 14000, "ActivoTotal": 110000, "PasivoTotal": 65000, "Patrimonio": 45000 }
}
```

Local dev run / build notes (discoverable)
- Back-end (from repo root):
  - Build: `dotnet build ProyectoFinancieroMIPYMES.sln`
  - Run API only: `dotnet run --project SistemaFinanciero.API` (starts on configured port; Swagger enabled in Development)
  - Swagger UI is available when `app.Environment.IsDevelopment()` (see `Program.cs`).
- Front-end (from `sistema-financiero.client/`):
  - Install and start: `npm install` then `npm start` (client README exists at `sistema-financiero.client/README.md`).
  - CORS in API is already set to allow `http://localhost:3000`.

Developer tips for PRs & code changes
- When adding new analysis functions prefer adding them as methods in `CalculosService` and expose them from `AnalisisController` only if required by the front-end.
- Keep domain models (DTOs/result types) next to `CalculosService` unless they are reused across controllers — then consider moving to a `Models/` folder.
- Preserve `decimal` types and the existing defensive checks (denominator > 0) to avoid DivideByZero and precision issues.

Files to reference when editing behavior
- `SistemaFinanciero.API/Program.cs` — DI, CORS, middleware order (CORS before Authorization).
- `SistemaFinanciero.API/Controllers/AnalisisController.cs` — how inputs are validated and how service results are returned.
- `SistemaFinanciero.API/Services/CalculosService.cs` — canonical place for business logic and data shapes.
- `sistema-financiero.client/README.md` — front-end run instructions and integration hints.

If you need more context
- Ask for sample payloads the front-end expects (UI formatting) or sample test vectors for known-good results to add unit tests.
- If adding cross-cutting behavior (logging, metrics), follow existing minimal app pipeline in `Program.cs` and register services there.

Please review these notes and tell me if you want extra examples (unit tests for `CalculosService`, sample Postman collection, or CI steps).
