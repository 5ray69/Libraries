REM Папка, которую мы указали в .csproj (PackageOutputPath)
set "PACKAGE_DIR=nupkg"
set "SOURCE=https://api.nuget.org/v3/index.json"

echo ====================================================
echo   ОТПРАВКА ПАКЕТА Libraries НА NUGET.ORG
echo ====================================================
echo.

@echo off
chcp 65001 > nul
REM 1. Проверка наличия папки с пакетами
if not exist "%PACKAGE_DIR%" (
    echo [ОШИБКА] Папка %PACKAGE_DIR% не найдена.
    echo Сначала соберите проект в конфигурации Release!
    echo.
    pause
    exit /b
)

REM 2. Показываем, что нашли
echo Список пакетов в очереди на отправку:
dir /b "%PACKAGE_DIR%\*.nupkg"
echo.

REM 3. Запрос ключа (он не сохраняется в коде!)
echo [!] Откройте ваш архив и скопируйте API Key.
set /p API_KEY=">>> Вставьте API Key и нажмите Enter: "

REM Проверка, что ключ не пустой
if "%API_KEY%"=="" (
    echo [ОШИБКА] Ключ не был введен. Операция отменена.
    pause
    exit /b
)

echo.
echo Отправка...
REM --skip-duplicate позволит не останавливаться, если версия уже есть на сервере
dotnet nuget push "%PACKAGE_DIR%\*.nupkg" --api-key %API_KEY% --source %SOURCE% --skip-duplicate

echo.
echo ====================================================
echo   ПРОЦЕСС ЗАВЕРШЕН
echo ====================================================
pause