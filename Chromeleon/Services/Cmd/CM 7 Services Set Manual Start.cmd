
@Set StartType=demand
@rem Set StartType=auto

Call SC config "ChromeleonCacheService"         start= %StartType%
Call SC config "ChromeleonDataProcessingServer" start= %StartType%
Call SC config "ChromeleonDataVaultServer"      start= %StartType%
Call SC config "ChromeleonDiscoveryServer"      start= %StartType%
Call SC config "ChromeleonInstrumentServer"     start= %StartType%
Call SC config "ChromeleonLicenseServer"        start= %StartType%
Call SC config "ChromeleonMobileDeviceService"  start= %StartType%
Call SC config "ChromeleonRealTimeKernel"       start= %StartType%
Call SC config "ChromeleonPrintService"         start= %StartType%
Call SC config "ChromeleonSchedulerServer"      start= %StartType%
Call SC config "ChromeleonUpdaterService"       start= %StartType%
Call SC config "ChromeleonUserServer"           start= %StartType%

@Echo.
@Pause
