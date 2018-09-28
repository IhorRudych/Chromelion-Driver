@Echo Off
Echo.

Set ExcludeInstruments=%~1%

CD "%~dp0"
@Echo.

Call Service_Start "ChromeleonLicenseServer"
Call Service_Start "ChromeleonUserServer"
Call Service_Start "ChromeleonDiscoveryServer"
Call Service_Start "ChromeleonDataVaultServer"

Echo.
Call Service_Start "ChromeleonSchedulerServer"

Echo.
Call Service_Start "ChromeleonCacheService"
Call Service_Start "ChromeleonDataProcessingServer"

Echo.
@rem Call Service_Start "ChromeleonPrintService"

Echo.
@rem Call Service_Start "ChromeleonUpdaterService"

Echo.
If %ExcludeInstruments%.==. (
  Call Service_Start "ChromeleonRealTimeKernel"
  Call Service_Start "ChromeleonInstrumentServer"
)

Echo.
@rem Call Service_Start "ChromeleonMobileDeviceService"

Echo.
Echo On
@rem Pause
