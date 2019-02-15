using System.Runtime.InteropServices;

namespace LogonService
{
    static class LogonUI
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern int GetSystemMetrics(int nIndex);
        const int SM_SHUTTINGDOWN = 0x2000;

        static readonly string LogonUIexe = "LogonUI.exe";
        static readonly string logonPath = @"C:\Windows\System32\" + LogonUIexe;

        public static bool IsLogonMode()
        {
            return (GetSystemMetrics(SM_SHUTTINGDOWN) == 0) & Util.IsProcExist("LogonUI");
        }
    }
}
