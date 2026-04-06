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
start "Backend" /D "!backendPath!" cmd /c "dotnet run --project Backend.csproj"

echo [3/5] Esperando a que el backend escuche en http://localhost:5000 ...
set "backendReady=0"
for /L %%i in (1,1,15) do (
    powershell -NoProfile -Command "if (Get-NetTCPConnection -LocalPort 5000 -State Listen -ErrorAction SilentlyContinue) { exit 0 } else { exit 1 }"
    if !errorlevel! equ 0 (
        set "backendReady=1"
        goto backend_ready
    )
    timeout /t 1 /nobreak >nul
)

:backend_ready
if "!backendReady!" equ "1" (
    echo Backend disponible en http://localhost:5000
) else (
    echo ERROR: el backend no se inició en http://localhost:5000
    echo Revisa el proyecto Backend o inicia Backend manualmente.
)

REM Iniciar Backend en background
echo.
echo [3/5] Iniciando Backend en background...
cd /d "!backendPath!"
start "Backend" /D "!backendPath!" cmd /c "dotnet run --project Backend.csproj"

echo [3/5] Esperando a que el backend escuche en http://localhost:5000 ...
set "backendReady=0"
for /L %%i in (1,1,15) do (
    powershell -NoProfile -Command "if (Get-NetTCPConnection -LocalPort 5000 -State Listen -ErrorAction SilentlyContinue) { exit 0 } else { exit 1 }"
    if !errorlevel! equ 0 (
        set "backendReady=1"
        goto backend_ready
    )
    timeout /t 1 /nobreak >nul
)

:backend_ready
if "!backendReady!" equ "1" (
    echo Backend disponible en http://localhost:5000
) else (
    echo ERROR: el backend no se inició en http://localhost:5000
    echo Revisa el proyecto Backend o inicia Backend manualmente.
)

REM Iniciar Frontend en foreground
echo [4/5] Iniciando Frontend...
cd /d "!frontendPath!"
dotnet run --project Frontend.csproj --urls http://localhost:5001


echo.
echo [5/5] Limpiando procesos...
echo Backend y Frontend detenidos

endlocal
