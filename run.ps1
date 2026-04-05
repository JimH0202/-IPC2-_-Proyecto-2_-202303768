# Script para ejecutar Backend y Frontend simultaneamente
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Iniciando Backend y Frontend" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$rootPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendPath = Join-Path $rootPath "Backend"
$frontendPath = Join-Path $rootPath "Frontend"

# Limpiar puertos 5000 y 5001 antes de iniciar
Write-Host "`n[0/5] Liberando puertos 5000 y 5001..." -ForegroundColor Yellow
try {
    Get-NetTCPConnection -LocalPort 5000,5001 -ErrorAction SilentlyContinue | ForEach-Object {
        $proc = Get-Process -Id $_.OwningProcess -ErrorAction SilentlyContinue
        if ($proc) {
            Stop-Process -Id $_.OwningProcess -Force -ErrorAction SilentlyContinue
            Write-Host "  OK Detenido $($proc.Name) (PID: $($_.OwningProcess)) en puerto $($_.LocalPort)" -ForegroundColor Gray
        }
    }
    Start-Sleep -Seconds 1
}
catch {
    Write-Host "  SKIP (sin procesos en puertos)" -ForegroundColor Gray
}

Write-Host "`n[1/5] Compilando Backend..." -ForegroundColor Yellow
Push-Location $backendPath
dotnet build --nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error compilando Backend" -ForegroundColor Red
    Pop-Location
    exit 1
}
Pop-Location

Write-Host "[2/5] Compilando Frontend..." -ForegroundColor Yellow
Push-Location $frontendPath
dotnet build --nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error compilando Frontend" -ForegroundColor Red
    Pop-Location
    exit 1
}
Pop-Location

Write-Host "`n[3/5] Iniciando Backend (en background)..." -ForegroundColor Green
$backendProcess = Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", (Join-Path $backendPath "Backend.csproj") -WorkingDirectory $backendPath -NoNewWindow -PassThru
Write-Host "Backend iniciado (PID: $($backendProcess.Id))" -ForegroundColor Green

Start-Sleep -Seconds 3

Write-Host "[4/5] Iniciando Frontend..." -ForegroundColor Green
Push-Location $frontendPath
dotnet run --project Frontend.csproj

# Limpiar el proceso del backend cuando se cierre el frontend
Write-Host "`n[5/5] Deteniendo servicios..." -ForegroundColor Yellow
Stop-Process -Id $backendProcess.Id -Force -ErrorAction SilentlyContinue
Write-Host "OK Backend y Frontend detenidos" -ForegroundColor Green

Pop-Location
