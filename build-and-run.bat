@echo off
echo ========================================
echo   DrMusa POS - Build and Run
echo ========================================
echo.

echo [1/2] Building solution...
dotnet build DrMusa.sln --configuration Debug
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo BUILD FAILED! Fix errors above and try again.
    pause
    exit /b 1
)

echo.
echo [2/2] Launching DrMusa POS...
start "" "src\DrMusa.Desktop\bin\Debug\net8.0-windows\DrMusa.Desktop.exe"

echo.
echo DrMusa POS launched successfully!
