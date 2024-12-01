using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;

namespace LogonService
{
    public static class Args
    {
        private const string NotInstalled = "NotInstalled";
        private static TimeSpan ServiceTimeout = TimeSpan.FromSeconds(10);

        public static void Parse(
            string[] args,
            Action install,
            Action uninstall,
            Action<bool> run
        )
        {
            try
            {
                if (IsCommand(args, out string command))
                {
                    //string serviceName = AppConfig.ServiceName;
                    bool isInstalled = Util.IsServiceInstalled(AppConfig.ServiceName);

                    switch (command)
                    {
                        case "install":
                        case "i":
                            if (isInstalled)
                            {
                                Console.WriteLine($"Failed to install service: service already installed");
                                return;
                            }
                            ServiceInstall(install);
                            break;

                        case "uninstall":
                        case "u":
                            if (IfNotInstalledWarning(isInstalled, "uninstall"))
                            {
                                ServiceUninstall(uninstall);
                            }
                            break;

                        case "reinstall":
                        case "r":
                            if (IfNotInstalledWarning(isInstalled, "reinstall"))
                            {
                                ServiceReinstall(install, uninstall);
                            }
                            break;

                        case "status":
                        case "s":
                            ShowStatus(isInstalled);
                            break;

                        case "start":
                            if (IfNotInstalledWarning(isInstalled, command))
                            {
                                ServiceStart();
                            }
                            break;

                        case "stop":
                            if (IfNotInstalledWarning(isInstalled, command))
                            {
                                ServiceStop();
                            }
                            break;

                        case "restart":
                            if (IfNotInstalledWarning(isInstalled, command))
                            {
                                ServiceRestart();
                            }
                            break;

                        default:
                            ShowHelp(args);
                            break;
                    }
                }
                else
                {
                    run(Environment.UserInteractive);
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"InvalidOperationException: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IOException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not start service due application error: {ex.Message}; {ex.StackTrace}");
            }
            //catch
            //{
            //    //log.Fatal("Could not start polling service due to unknown error", ex);
            //    Console.WriteLine("Could not start service due to unknown error");
            //}
        }

        private static bool IsCommand(string[] args, out string command)
        {
            if (
                args != null &&
                args.Length == 1 &&
                args[0].Length > 1)
            {
                char dl = args[0][0];
                if (dl == '-' || dl == '/' || dl == '\\' || dl == '|')
                {
                    command = args[0].Substring(1).ToLower(); // Drop delimiter
                    return true;
                }
            }
            command = string.Empty;
            return false;
        }

        private static bool IfNotInstalledWarning(bool isInstalled, string operation)
        {
            if (!isInstalled)
            { Console.WriteLine($"Failed to {operation} service: service not installed"); }
            return isInstalled;
        }

        private static bool IfStatusWarning(string action, ServiceControllerStatus current, ServiceControllerStatus expected)
        {
            if (current != expected)
            {
                Console.WriteLine($"Failed to {action} service - service wrong status. Expected: {expected}; got: {current}");
                return true;
            }
            return false;
        }

        private static void ServiceInstall(Action install)
        {
            install();
            //Util.NetStartService(serviceName);
            ServiceController service = new ServiceController(AppConfig.ServiceName);
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, ServiceTimeout);

            Console.WriteLine("Service installed and started");
        }

        private static void ServiceUninstall(Action uninstall)
        {
            //Util.NetStopService(serviceName);
            ServiceController service = new ServiceController(AppConfig.ServiceName);

            if (service.Status == ServiceControllerStatus.Running)
            {
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, ServiceTimeout);
            }

            uninstall();

            Console.WriteLine("Service stopped and uninstalled");
        }

        private static void ServiceReinstall(Action install, Action uninstall)
        {
            ServiceController service = new ServiceController(AppConfig.ServiceName);

            //Util.NetStopService(serviceName);
            if (service.Status == ServiceControllerStatus.Running)
            {
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, ServiceTimeout);
            }

            uninstall();
            install();

            //Util.NetStartService(serviceName);
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, ServiceTimeout);

            Console.WriteLine("Service stopped, reinstalled and started");
        }


        private static void ServiceStart()
        {
            ServiceController service = new ServiceController(AppConfig.ServiceName);
            if (IfStatusWarning("start", service.Status, ServiceControllerStatus.Stopped))
            {
                return;
            }

            //Util.NetStartService(serviceName);
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, ServiceTimeout);

            Console.WriteLine("Service started");
        }

        private static void ServiceStop()
        {
            ServiceController service = new ServiceController(AppConfig.ServiceName);
            if (IfStatusWarning("stop", service.Status, ServiceControllerStatus.Running))
            {
                return;
            }

            //Util.NetStopService(serviceName);
            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped, ServiceTimeout);

            Console.WriteLine("Service stopped");
        }

        private static void ServiceRestart()
        {
            ServiceController service = new ServiceController(AppConfig.ServiceName);
            if (service.Status != ServiceControllerStatus.Running)
            {
                Console.WriteLine($"Failed to stop service: service isn't running. State: {service.Status}");
            }

            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped, ServiceTimeout);
            //service.Refresh();

            if (service.Status != ServiceControllerStatus.Stopped)
            {
                Console.WriteLine($"Failed to start service: service isn't stopped. State: {service.Status}");
                return;
            }

            //Util.NetStopService(serviceName);
            //Util.NetStartService(serviceName);

            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, ServiceTimeout);

            Console.WriteLine("Service restarted");
        }

        private static void ShowStatus(bool isInstalled)
        {
            if (isInstalled)
            {
                Console.WriteLine(new ServiceController(AppConfig.ServiceName).Status);
            }
            else
            {
                Console.WriteLine(NotInstalled);
            }
        }

        private static void ShowHelp(string[] args)
        {
            // Define help header
            string helpCommandMessage;
            if (args.Length == 0)
            {
                helpCommandMessage = "No command line arguments founded.";
            }
            else
            {
                // Combine arguments list to one string
                string arguments = string.Join("\n", args);
                helpCommandMessage = $"Unrecognized command line arguments:\n{arguments}\n\n";
            }

            // Get statuses list
            List<string> statuses = Enum.GetNames(typeof(ServiceControllerStatus)).ToList();
            // Add "NotInstalled" status
            statuses.Add(NotInstalled);
            // Combine to one string
            string serviceStates = string.Join(" | ", statuses);

            // Show help:
            Console.WriteLine(helpCommandMessage +
                " Available commands:\n" +
                "    -i /i -install /install — install and start service\n" +
                "    -u /u -uninstall /uninstall — stop and uninstall service\n" +
                "    -r /r -reinstall /reinstall — reinstall service\n" +
                $"    -s /s -status /status — service status, possible values: {serviceStates}\n" +
                "    -start /start — start service\n" +
                "    -stop /stop — stop service\n" +
                "    -restart /restart — restart service\n"
            );
        }

    }
}
