@echo off
title DentalBooking Startup

echo =======================================
echo   Starting DentalBooking Microservices
echo =======================================
echo.

:: Check Docker
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Docker is not installed or not running.
    echo.
    pause
    exit /b
)

:: Build images
echo Building Docker images...
docker-compose build
if %errorlevel% neq 0 (
    echo ERROR: Build failed.
    echo.
    pause
    exit /b
)

:: Start containers
echo Starting containers...
docker-compose up -d
if %errorlevel% neq 0 (
    echo ERROR: Failed to start containers.
    echo.
    pause
    exit /b
)

echo.
echo =======================================
echo Containers are running successfully.
echo ---------------------------------------
echo Gateway:      http://localhost:7000
echo Auth Service: http://localhost:7017
echo Client:       http://localhost:7050
echo PostgreSQL:   localhost:5432
echo =======================================
echo.

pause
