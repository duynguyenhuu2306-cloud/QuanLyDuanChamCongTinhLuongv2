@echo off
setlocal
set "SCRIPT=%~dp0start-workly.ps1"
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT%" -OpenBrowser
if errorlevel 1 (
  echo.
  echo Workly startup encountered an error. See the message above.
  pause
)
endlocal
