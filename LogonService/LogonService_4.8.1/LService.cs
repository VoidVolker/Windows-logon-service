using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
//using System.Windows.Forms;

namespace LogonService
{
    public partial class LService : ServiceBase
    {
        private const int WatchInterval = 100; // ms

        ApplicationLoader.PROCESS_INFORMATION procInfo;
        //mainForm mForm;
        static bool isConsole = false;
        private readonly Thread appThread;
        private bool isActive = true;
        private string logPath;
        private bool isLogging = false;

        public string execApp;
        public string execAppPath;

        Process executingProc = null;
        //readonly Command cmd = Command.Execute;

        //public bool CanShutdown = true;
        //public bool CanStop = true;

        public LService(bool isCon)
        {
            isConsole = isCon;
            InitializeComponent();

            //Get log path
            logPath = AppConfig.LogPath;
            // Turning log on or off
            isLogging = bool.Parse(AppConfig.LogEnabled);

            //var command = AppConfig.OnLogon;
            //switch (command)
            //{
            //    default:
            //        cmd = Command.Execute;
            //        execAppPath = command;
            //        execApp = Path.GetFileNameWithoutExtension(execAppPath);
            //        appThread = new Thread(Execute_Proc);
            //        break;
            //}

            // Configuring app to run

            execAppPath = AppConfig.OnLogon;
            execApp = Path.GetFileNameWithoutExtension(execAppPath);
            appThread = new Thread(Execute_Proc);

            appThread.SetApartmentState(ApartmentState.STA);
        }

        void Log(string str)
        {
            if (isLogging)
            {
                File.AppendAllText(logPath, $"[{DateTime.Now}] {str}\n");
            }
        }

        void Execute_Proc()
        {
            while (isActive)
            {
                executingProc = Util.TryProc(execApp);

                if (LogonUI.IsLogonMode())
                {
                    if (executingProc == null)
                    {
                        Log($"Logon mode detected - application not running: starting application");
                        ApplicationLoader.StartProcessAndBypassUAC(execAppPath, out procInfo);
                    }
                    else if (ApplicationLoader.IsNewSession())
                    {
                        Log($"Logon mode detected - application is running: new session detected");

                        try
                        {
                            Log($"Killing application");
                            executingProc.Kill();
                        }
                        catch (Exception e)
                        {
                            Log(e.Message);
                            Log(e.StackTrace);
                        }
                        Log($"Application killed: starting application from scratch");
                        ApplicationLoader.StartProcessAndBypassUAC(execAppPath, out procInfo);
                    }
                }
                else
                {
                    if (executingProc != null)
                    {
                        Log($"User logged in detected: killing application");
                        try
                        {
                            executingProc.Kill();
                        }
                        catch (Exception e)
                        {
                            Log(e.Message);
                            Log(e.StackTrace);
                        }
                    }
                }
                executingProc = null;
                Thread.Sleep(WatchInterval);
            }
        }

        public void Start()
        {
            if (string.IsNullOrEmpty(execAppPath))
            {
                Log($"Service is started. No comand found to execute in App.config - main thread will not start.");
            }
            else
            {
                Log($"Service is started. Founded comand from settings: '{execAppPath}'");
                //Log($"Service is started. Founded comand from settings: {cmd}");
                appThread.Start();
            }
        }

        public void LogonWatcherStop()
        {
            isActive = false;
            if (executingProc != null)
            {
                try
                {
                    executingProc.Kill();
                }
                catch (Exception)
                {
                }
                executingProc = null;
            }
        }

        protected override void OnStart(string[] args)
        {
            Start();
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            //Log($"OnStop()");
            LogonWatcherStop();
            //Stop();
            base.OnStop();
        }

        protected override void OnShutdown()
        {
            //Log($"OnShutdown()");
            // Add your save code here
            LogonWatcherStop();
            Stop();
            base.OnShutdown();
        }

        //enum Command : byte
        //{
        //    None,
        //    Execute,
        //    LogonUI_WindowHide
        //}
    }
}
