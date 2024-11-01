# Windows Logon service

This service is able to run any application on user login screen. I spend several weeks to collect all solutions from internet into one working project. Using specific WinAPI calls this service collects info about logon session and using this info to run application in logon session. PR are welcome!

![LogonApplication](https://github.com/VoidVolker/Windows-logon-service/assets/5086438/23dfd564-a8b7-43d2-a96d-3205aa40c341)

# How to use
1. Download archive for your platform at  [Releases](https://github.com/VoidVolker/Windows-logon-service/releases) page
1. Edit configuration file `LogonService.exe.config`:
    - `OnLogon`, `string` - aplication to run at logon screen (mode: immediately restart app if it die)
    - `LogEnabled`, `bool` - turn on/off logging
    - `LogPath`, `string` - full file path to log file (default: `exe dir/log.txt` - not current dir, because for service it will be system32)
    - `Description`, `string` - service description
    - `DisplayName`, `string` - service display name
    - `ServiceName`, `string` - service system level name (no spaces!)

1. Install service in command line:

```cmd
LogonService.exe /install
```

Restart service after editing configuration file.

# Available commands

- `-i` `/i` `-install` `/install` - install and start service
- `-u` `/u` `-uninstall` `/uninstall` - stop and uninstall service
- `-r` `/r` `-reinstall` `/reinstall` - reinstall service
- `-start` `/start` - start service
- `-stop` `/stop` - stop service

# Operating systems support

Windows XP SP3, 7, 8.1, 10, 11

# Requirements

- DotNet 4.5.1 for Windows XP
- DotNet 4.8.1 for all other Windows versions

# ToDo

- Add config file watcher for options reload in runtime

# Sponsors

Big thanks to all open source software sponsors - you make this world better =)

1. [jazir555](https://github.com/jazir555)
