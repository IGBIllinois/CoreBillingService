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
* Windows 7 TLS 1.1/1.2 support. KB314025.  This can be installed through Windows Update as optional update.  It can also be manually installed by going to [https://www.catalog.update.microsoft.com/search.aspx?q=kb3140245](https://www.catalog.update.microsoft.com/search.aspx?q=kb3140245)
* Windows 7 Enable TLS 1.2.  Download and install the easyfix from microsoft at [http://download.microsoft.com/download/0/6/5/0658B1A7-6D2E-474F-BC2C-D69E5B9E9A68/MicrosoftEasyFix51044.msi](http://download.microsoft.com/download/0/6/5/0658B1A7-6D2E-474F-BC2C-D69E5B9E9A68/MicrosoftEasyFix51044.msi).  This set the registry settings as described in [https://support.microsoft.com/en-us/topic/update-to-enable-tls-1-1-and-tls-1-2-as-default-secure-protocols-in-winhttp-in-windows-c4bd73d2-31d7-761e-0178-11268bb10392](https://support.microsoft.com/en-us/topic/update-to-enable-tls-1-1-and-tls-1-2-as-default-secure-protocols-in-winhttp-in-windows-c4bd73d2-31d7-761e-0178-11268bb10392)

### Installation
* Download release from [https://github.com/IGBIllinois/CoreBillingService/releases](https://github.com/IGBIllinois/CoreBillingService/releases)
* Run **CoreBillingServiceSetup_x64.msi** or **CoreBillingServiceSetup_x86.msi**
* During installation put in the URL and generated key from core billing software
* The Service will automatically start.
* You need to set Service Recovery Options by going to Control Panel->Administrative Tools->Services->Core Billing Service->Recovery
* For this to function properly, disable **Fast User Switching**
* Logs can be viewed by going to Control Panel->Administrative Tools->Event Viewer->Windows Logs->Application->CoreBillingService

