@echo off
REM Script para ejecutar Backend y Frontend simultáneamente desde Windows CMD/PowerShell
REM Limpia automáticamente los puertos 5000 y 5001 antes de iniciar

echo.
echo ========================================
echo   Iniciando Backend y Frontend
echo ========================================
echo.

setlocal enabledelayedexpansion

set "rootPath=%~dp0"
set "backendPath=!rootPath!Backend"
set "frontendPath=!rootPath!Frontend"

REM [0/5] Limpiar puertos
echo [0/5] Liberando puertos 5000 y 5001...
powershell -NoProfile -Command "Get-NetTCPConnection -LocalPort 5000,5001 -State Listen -ErrorAction SilentlyContinue | Select-Object -ExpandProperty OwningProcess | Sort-Object -Unique | ForEach-Object { Try { Stop-Process -Id $_ -Force -ErrorAction Stop; Write-Host '  Detenido proceso PID:' $_ } Catch { } }"
timeout /t 1 /nobreak >nul

REM Compilar Backend
echo [1/5] Compilando Backend...
cd /d "!backendPath!"
dotnet build --nologo --verbosity quiet
if errorlevel 1 (
    echo Error compilando Backend
    exit /b 1
)

REM Compilar Frontend
echo [2/5] Compilando Frontend...
cd /d "!frontendPath!"
dotnet build --nologo --verbosity quiet
if errorlevel 1 (
    echo Error compilando Frontend
    exit /b 1
)

REM Iniciar Backend en background
echo.
echo [3/5] Iniciando Backend en background...
cd /d "!backendPath!"
start "", /D "!backendPath!" dotnet run --project Backend.csproj

timeout /t 3 /nobreak >nul

REM Iniciar Frontend en foreground
echo [4/5] Iniciando Frontend...
cd /d "!frontendPath!"
dotnet run --project Frontend.csproj

echo.
echo [5/5] Limpiando procesos...
echo Backend y Frontend detenidos

endlocal
