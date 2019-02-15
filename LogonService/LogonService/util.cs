using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LogonService
{
    static class Util
    {        
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        public static bool IsProcExist(string name)
        {
            Process[] arrProcesses = Process.GetProcessesByName(name);
            return arrProcesses.Length > 0;
        }

        public static Process TryProc(string name)
        {
            Process[] arrProcesses = Process.GetProcessesByName(name);
            if (arrProcesses.Length > 0)
            {
                return arrProcesses[0];
            }
            else
            {
                return null;
            }           
        }

        public static bool IsProcWinVisible(Process proc)
        {
            return IsWindowVisible(proc.MainWindowHandle);
        }

        public static bool IsProcWinVisible(string name)
        {
            Process[] arrProcesses = Process.GetProcessesByName(name);
            bool r = false;
            if (arrProcesses.Length > 0)
            {
                for (var i = 0; i < arrProcesses.Length; i++)
                {
                    if (!r)
                    {
                        r = IsWindowVisible(arrProcesses[i].MainWindowHandle);
                    }
                }
            }
            return r;
        }
    }
}
