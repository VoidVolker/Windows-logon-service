# Windows Logon service

This service is able to run any application on user login screen. I spend several weeks to collect all solutions from internet into one working project. Using specific WinAPI calls this service collects info about logon session and using this info to run application in logon session. PR are welcome!

Этот сервис может запускать любое приложение на экране входа в систему или экране блокировки. В процессе реализации было потрачено несколько недель времени на сбор и объединение нескольких разных решений с просторов интернета. Используется несколько специфичных WinAPI вызовов для открытия процесса `LogonUI` и получения информации о сессии логина и использовании её для запуска приложения в контексте данной сессии. При этом сам сервис работает от имени системы. Пулл-реквесты приветствуются!

![LogonApplication](https://github.com/VoidVolker/Windows-logon-service/assets/5086438/23dfd564-a8b7-43d2-a96d-3205aa40c341)

# How to use / Как использовать

1. Download archive for your platform at [Releases](https://github.com/VoidVolker/Windows-logon-service/releases) page
1. Unzip to `Program files/LogonService` or any other preffered dir
1. Edit configuration file `LogonService.exe.config`:
    - `App`, `string` - aplication to run at logon screen, full file path, both slash and backslash is supported (`/` or `\`)
    - `Restart`, `bool` - watch to app stop event and restart application if it was closed
    - `RestartLimit`, `uint` - application restarts limit, when it reached Logon Service will stop restarting application, `0` - no limit
    - `LogEnabled`, `bool` - turn on/off logging
    - `LogPath`, `string` - full file path to log file (default: `<exe dir>/log.txt` - not current dir, because for service it will be `system32`)
    - `Description`, `string` - service description
    - `DisplayName`, `string` - service display name
    - `ServiceName`, `string` - service system level name (no spaces!)
1. Install service in command line:
```cmd
LogonService.exe -install
```

1. Скачать архив для вашей платформы на странице [Releases](https://github.com/VoidVolker/Windows-logon-service/releases)
1. Распаковать в `Program files/LogonService` или любой другой каталог
1. Внести настройки в файл конфигурации `LogonService.exe.config`:
    - `App`, `string` - приложение для запуска, полный путь, оба вида слэшей поддерживаются (`/` или `\`)
    - `Restart`, `bool` - автоматически перезапускать приложение при его остановке
    - `RestartLimit`, `uint` - ограничения числа перезапусков приложения, `0` - неограниченное число
    - `LogEnabled`, `bool` - включить/выключить лог
    - `LogPath`, `string` - полный путь до лог-файла (по-умолчанию: `<каталог сервиса>/log.txt` - текущий каталог не используется, т.к. для сервисов это будет каталог `system32`)
    - `Description`, `string` - описание сервиса
    - `DisplayName`, `string` - название сервиса
    - `ServiceName`, `string` - системное название сервиса (без пробелов!)
1. Установить сервис в коммандной строке:
```cmd
LogonService.exe -install
```

# Operating systems support / Поддерживаемые ОС

- Windows 7, 8.1, 10, 11

# Requirements / Требования

- DotNet 4.6.1, 4.8.0, 4.8.1

What about newer dotNet versions? Until dotNet 4.8.1 support in OS is dropped no migrations is planned. Newer dotNet service requires to install additional dependencies and that makes service heavy without any reason.

Что на счет более новых версии dotNet? До тех пор, пока ОС будут поддерживать dotNet 4.8.1 миграция проекта не планируется. Более новые версии dotNet для разработки сервиса требуют устанавливать дополнительные зависимости, а это утяжелит и усложнит сервис.

# Run multiple applications / Запуск нескольких приложений

Use next pattern for options naming in configuration file to run any number of applications: `<option><delimiter><application unique id>`.
Short example: `App Info`, `Restart Info`, `RestartLimit Info` and etc.
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

Используйте следующий паттерн в файле настроек для запуска нескольких приложений: `<настройка><разделитель><уникальный идентификатор приложения>`.
Краткий пример: `App Info`, `Restart Info`, `RestartLimit Info` и т.д.
Полный пример:
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

Список поддерживаемых разделителей: ```\t !\\\"#$%&'()*+,-./|:;?[\\]^_`{|}~``` - используйте любой по вашему вкусу.

# Config apply / Применение настроек

Logon Service check last write date of configuration and reloads it at start and logon events.

Сервис в момент запуска и активации экрана входа проверяет дату изменения файла конфигурации и подгружает изменения в память.

# Available commands / Доступные команды

- `-i` `/i` `-install` `/install` - install and start service
- `-u` `/u` `-uninstall` `/uninstall` - stop and uninstall service
- `-r` `/r` `-reinstall` `/reinstall` - reinstall service
- `-s` `/s` `-status` `/status` - service status, possible values: `Stopped` | `StartPending` | `StopPending` | `Running` | `ContinuePending` | `PausePending` | `Paused` | `NotInstalled`
- `-start` `/start` - start service
- `-stop` `/stop` - stop service
- `-restart` `/restart` - restart service



- `-i` `/i` `-install` `/install` - устанвоить и запустить сервис
- `-u` `/u` `-uninstall` `/uninstall` - остановить и удалить сервис
- `-r` `/r` `-reinstall` `/reinstall` - переустановить сервис
- `-s` `/s` `-status` `/status` - статус сервиса, возможные значения: `Stopped` | `StartPending` | `StopPending` | `Running` | `ContinuePending` | `PausePending` | `Paused` | `NotInstalled`
- `-start` `/start` - запустить сервис
- `-stop` `/stop` - остановить сервис
- `-restart` `/restart` - перезапустить сервис

# Security / Безопасность

Run classic applications at logon screen can create security issues - be very attentive when you do that. Run only specific applications at logon screen with limited features. Installed Logon Service doesn't creates new security issue because it can run applications at logon screen - this can be done by any application in system.

Запуск классических приложений на экране входа может создать угрозу безопасности системы - будьте аккуратны в выборе запускаемых приложений. Рекомендуется запускать приложения только с ограничениями. Установленный сервис не создаёт дополнительных угроз безопасности системы - используются стандартные вызовы WinAPI, которые могут быть использованы любым приложением в системе.

# Sponsors / Спонсоры

Big thanks to all open source software sponsors - you make this world better =)

Большое спасибо всем спонсорам приложений с открытым исходным кодом - вы делаете этот мир лучше =)

1. [jazir555](https://github.com/jazir555)
