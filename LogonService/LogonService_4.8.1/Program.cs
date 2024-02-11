using System;
using System.IO;
using System.ServiceProcess;
using System.Diagnostics;

namespace LogonService
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main(string[] args)
        {
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[] 
            //{ 
            //    new adsrpService() 
            //};
            //ServiceBase.Run(ServicesToRun);
            if (args != null && args.Length == 1 && args[0].Length > 1
                && (args[0][0] == '-' || args[0][0] == '/'))
            {
                var serviceName = AppConfig.ServiceName;

                switch (args[0].Substring(1).ToLower())
                {
                    case "install":
                    case "i":
                        try
                        {
                            ServiceInstaller.Install();
                            Process.Start("net", $"start {serviceName}");
                            Console.WriteLine("Service installed and started");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Failed to install service");
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                        }
                        break;

                    case "uninstall":
                    case "u":
                        try
                        {
                            Process.Start("net", $"stop {serviceName}");
                            ServiceInstaller.Uninstall();
                            Console.WriteLine("Service stopped and uninstalled");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Failed to uninstall service");
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                        }
                        break;

                    case "reinstall":
                    case "r":
                        try
                        {
                            Process.Start("net", $"stop {serviceName}");
                            ServiceInstaller.Uninstall();
                            ServiceInstaller.Install();
                            Process.Start("net", $"start {serviceName}");
                            Console.WriteLine("Service stopped and uninstalled");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Failed to uninstall service");
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                        }
                        break;


                    case "start":
                        Process.Start("net", $"start {serviceName}");
                        break;

                    case "stop":
                        Process.Start("net", $"stop {serviceName}");
                        break;

                    default:
                        Console.WriteLine("Unrecognized parameters.\n\n" +
                            "    -i /i -install /install — install and start service\n\n" +
                            "    -u /u -uninstall /uninstall — stop and uninstall service\n\n" +
                            "    -r /r -reinstall /reinstall — reinstall service\n\n" +
                            "    -start /start — start service\n\n" +
                            "    -stop /stop — stop service\n\n"
                        );
                        break;
                }
            }
            else
            {
                try
                {
                    //bool isConsole = Environment.UserInteractive;
                    LService service = new LService(false);
                    ServiceBase[] servicesToRun = new ServiceBase[] { service };

                    //if (isConsole)
                    //{
                    //    Console.OutputEncoding = Encoding.UTF8;
                    //    Console.CancelKeyPress += (x, y) => service.Stop();
                    //    try
                    //    {
                    //        service.start();
                    //    }
                    //    catch
                    //    {
                    //        Console.WriteLine("Initialization unknown error");
                    //        throw;
                    //    }

                    //    Console.WriteLine("Service started. Press any key to stop.");
                    //    Console.ReadKey();
                    //    service.stop();
                    //    Console.WriteLine("Service stopped. Good bye.");
                    //}
                    //else
                    //{
                    ServiceBase.Run(servicesToRun);
                    //}
                }
                catch (IOException ex)
                {
                    //log.Error("", ex);
                    Console.WriteLine("IOException", ex);
                }
                catch (Exception ex)
                {
                    //log.Fatal("Could not start polling service due to unknown error", ex);
                    Console.WriteLine("Could not start service due application error", ex);
                }
                //catch
                //{
                //    //log.Fatal("Could not start polling service due to unknown error", ex);
                //    Console.WriteLine("Could not start service due to unknown error");
                //}
            }
        }


    }
}
