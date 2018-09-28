@Echo Off

rem Del /S/Q/F *.opensdf *.sdf
Echo.

Call "Del Folder.cmd" "Detector\EditorPlugIn\obj"
Echo.
Call "Del Folder.cmd" "Heater\EditorPlugIn\obj"
Echo.
Call "Del Folder.cmd" "Pump\EditorPlugIn\obj"
Echo.
Call "Del Folder.cmd" "ipch"
Echo.
@rem Call "Del Folder.cmd" "C:\Thermo\Chromeleon\Bin\DDK\V2\Drivers\MyCompany"
Echo.

Echo On
@Pause
