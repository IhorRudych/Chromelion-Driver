@Echo Off

rem Del /S/Q/F *.opensdf *.sdf
Echo.
Call "Del Folder.cmd" "Config\obj"
Echo.
Call "Del Folder.cmd" "Driver\obj"
Echo.
Call "Del Folder.cmd" "ipch"
Echo.
@rem Call "Del Folder.cmd" "C:\Thermo\Chromeleon\Bin\DDK\V1\Drivers\MyCompany"
Echo.

Echo On
@Pause
