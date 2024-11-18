using System.Diagnostics;
using System.IO;

namespace LogonService
{
    /// <summary>
    /// LogonUI app control and info
    /// </summary>
    public static class LogonUI
    {
        /// <summary>
        /// Process name without extension
        /// </summary>
        public static readonly string ProcName = "LogonUI";
        /// <summary>
        /// Process full name with extension
        /// </summary>
        public static readonly string ProcFullName = ProcName + ".exe";
        /// <summary>
        /// Full process path
        /// </summary>
        public static readonly string ProcPath = Path.GetFullPath(@"C:\Windows\System32\" + ProcFullName);

        /// <summary>
        /// Check if current system in logon mode
        /// </summary>
        /// <returns></returns>
        public static bool IsLogonMode() =>
            Native.IsNotShuttingDown() & Util.IsProcExist(ProcName, ProcPath);

        /// <summary>
        /// Hide logon window
        /// </summary>
        public static void WindowHide()
        {
            if (TryProc() is Process logonProc)
            {
                WindowHide(logonProc);
            }
        }

        /// <summary>
        /// Show logon window
        /// </summary>
        public static void WindowShow()
        {
            if (TryProc() is Process logonProc)
            {
                WindowShow(logonProc);
            }
        }

        /// <summary>
        /// Hide logon window using it's process ID
        /// </summary>
        /// <param name="pid">Process ID</param>
        public static void WindowHide(int pid)
        {
            if (Util.TryProc(pid) is Process logonProc)
            {
                WindowHide(logonProc);
            }
        }

        /// <summary>
        /// Show logon window using it's process ID
        /// </summary>
        /// <param name="pid"></param>
        public static void WindowShow(int pid)
        {
            if (Util.TryProc(pid) is Process logonProc)
            {
                WindowShow(logonProc);
            }
        }

        /// <summary>
        /// Hide logon window using it's process description
        /// </summary>
        /// <param name="logonProc"></param>
        public static void WindowHide(Process logonProc)
        {
            if (Native.IsWindowVisible(logonProc.MainWindowHandle))
            {
                Native.WindowHide(logonProc.MainWindowHandle);
            }
        }

        /// <summary>
        /// Show logon window using it's process description
        /// </summary>
        /// <param name="logonProc"></param>
        public static void WindowShow(Process logonProc)
        {
            if (!Native.IsWindowVisible(logonProc.MainWindowHandle))
            {
                Native.WindowShow(logonProc.MainWindowHandle);
            }
        }

        /// <summary>
        /// Trye to find logonUI process
        /// </summary>
        /// <returns></returns>
        public static Process TryProc() => Util.TryProc(ProcFullName, ProcPath);
    }
}
