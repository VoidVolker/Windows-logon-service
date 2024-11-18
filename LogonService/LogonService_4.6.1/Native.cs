using System;
using System.Runtime.InteropServices;

namespace LogonService
{
    public static class Native
    {
        [DllImport("User32")]
        static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        public static void WindowShow(IntPtr hwnd) => ShowWindow(hwnd, Const.SW_SHOW);
        public static void WindowHide(IntPtr hwnd) => ShowWindow(hwnd, Const.SW_HIDE);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int ProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetSystemMetrics(int nIndex);

        /// <summary>
        /// Check if system is shutting down
        /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getsystemmetrics
        /// </summary>
        /// <returns></returns>
        public static bool IsShuttingDown() =>
            GetSystemMetrics(Const.SM_SHUTTINGDOWN) != 0;

        /// <summary>
        /// Check if system is not shutting down
        /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getsystemmetrics
        /// </summary>
        /// <returns></returns>
        public static bool IsNotShuttingDown() =>
            GetSystemMetrics(Const.SM_SHUTTINGDOWN) == 0;

        // Windows API constants list
        public static class Const
        {
            public const int SW_HIDE = 0;
            public const int SW_SHOW = 5;
            public const int SM_SHUTTINGDOWN = 0x2000;
        }
    }
}
