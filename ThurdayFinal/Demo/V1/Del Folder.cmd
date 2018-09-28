
Set Folder=%~1

If Exist "%Folder%" (
  Echo "%Folder%"
  Del /S/Q/F "%Folder%\*.*"
  RD  /S/Q   "%Folder%"
) else (
  rem Echo Cannot find folder "%Folder%"
)
