using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace LogonService
{
    public static class Util
    {
        public static bool IsProcExist(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);
            return processes.Length > 0;
        }

        public static bool IsProcExist(string name, string fullPath)
        {
            Process[] processes = Process.GetProcessesByName(name);
            foreach (var proc in processes)
            {
                if (fullPath.Equals(proc.MainModule?.FileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static Process TryProc(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);
            return processes.Length > 0 ? processes[0] : null;
        }

        public static Process TryProc(string name, string fullPath)
        {
            Process[] processes = Process.GetProcessesByName(name);
            foreach (var proc in processes)
            {
                if (fullPath.Equals(proc.MainModule?.FileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return proc;
                }
            }
            return null;
        }

        public static Process TryProc(int id)
        {
            try
            {
                return Process.GetProcessById(id);
            }
            catch
            {
                return null;
            }
        }

        public static void RunProcWait(string app, string args = "")
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo(app, args)
                {
                    UseShellExecute = false
                }
            };
            proc.Start();
            proc.WaitForExit();
        }

        public static ProcResult RunProcOutput(string app, string args = "", Encoding encoding = null)
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo(app, args)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = encoding,
                    StandardErrorEncoding = encoding
                }
            };

            proc.Start();
            ProcResult result = new ProcResult()
            {
                Output = proc.StandardOutput.ReadToEnd(),
                Error = proc.StandardError.ReadToEnd()
            };
            proc.WaitForExit();

            return result;
        }

        public static void NetStartService(string serviceName)
        {
            RunProcWait("net", $"start {serviceName}");
        }

        public static void NetStopService(string serviceName)
        {
            RunProcWait("net", $"stop {serviceName}");
        }

        public static bool IsServiceInstalled(string serviceName) =>
            ServiceController.GetServices().Any(s => s.ServiceName == serviceName);

        public class ProcResult
        {
            public string Output { get; internal set; }
            public string Error { get; internal set; }
        }
    }
}
