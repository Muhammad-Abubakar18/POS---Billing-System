@echo off
echo ========================================
echo Publishing DrMusa POS for Client
echo ========================================
echo.
echo This will create a standalone executable that does not require .NET to be installed.
echo.

dotnet publish src\DrMusa.Desktop\DrMusa.Desktop.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o "c:\Users\QS\Downloads\POS\DrMusa\ClientRelease"

echo Copying missing assets...
mkdir "c:\Users\QS\Downloads\POS\DrMusa\ClientRelease\logo" 2>nul
copy /Y "c:\Users\QS\Downloads\POS\DrMusa\assets\logo\DrMusa-logo.ico" "c:\Users\QS\Downloads\POS\DrMusa\ClientRelease\logo\DrMusa-logo.ico"
copy /Y "c:\Users\QS\Downloads\POS\DrMusa\assets\logo\DrMusa-logo.jpg" "c:\Users\QS\Downloads\POS\DrMusa\ClientRelease\logo\DrMusa-logo.jpg"

echo.
echo ========================================
echo Publish Complete!
echo You can find the client files in: c:\Users\QS\Downloads\POS\DrMusa\ClientRelease
echo You can simply zip this folder and send it to your client, or use Inno Setup to create a Setup.exe installer.
pause
