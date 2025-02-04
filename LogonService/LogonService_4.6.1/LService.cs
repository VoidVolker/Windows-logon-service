﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;

using static LogonService.ProcWMI;

namespace LogonService
{
    /// <summary>
    /// Main service logic
    /// </summary>
    public partial class LService : ServiceBase
    {
        //Created via constructor and added some specific options wich I don't remember :(

        #region Public Constructors

        public LService(bool isConsole) : this()
        {
            IsConsole = isConsole;
        }

        public LService()
        {
            //isConsole = isCon;
            InitializeComponent();

            // Reload config
            AppConfig.LoadIfUpdated();

            eventLog.Source = AppConfig.ServiceName;
            ServiceName = AppConfig.ServiceName;

            // Subscribe to events
            ProcWatcher.OnStart(LogonUI.ProcFullName, LogonUIStarted);
            ProcWatcher.OnStop(LogonUI.ProcFullName, LogonUIStopped);
        }

        #endregion Public Constructors

        #region Private Fields

        private readonly bool IsConsole = false;
        private readonly ProcWMI ProcWatcher = new ProcWMI();

        // LogonUI process ID - required for catching it's stop
        private uint LogonPid = 0;

        #endregion Private Fields

        #region Public Methods

        public void LogonServiceStart()
        {
            ProcWatcher.Start();
            Log("[STARTED]");
        }

        public void LogonServiceStop()
        {
            ProcWatcher.Stop();
            StopApps();
            Log("[STOPPED]");
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnShutdown()
        {
            LogonServiceStop();
            Stop();
            base.OnShutdown();
        }

        protected override void OnStart(string[] args)
        {
            LogonServiceStart();
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            LogonServiceStop();
            base.OnStop();
        }

        #endregion Protected Methods

        #region Private Methods

        private void Log(string str) => AppConfig.Log(IsConsole, str);

        private void LogonMode()
        {
            // Logon session mode
            Log("[LOGON MODE]");

            // Reload config
            AppConfig.LoadIfUpdated();

            // Run applications
            foreach (AppConfig.App app in AppConfig.Apps)
            {
                RunApp(app);
            }
        }

        private void LogonUIStarted(uint pid, ProcEventArgs ev)
        {
            // Do nothing on shutting down / reboot
            if (Native.IsShuttingDown()) { return; }

            // Get process info
            Process proc = Process.GetProcessById((int)pid);
            string procPath = proc.MainModule.FileName;
            // Check process full path
            if (!procPath.Equals(LogonUI.ProcPath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Save pid to catch stopping exactly this process (for rare case when user runs random app with same name)
            LogonPid = (uint)proc.Id;

            LogonMode();
        }

        private void LogonUIStopped(uint pid, ProcEventArgs ev)
        {
            if (pid != LogonPid) { return; }

            LogonPid = 0;
            UserMode();
        }

        private void RunApp(AppConfig.App app)
        {
            // Check app path
            if (string.IsNullOrEmpty(app.Path))
            {
                Log($"[NOTHING TO START] [{app.Id}]: application path is empty");
                return;
            }

            // Check app exists
            if (!File.Exists(app.Path))
            {
                Log($"[FILE NOT FOUND] [{app.Id}]: {app.Path}");
                return;
            }

            // Create new proc info struct
            app.ProcInfo = new ApplicationLoader.PROCESS_INFORMATION();

            // Try process start
            if (ApplicationLoader.StartProcessAndBypassUAC(app.Command, out app.ProcInfo))
            {
                Log($"[STARTED] [{app.Id}] process/thread IDs: {app.ProcInfo.dwProcessId}/{app.ProcInfo.dwThreadId}");
                if (app.IsRestart)
                {
                    // Restart limit logic
                    if (app.RestartCounter >= app.RestartLimit)
                    {
                        app.RestartCounter = 0;
                        Log($"[RESTART LIMIT REACHED ({app.RestartLimit})] [{app.Id}]");
                        return;
                    }

                    ProcWatcher.OnIdStop(
                        app.ProcInfo.dwProcessId,
                        procData =>
                        {
                            app.RestartCounter++;
                            Log($"[STOPPED, RESTARTING, TRY #{app.RestartCounter}] [{app.Id}] process/thread IDs: {app.ProcInfo.dwProcessId}/{app.ProcInfo.dwThreadId}");
                            RunApp(app);
                        });
                }
            }
            else
            {
                Log($"[FAILED TO START] [{app.Id}] GetLastError(): '{ApplicationLoader.GetLastError()}'");
            }
        }

        private void StopApps()
        {
            // Try kill all running apps
            foreach (AppConfig.App app in AppConfig.Apps)
            {
                app.RestartCounter = 0;

                if (EqualityComparer<ApplicationLoader.PROCESS_INFORMATION>.Default.Equals(app.ProcInfo, default))
                { continue; }

                if (Util.TryProc((int)app.ProcInfo.dwProcessId) is Process proc)
                {
                    ProcWatcher.IdStopRemove(proc.Id);
                    proc.Kill();
                    Log($"[STOPPED] [{app.Id}] using PID: {proc.Id}");
                }
                app.ProcInfo = default;
            }
        }

        private void UserMode()
        {
            // User session mode
            Log("[USER MODE]");
            StopApps();
        }

        #endregion Private Methods
    }
}