using System;
using System.Configuration;

namespace LogonService
{
    internal static class AppConfig
    {
        public static string OnLogon = Get("OnLogon", "");
        public static string LogEnabled = Get("LogEnabled", "false");
        public static string LogPath = Get("LogPath", $"{AppDomain.CurrentDomain.BaseDirectory}log.txt");
        public static string Description = Get("Description", "Logon Service for running applications on logon screen");
        public static string DisplayName = Get("DisplayName", "Logon Service");
        public static string ServiceName = Get("ServiceName", "LogonService");

        public static string Get(string prop, string defProp = null)
        {
            var value = ConfigurationManager.AppSettings[prop];
            if (string.IsNullOrEmpty(value)) { value = defProp; }
            return value;
        }
    }
}
