# Windows Logon service

This service is able to run any application on user login screen. I spend several weeks to collect all solutions from internet into one working project. Using specific WinAPI calls this service collects info about logon session and using this info to run application in logon session. PR are welcome!

![LogonApplication](https://github.com/VoidVolker/Windows-logon-service/assets/5086438/23dfd564-a8b7-43d2-a96d-3205aa40c341)

# How to use

1. Download archive for your platform at [Releases](https://github.com/VoidVolker/Windows-logon-service/releases) page
1. Unzip to `Program files/LogonService` or any other preffered dir
1. Edit configuration file `LogonService.exe.config`:
    - `App`, `string` - aplication to run at logon screen, full file path, both slash and backslash is supported (`/` or `\`)
    - `Restart`, `bool` - watch to app stop event and restart appliation if it was closed
    - `RestartLimit`, `uint` - application restarts limit, when it reached Logon Service will stop restarting application, `0` value - no limit
    - `LogEnabled`, `bool` - turn on/off logging
    - `LogPath`, `string` - full file path to log file (default: `<exe dir>/log.txt` - not current dir, because for service it will be system32)
    - `Description`, `string` - service description
    - `DisplayName`, `string` - service display name
    - `ServiceName`, `string` - service system level name (no spaces!)
1. Install service in command line:

```cmd
LogonService.exe -install
```

# Requirements

- DotNet 4.6.1 or 4.8.0 installed

# Operating systems support

- Windows 7, 8.1, 10, 11

# Run multiple applications

Use next pattern for options naming in configuration file to run any number of applications: `<option><delimiter><application unique id>`.
Short example 1: `App Info`, `Restart Info`, `RestartLimit Info` and etc.
Full example:
```xml
<add key="App Info"
        value="C:\adm\info.exe"/>
<add key="Watch Info"
        value="true"/>
<add key="RestartLimit Info"
        value="100"/>
<add key="App Password recovery"
        value="C:\adm\pwdrecovery.exe"/>
<add key="Watch Password recovery"
        value="true"/>
<add key="RestartLimit Password recovery"
        value="0"/>
```

Supported delimiters list: ```\t !\\\"#$%&'()*+,-./|:;?[\\]^_`{|}~``` - feel free to use whatever you like.

# Config apply

Logon Service check last write date of configuration and reloads it at start and logon events.

# Available commands

- `-i` `/i` `-install` `/install` - install and start service
- `-u` `/u` `-uninstall` `/uninstall` - stop and uninstall service
- `-r` `/r` `-reinstall` `/reinstall` - reinstall service
- `-s` `/s` `-status` `/status` - service status, possible values: `Stopped` | `StartPending` | `StopPending` | `Running` | `ContinuePending` | `PausePending` | `Paused` | `NotInstalled`
- `-start` `/start` - start service
- `-stop` `/stop` - stop service
- `-restart` `/restart` - restart service

# Security

Run classic applications at logon screen can create security issues - be very attentive when you do that. Run only specific applications at logon screen with limited features. Installed Logon Service doesn't creates new security issue because it can run applications at logon screen - this can be done by any application in system.

# Sponsors

Big thanks to all open source software sponsors - you make this world better =)

1. [jazir555](https://github.com/jazir555)
