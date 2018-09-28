
CD "%~dp0"
@Echo.

Call Service_Start "ChromeleonLicenseServer"
Call Service_Start "ChromeleonUserServer"

Echo.
Echo On
@rem Pause
