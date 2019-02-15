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
//using System.Windows.Forms;

namespace LogonService
{
    public partial class LService : ServiceBase
    {
        ApplicationLoader.PROCESS_INFORMATION procInfo;
        //mainForm mForm;
        static bool isConsole = false;
        private readonly Thread appThread;
        private bool isActive = true;
        public static string logonApp = "LogonUI";
        public static string logonAppExe = logonApp + ".exe";
        public static string logonAppPath;
        Process helperProc = null;

        //public bool CanShutdown = true;
        //public bool CanStop = true;

        public LService(bool isCon)
        {
            isConsole = isCon;
            InitializeComponent();
            appThread = new Thread(App);
            appThread.SetApartmentState(ApartmentState.STA);
        }

        void App()
        {            
            logonAppPath = AppDomain.CurrentDomain.BaseDirectory + logonAppExe;
            

            while (isActive)
            {                
                helperProc = Util.TryProc(logonApp);

                if (LogonUI.IsLogonMode())
                {
                    if (helperProc == null)
                    {
                        ApplicationLoader.StartProcessAndBypassUAC(logonAppPath, out procInfo);

                    }
                    else if (ApplicationLoader.IsNewSession())
                    {
                        try
                        {                            
                            helperProc.Kill();
                        }
                        catch (Exception)
                        {                         
                        }
                        ApplicationLoader.StartProcessAndBypassUAC(logonAppPath, out procInfo);
                    }
                }
                else
                {
                    if (helperProc != null)
                    {
                        try
                        {
                            helperProc.Kill();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                helperProc = null;
                Thread.Sleep(100);
            }
        }

        public void Start()
        {
            appThread.Start();
        }

        public void HStop()
        {
            isActive = false;
            if (helperProc != null)
            {
                try
                {
                    helperProc.Kill();
                }
                catch (Exception)
                {
                }
                helperProc = null;
            }
        }

        protected override void OnStart(string[] args)
        {
            Start();
        }

        protected override void OnStop()
        {
            Stop();
        }

        protected override void OnShutdown()
        {
            // Add your save code here
            Stop();
            base.OnShutdown();
        }        
    }
}
