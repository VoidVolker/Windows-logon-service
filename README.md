# Windows Logon service

This service is able to run any application on user login/lock screen. I spend several weeks to collect all solutions from internet into one working project. Using specific WinAPI calls this service collects info about logon session and using this info to run application in logon session. PR are welcome!

Этот сервис может запускать любое приложение на экране входа в систему/экране блокировки. В процессе реализации было потрачено несколько недель времени на сбор и объединение нескольких разных решений с просторов интернета. Используется несколько специфичных WinAPI вызовов для открытия процесса `LogonUI` и получения информации о сессии логина и использовании её для запуска приложения в контексте данной сессии. При этом сам сервис работает от имени системы. Пулл-реквесты приветствуются!

![LogonApplication](https://github.com/VoidVolker/Windows-logon-service/assets/5086438/23dfd564-a8b7-43d2-a96d-3205aa40c341)

# English

## How to use

1. Download archive for your platform at [Releases](https://github.com/VoidVolker/Windows-logon-service/releases) page (for x64 platform download x64 version due system API limitations for x86 apps)
1. Unzip to `Program files/LogonService` or any other preffered dir
1. Edit configuration file `LogonService.exe.config`:
    - `App`, `string`, required, - aplication to run at logon screen, full file path or relative path to `LogonService.exe` directory, both slash and backslash is supported (`/` or `\`)
    - `App.Arguments`, `string`, optional - application arguments, use single quotes for parameter and double quotes to split arguments with spaces, if no spaces in arguments - use space as argument delimiter and default double quotes for full option string
    - `App.Restart`, `bool`, optional - watch to app stop event and restart application if it was closed, default - `false`
    - `App.RestartLimit`, `uint`, optional - application restarts limit, when it reached Logon Service will stop restarting application, `0` - no limit, default - `10`
    - `LogEnabled`, `bool`, optional - turn on/off logging, default - `false`
    - `LogPath`, `string`, optional - full file path to log file (default: `<exe dir>/log.txt` - not current dir, because for service it will be `system32`)
    - `Description`, `string`, optional - service description
    - `DisplayName`, `string`, optional - service display name
    - `ServiceName`, `string`, optional - service system level name (no spaces!)
1. Install service in command line:
```cmd
LogonService.exe -install
```

## Operating systems support

- Windows 7, 8.1, 10, 11

## Requirements

- DotNet 4.6.1, 4.8.0, 4.8.1

What about newer dotNet versions? Until dotNet 4.8.1 support in OS is dropped no migrations is planned. Newer dotNet service requires to install additional dependencies and that makes service heavy without any reason.

## Run multiple applications

Use next pattern for options naming in configuration file to run any number of applications: `<option><delimiter><application unique id>`.
Short example: `App Info`, `App.Arguments Info`, `App.Restart Info`, `App.RestartLimit Info` and etc.
Full example:
```xml
<add key="App Info"
     value="C:\adm\LockScreenText.exe"/>
<add key="App.Arguments Info"
     value='"-postion top-left" "-time HH:mm:ss" -pcname'/>
<add key="App.Restart Info"
     value="true"/>
<add key="App.RestartLimit Info"
     value="100"/>
<add key="App Password recovery"
     value="C:\adm\pwdrecovery.exe"/>
<add key="App.Restart Password recovery"
     value="true"/>
<add key="App.RestartLimit Password recovery"
     value="0"/>
```

Supported delimiters list between property key and application ID: ```\t !\\\"#$%&'()*+,-./|:;?[\\]^_`{|}~``` - feel free to use whatever you like.

## Config apply

Logon Service check last write date of configuration and reloads it at start and logon events.

## Available commands

- `-i` `/i` `-install` `/install` - install and start service
- `-u` `/u` `-uninstall` `/uninstall` - stop and uninstall service
- `-r` `/r` `-reinstall` `/reinstall` - reinstall service
- `-s` `/s` `-status` `/status` - service status, possible values: `Stopped` | `StartPending` | `StopPending` | `Running` | `ContinuePending` | `PausePending` | `Paused` | `NotInstalled`
- `-start` `/start` - start service
- `-stop` `/stop` - stop service
- `-restart` `/restart` - restart service

## Security

Run classic applications at logon screen can create security issues - be very attentive when you do that. Run only specific applications at logon screen with limited features. Installed Logon Service doesn't creates new security issue because it can run applications at logon screen - this can be done by any application in system.

---

# Russian

## Как использовать

1. Скачать архив для вашей платформы на странице [Releases](https://github.com/VoidVolker/Windows-logon-service/releases) (для x64 скачивайте сборку x64, т.к. х86 не будет работать из-за ограничений системных вызовов)
1. Распаковать в `Program files/LogonService` или любой другой каталог
1. Внести настройки в файл конфигурации `LogonService.exe.config`:
    - `App`, `string`, обязательно - приложение для запуска, полный путь или относительный путь каталога, в котором находится `LogonService.exe`, оба вида слэшей поддерживаются (`/` или `\`)
    - `App.Arguments`, `string`, опционально - аргументы приложения, дляпередачи аргументов с пробелами используйте одинарные кавычки для опции и двойные кавычки для самих аргументов, если в аргументах нет пробелов - то используйте пробел для разделения аргументов и стандартные двойные кавычки для всей строки опции
    - `App.Restart`, `bool`, опционально - автоматически перезапускать приложение при его остановке, по умолчанию - `false`
    - `App.RestartLimit`, `uint`, опционально - ограничения числа перезапусков приложения, `0` - неограниченное число, по умолчанию - `10`
    - `LogEnabled`, `bool`, опционально - включить/выключить лог, по умолчанию - `false`
    - `LogPath`, `string`, опционально - полный путь до лог-файла (по-умолчанию: `<каталог сервиса>/log.txt` - текущий каталог не используется, т.к. для сервисов это будет каталог `system32`)
    - `Description`, `string`, опционально - описание сервиса
    - `DisplayName`, `string`, опционально - название сервиса
    - `ServiceName`, `string`, опционально - системное название сервиса (без пробелов!)
1. Установить сервис в коммандной строке:
```cmd
LogonService.exe -install
```

## Поддерживаемые ОС

- Windows 7, 8.1, 10, 11

## Требования

- DotNet 4.6.1, 4.8.0, 4.8.1

Что на счет более новых версии dotNet? До тех пор, пока ОС будут поддерживать dotNet 4.8.1, миграция проекта не планируется. Более новые версии dotNet для разработки сервиса требуют устанавливать дополнительные зависимости, а это утяжелит и усложнит сервис.

## Запуск нескольких приложений

Используйте следующий паттерн в файле настроек для запуска нескольких приложений: `<настройка><разделитель><уникальный идентификатор приложения>`.
Краткий пример: `App Info`, `App.Arguments Info`, `App.Restart Info`, `App.RestartLimit Info` и т.д.
Полный пример:
```xml
<add key="App Info"
     value="C:\adm\LockScreenText.exe"/>
<add key="App.Arguments Info"
     value='"-postion top-left" "-time HH:mm:ss" -pcname'/>
<add key="App.Restart Info"
     value="true"/>
<add key="App.RestartLimit Info"
     value="100"/>
<add key="App Password recovery"
     value="C:\adm\pwdrecovery.exe"/>
<add key="App.Restart Password recovery"
     value="true"/>
<add key="App.RestartLimit Password recovery"
     value="0"/>
```

Список поддерживаемых разделителей между названием опции и идентификатором приложения: ```\t !\\\"#$%&'()*+,-./|:;?[\\]^_`{|}~``` - используйте любой по вашему вкусу.

## Применение настроек

Сервис в момент запуска и активации экрана входа проверяет дату изменения файла конфигурации и подгружает изменения в память.

## Доступные команды

- `-i` `/i` `-install` `/install` - установить и запустить сервис
- `-u` `/u` `-uninstall` `/uninstall` - остановить и удалить сервис
- `-r` `/r` `-reinstall` `/reinstall` - переустановить сервис
- `-s` `/s` `-status` `/status` - статус сервиса, возможные значения: `Stopped` | `StartPending` | `StopPending` | `Running` | `ContinuePending` | `PausePending` | `Paused` | `NotInstalled`
- `-start` `/start` - запустить сервис
- `-stop` `/stop` - остановить сервис
- `-restart` `/restart` - перезапустить сервис

## Безопасность

Запуск классических приложений на экране входа может создать угрозу безопасности системы - будьте аккуратны в выборе запускаемых приложений. Рекомендуется запускать приложения только с ограничениями. Установленный сервис не создаёт дополнительных угроз безопасности системы - используются стандартные вызовы WinAPI, которые могут быть использованы любым приложением в системе.

# Sponsors / Спонсоры

Big thanks to all open source software sponsors - you make this world better =)

Большое спасибо всем спонсорам приложений с открытым исходным кодом - вы делаете этот мир лучше =)

1. [jazir555](https://github.com/jazir555)
