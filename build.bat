@echo off
chcp 65001 > nul
echo ========================================================
echo          Merdo Launcher Derleme ve Paketleme
echo ========================================================
echo.
echo [1/2] Proje yayınlanıyor (Release)...
dotnet publish MerdoClient -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ".\publish"

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [HATA] Proje derlenirken bir hata oluştu!
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [2/2] Inno Setup Kurulum Sihirbazı oluşturuluyor...
"%LOCALAPPDATA%\Programs\Inno Setup 6\ISCC.exe" ".\setup\setup.iss"

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [HATA] Kurulum paketi (Setup) oluşturulurken hata oluştu!
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo ========================================================
echo [BAŞARILI] Kurulum paketi başarıyla güncellendi!
echo Konum: .\installer\MerdoLauncher_Setup.exe
echo ========================================================
echo.
pause
