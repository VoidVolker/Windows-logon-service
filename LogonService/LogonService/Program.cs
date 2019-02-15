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
                switch (args[0].Substring(1).ToLower())
                {
                    case "install":
                    case "i":
                        if (ServiceInstaller.Install())
                        {
                            Console.WriteLine("Service installed");
                            Process.Start("net", "start logonService");
                        }
                        else
                        {
                            Console.WriteLine("Failed to install service");
                        }

                        break;
                    case "uninstall":
                    case "u":
                        if (ServiceInstaller.Uninstall())
                        {
                            Console.WriteLine("Service uninstalled");
                        }
                        else
                        {
                            Console.WriteLine("Failed to uninstall service");
                        }

                        break;
                    default:
                        Console.WriteLine("Unrecognized parameters.\n\n    -i /i -install /install — install service\n\n    -u /u -uninstall /uninstall — uninstall service");
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
