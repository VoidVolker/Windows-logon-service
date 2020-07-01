# Windows Logon service

This service is able to run any application on user login screen. I spend several weeks to collect all solutions from internet into one working project. Using specific WinAPI calls this service collects info about logon session and using this info runs application in logon session. PR are welcome!

# How to use
1. Find in code LogonUI string and replace to your application
2. Build solution
3. Install service

# Tested in next Operating systems:
Windows XP SP3, 7, 8.1, 10

TODO:
1. Write documentation (just run in console)
2. Write documentation for code
3. Move running aplication path to settings for standalone use
4. Make code more logical (?)
5. Publish in repository as library (?)
