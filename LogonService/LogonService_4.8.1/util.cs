using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace LogonService
{
    static class Util
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);


        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        [DllImport("User32")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

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
    }
}
