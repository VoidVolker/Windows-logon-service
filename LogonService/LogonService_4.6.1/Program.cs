using System;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace LogonService
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                AppConfig.LoadIfUpdated();
            }
            catch (Exception e)
            {
                AppConfig.Log(true, $"Configuration loading error: {e.Message}");
                return;
            }

            Args.Parse(
                args,
                ServiceInstaller.Install,
                ServiceInstaller.Uninstall,
                (isConsole) =>
                {
                    LService service = new LService(isConsole);
                    if (isConsole)
                    {
                        Console.OutputEncoding = Encoding.UTF8;
                        Console.CancelKeyPress += (x, y) => service.Stop();

                        Console.WriteLine("Service started in interactive debug mode. Log forced to console using encoding UTF8. Press any key to stop.");
                        if (args.Length > 0 && args[0].Length > 0)
                        {
                            Console.WriteLine($"Detected arguments list:");
                            for (int i = 0; i < args.Length; i++)
                            {
                                Console.WriteLine($"{i}: {args[i]}");
                            }
                        }

                        try
                        {
                            service.LogonServiceStart();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Initialization error: {e.Message}");
                            Console.WriteLine(e.StackTrace);
                            throw;
                        }

                        Console.ReadKey();
                        service.LogonServiceStop();
                        Console.WriteLine("Service stopped");
                    }
                    else
                    {
                        ServiceBase[] servicesToRun = new ServiceBase[] { service };
                        ServiceBase.Run(servicesToRun);
                    }
                }
            );
        }

    }
}
