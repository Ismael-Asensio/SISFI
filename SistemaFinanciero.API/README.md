# SistemaFinanciero.API

Endpoint principal: `POST /api/analisis/completo`

Request body: `AnalisisMultiPeriodoDTO` with two properties `PeriodoActual` and `PeriodoAnterior` of type `DatosPeriodoDTO`.

Example request body:

```json
{
  "PeriodoActual": { "Ventas": 100000, "UtilidadNeta": 8000, "Depreciacion": 500, "Efectivo": 12000, "CuentasPorCobrar": 8000, "Inventario": 15000, "ActivoCorriente": 40000, "CuentasPorPagar": 3000, "PasivoCorriente": 15000, "ActivoTotal": 120000, "PasivoTotal": 70000, "Patrimonio": 50000 },
  "PeriodoAnterior": { "Ventas": 90000, "UtilidadNeta": 7000, "Depreciacion": 450, "Efectivo": 10000, "CuentasPorCobrar": 7500, "Inventario": 14000, "ActivoCorriente": 38000, "CuentasPorPagar": 2800, "PasivoCorriente": 14000, "ActivoTotal": 110000, "PasivoTotal": 65000, "Patrimonio": 45000 }
}
```

Response: `ResultadoAnalisisCompleto` containing:

- `DuPont`: breakdown of ROE
  - `MargenUtilidad`: Returned as percentage (e.g., 12.34 means 12.34%).
  - `RotacionActivos`: Rotation (times), plain number.
  - `ApalancamientoFinanciero`: Leverage (times), plain number.
  - `ROE`: Returned as percentage (e.g., 8.45 means 8.45%).

- `Razones`: key ratios
  - `RazonCorriente`: plain ratio (current assets / current liabilities).
  - `NivelEndeudamiento`: Returned as percentage (e.g., 75.00 means 75%).
  - `ROA`: Returned as percentage.
  - `ROE`: Returned as percentage.
  - `MargenNeto`: Returned as percentage.

- `Vertical`: percentages of balance sheet items (fields prefixed with `Pct_` are percentages, e.g., `Pct_ActivoCorriente`).

- `Horizontal`: list of `VariacionCuenta` entries with numeric values and percent variation (`VariacionPorcentual` is percentage).

- `FlujoEfectivo`: cash-flow items (monetary values), numeric.

Notes and front-end mapping
- The API now returns many indicators already converted to percentage units (multiplied by 100). The front-end should not multiply those fields again by 100. Fields that remain as plain numbers are `RotacionActivos` and `ApalancamientoFinanciero`.
- Example: `DuPont.MargenUtilidad = 12.34` means 12.34%.

Local run
- Build and run API:

```powershell
dotnet build ProyectoFinancieroMIPYMES.sln
dotnet run --project .\SistemaFinanciero.API
```

Testing
- Unit + integration tests live under `tests/SistemaFinanciero.Tests`. Run:

```powershell
dotnet test tests\SistemaFinanciero.Tests\SistemaFinanciero.Tests.csproj
```

If you change units (percent vs ratio), update this README and the front-end formatting accordingly.