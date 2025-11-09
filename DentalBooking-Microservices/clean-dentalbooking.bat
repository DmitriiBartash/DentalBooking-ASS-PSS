@echo off
title DentalBooking Cleanup

echo =======================================
echo   Cleaning up DentalBooking containers
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

:: Stop running containers
echo Stopping containers...
docker-compose down
if %errorlevel% neq 0 (
    echo ERROR: Failed to stop containers.
    echo.
    pause
    exit /b
)

:: Remove unused Docker resources
echo Removing unused Docker volumes, networks, and images...
docker system prune -a -f
docker volume prune -f
docker network prune -f

echo.
echo =======================================
echo All Docker containers and resources removed.
echo =======================================
echo.

pause
