@echo off
setlocal
if '%1'=='' goto SHOWUSAGE
:CHECKSERVICE
for /f "tokens=4 skip=3" %%i in ('sc query %1') do ^
if not defined SERVICESTATE set SERVICESTATE=%%i
if '%SERVICESTATE%'=='RUNNING' goto STOPSERVICE
if '%SERVICESTATE%'=='STOP_PENDING' goto WAITSERVICE
goto FINISH
:STOPSERVICE
echo Stopping %1 . . .
sc stop %1 >nul
:WAITSERVICE
set SERVICESTATE=
ping 127.0.0.1 >nul
goto CHECKSERVICE
:SHOWUSAGE
echo.
echo Usage: stop_service ^<servicename^>
echo.
:FINISH
set SERVICESTATE=
