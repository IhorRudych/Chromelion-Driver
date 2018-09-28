@Echo Off
setlocal
if '%1'=='' goto SHOWUSAGE
:CHECKSERVICE
for /f "tokens=4 skip=3" %%i in ('sc query %1') do ^
if not defined SERVICESTATE set SERVICESTATE=%%i
if '%SERVICESTATE%'=='STOPPED' goto STARTSERVICE
if '%SERVICESTATE%'=='START_PENDING' goto WAITSERVICE
goto FINISH
:STARTSERVICE
echo Starting %1 . . .
sc start %1 >nul
:WAITSERVICE
set SERVICESTATE=
ping 127.0.0.1 >nul
goto CHECKSERVICE
:SHOWUSAGE
echo.
echo Usage: start_service ^<servicename^>
echo.
:FINISH
set SERVICESTATE=
