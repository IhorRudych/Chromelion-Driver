
CD "%~dp0"
@Echo.

Call Service_Stop "ChromeleonMobileDeviceService"
Call Service_Stop "ChromeleonPrintService"
Call Service_Stop "ChromeleonUpdaterService"
Call Service_Stop "ChromeleonInstrumentServer"
Call Service_Stop "ChromeleonSchedulerServer"
Call Service_Stop "ChromeleonDataProcessingServer"
Call Service_Stop "ChromeleonDataVaultServer"
Call Service_Stop "ChromeleonCacheService"
Call Service_Stop "ChromeleonDiscoveryServer"
Call Service_Stop "ChromeleonUserServer"
Call Service_Stop "ChromeleonRealTimeKernel"
Call Service_Stop "ChromeleonLicenseServer"

Call Kill "WebApiServiceHost.exe"
Call Kill "CmDriver.exe"
Call Kill "ServiceHost.exe"
Call Kill "SharedCache.WinService.exe"
