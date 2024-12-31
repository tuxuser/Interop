# Durango Interop Dotnet

This repository contains:
1. C# interop definitions for DLL exports & COM objects that have been discovered within the Xbox operating systems.
1. Applications that leverage the interops to perform various operations on an Xbox via MSBuild task launching.
1. Powershell modules that allow for scripted access to the interop code.

## C# interops

|Interop COM Objects|
|-|-|
|IAppPackageMountManager|
|IAppPackageMountManagerInternal|
|IClipLicenseManager|
|ILicense|
|UserManager|

|Interop Libraries|
|-|-|
|FirewallManager|
|XcrdApi|
|WinUserManager|

## Applications

All applications are launched via MSBuild tasks due to Code Integrity checks implemented on the Xbox.  This requires the [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) to be downloaded and accessible on the Xbox as well as the [latest Interop release](https://github.com/xboxoneresearch/Interop/releases).

|App|Description|
|-|-|
|[Disable Firewall](apps\disable_firewall\README.md)|Disable firewall and open up SSH & Debugger ports|
|[List Temp XVDs Games](apps\get_tempxvd_owners\README.md)|List the game associated with each `[XUC:]\tempXX` slot|
|[List Licenses](apps\list_licenses\README.md)|Prints all installed licenses and their accompanying metadata|
|[Load PE ShellCode](apps\load_pe_shellcode\README.md)|Load and execute arbitrary shellcode|
|[Mount ConnectedStorage](apps\mount_connectedstorage\README.md)|Mount the `ConnectedStorage` XVD which contains game saves|
|[Prepare Game Dump](apps\prepare_gamedump\README.md)|Copy a target game to the Temp XVD slot and load its keys into the PSP|
|[Dump Registry](apps\registry_dump\README.md)|Dump the registry to a HIVE file|
|[Backup XBFS](apps\xbfs_backup\README.md)|Dump the XBFS Flash|

**NOTE**: Some tasks require modifications before being used, f.e. to adjust paths or addresses.

### Running as different users

The above tasks can be run as other users (DefaultAccount, SYSTEM, etc) via:

```
schtasks /create /f /tn "MyBackgroundTask" /ru USER_NAME /sc ONSTART /tr "D:\dotnet\dotnet.exe msbuild D:\apps\APP_NAME\launch.xml"
schtasks /run /tn "MyBackgroundTask"
```

## Powershell Modules

By leveraging Powershell we are able to interact with the C# interops via the command line.  This requires powershell to be downloaded and accessible on the Xbox (this can be accomplished by leveraging [SharpShell](https://github.com/xboxoneresearch/SharpShell) or [Silverton](https://github.com/kwsimons/Silverton)/[AnimaSSH](https://github.com/kwsimons/AnimaSSH)) as well as the [latest Interop release](https://github.com/xboxoneresearch/Interop/releases).

### Usage

1. Enter a Powershell shell
1. Execute interop code via scripting commands (examples below)

#### XCrdAPi

```powershell
Add-Type -Path pwsh\xcrd.cs
$xcrd = [XCrdManager]::new()

# Mount savegame
$xcrd.Unmount("[XTE:]\ConnectedStorage-retail")
$xcrd.Mount("[XTE:]\ConnectedStorage-retail")
# [+] Opened adapter...
# [+] Successfully mounted XVD!
# [+] Mount Path: \\?\GLOBALROOT\Device\Harddisk15\Partition1
cmd /c mklink /j T:\connectedStorage "\\?\GLOBALROOT\Device\Harddisk15\Partition1\"
```

#### LicenseManager

```powershell
Add-Type -Path pwsh\license.cs
$lm = [LicenseManager]::new()

$lm.PrintLicenses()
$lm.LoadLicenseFile("S:\Clip\g-u-i-d")
```


### XcrdUtil module

This module mimicks the program `xcrdutil.exe` that exists on the Xbox but cannot be natively executed (unless you are leveraging [Silverton](https://github.com/kwsimons/Silverton) to create your shell)

#### Usage
```
#Powershell
Import-Module -Name D:\modules\xcrdutil

interop.xcrdutil -m [XUC:]\connectedStorage-retail
```
