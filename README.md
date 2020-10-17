# Core Billing Service


Program to monitor who is logged into a windows machine.  Then it sends the information to the core billing website [https://github.com/IGBIllinois/Corebilling](https://github.com/IGBIllinois/Corebilling).

# Compiling

* Microsoft Visual Studios 2019
* Microsoft Visual Studio Installer Project Plugin
* .NET Framework 4.8 [https://dotnet.microsoft.com/download/dotnet-framework/net48](https://dotnet.microsoft.com/download/dotnet-framework/net48)

# Installing
## Requirements
* Windows 7 or 10, 32 or 64 bit
* .NET Framework 4.8.  The installer should install this.  It can be downloaded manually from [https://support.microsoft.com/en-us/help/4503548/microsoft-net-framework-4-8-offline-installer-for-windows](https://support.microsoft.com/en-us/help/4503548/microsoft-net-framework-4-8-offline-installer-for-windows)
* Windows 7 TLS 1.1/1.2 support. KB314025.  This can be installed through Windows Update as optional update.  It can also be manually installed by going to [https://www.catalog.update.microsoft.com/search.aspx?q=kb3140245](https://www.catalog.update.microsoft.com/search.aspx?q=kb3140245)

## Installation
* Run CoreBillingServiceSetup_x64.msi or CoreBillingServiceSetup_x86.msi
* During installation put in the URL and generated key from core billing software
* The Service will automatically start.
* You need to set Service Recovery Options by going to Control Panel->Administrative Tools->Services->Core Billing Service
* For this to function with Windows Remote Desktop, disable Fast User Switching


