# Aspire Employee Management — React + EF Core + SQL Server LocalDB

This solution extends the existing Aspire Employee Starter with a React dashboard.

## Requirements

- Windows + Visual Studio with .NET 10 SDK and Aspire workload/tooling
- SQL Server LocalDB instance `(localdb)\MSSQLLocalDB`
- Existing database `TestDataBase` with the Employees table (review the included EF migration before applying it)
- Node.js 20.19+ or 22.12+ and npm on PATH
- Aspire AppHost packages 13.6.5

## First-time setup

1. Open `AspireEmployeeStarter.slnx` in Visual Studio.
2. In a terminal at the solution root:

   ```powershell
   sqllocaldb start MSSQLLocalDB
   dotnet restore
   dotnet build
   cd .\AspireEmployeeStarter.Web
   npm install
   cd ..
   ```

3. If you haven't created Employees in `TestDataBase`, review the migration, then apply:

   ```powershell
   dotnet tool install --global dotnet-ef --version 10.*
   dotnet ef database update --project .\AspireEmployeeStarter.Api --startup-project .\AspireEmployeeStarter.Api
   ```

4. Set `AspireEmployeeStarter.AppHost` as the startup project and choose the `http` profile. Press F5.
5. Aspire dashboard: http://localhost:15000
6. React UI: http://localhost:5173 (open from the `web` resource URL).

## How requests work

Browser -> Vite React dev server (`localhost:5173`) -> Vite proxy `/api` -> Aspire-managed Minimal API -> EF Core -> SQL Server LocalDB `TestDataBase`.

Aspire supplies `API_BASE_URL` to the Vite process. No CORS policy or hard-coded API port is required when starting from Aspire.

## Troubleshooting

- **web resource exits**: run `npm install` in `AspireEmployeeStarter.Web` and restart AppHost; check that Node/npm is on PATH.
- **5173 already in use**: stop the other Vite instance, or change the port in both `AppHost/Program.cs` and `package.json`.
- **Database connection error**: verify `(localdb)\MSSQLLocalDB` and `TestDataBase`; open `/api/database/status` on the API URL shown in Aspire.
- **Invalid object name Employees**: review/apply the EF migration. Do not apply unreviewed migrations to an existing database.
- **Aspire executable endpoint issue**: check that your installed Aspire AppHost SDK matches the 13.6.5 package versions.
- **AppHost browser opening**: `launchBrowser: true` in the AppHost HTTP launch profile opens the Aspire dashboard; use its `web` resource link to open React.

## API routes

GET `/api/employees`, GET `/api/employees/{id}`, POST `/api/employees`, PUT `/api/employees/{id}`, DELETE `/api/employees/{id}`, GET `/api/database/status`.

## Notes

- Development HTTP only. Before Entra ID/OIDC, enable HTTPS and proper authentication.
- The React dashboard is a separate folder rather than a C# project; Aspire starts it as an executable.
- `npm install` is intentionally a one-time setup step, not a hidden runtime operation.
