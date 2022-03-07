# Core Billing Service

[![Build Status](https://www.travis-ci.com/IGBIllinois/CoreBillingService.svg?branch=master)](https://www.travis-ci.com/IGBIllinois/CoreBillingService)

Program to monitor who is logged into a windows machine.  

Then it sends the information to the core billing website [https://github.com/IGBIllinois/Corebilling](https://github.com/IGBIllinois/Corebilling).

## Compiling

* Microsoft Visual Studios 2019
* Microsoft Visual Studio Installer Project Plugin
* .NET Framework 4.8 - [https://dotnet.microsoft.com/download/dotnet-framework/net48](https://dotnet.microsoft.com/download/dotnet-framework/net48)

## Installing
### Requirements
* Windows 7 with SP1 or 10, 32 or 64 bit
* .NET Framework 4.8.  The installer should install this.  It can also be downloaded manually from [https://support.microsoft.com/en-us/help/4503548/microsoft-net-framework-4-8-offline-installer-for-windows](https://support.microsoft.com/en-us/help/4503548/microsoft-net-framework-4-8-offline-installer-for-windows)
* Windows 7 KB4474419.  This needs to be installed in order to install .Net Framework 4.8. [https://www.catalog.update.microsoft.com/search.aspx?q=kb4474419](https://www.catalog.update.microsoft.com/search.aspx?q=kb4474419)
* Windows 7 TLS 1.1/1.2 support. KB3140245.  This can be installed through Windows Update as optional update.  It can also be manually installed by going to [https://www.catalog.update.microsoft.com/search.aspx?q=kb3140245](https://www.catalog.update.microsoft.com/search.aspx?q=kb3140245)
* Windows 7 Enable TLS 1.2: Import either windows_7_tls_1.2_x86.reg or windows_7_tls_1.2_x64.reg into the Windows Registr.

### Installation
* Download release from [https://github.com/IGBIllinois/CoreBillingService/releases](https://github.com/IGBIllinois/CoreBillingService/releases)
* Run **CoreBillingServiceSetup_x64.msi** or **CoreBillingServiceSetup_x86.msi**
* During installation put in the URL and generated key from core billing software
* The Service will automatically start and set Recovery Options.
* For this to function properly, disable **Fast User Switching**
* Logs can be viewed by going to Control Panel->Administrative Tools->Event Viewer->Windows Logs->Application->CoreBillingService

