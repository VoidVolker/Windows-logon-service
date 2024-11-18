using System;
using System.Collections.Generic;
using System.Configuration;
using IO = System.IO;
using System.Linq;

namespace LogonService
{
    /// <summary>
    /// Application configuration
    /// </summary>
    internal static class AppConfig
    {
        private static readonly string defaultLogPath = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
        // Delimiters list for application id
        private static readonly string delimiters = "\t !\\\"#$%&'()*+,-./|:;?[\\]^_`{|}~";

        // Options
        public static App[] Apps = new App[0];
        public static bool IsLogEnabled = false;
        public static string LogPath;
        public static string Description;
        public static string DisplayName;
        public static string ServiceName;

        // Config last write time - required for reloading when it changes
        public static DateTime LastUpdated = DateTime.MinValue;

        /// <summary>
        /// Load configuration if it not yet loaded OR reload configuration if it was updated on disk OR do nothing. 
        /// Call this methond only when service starts and switching to logon mode. Do not run config update during apps processing - original array will be destroyed.
        /// </summary>
        public static void LoadIfUpdated()
        {
            // Check file update date
            DateTime updated = IO.File.GetLastWriteTime(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            if (LastUpdated == updated)
            {
                return;
            }

            // Save update date to variable
            LastUpdated = updated;

            // Reload configuration
            ConfigurationManager.RefreshSection("appSettings");

            // Load values from config
            Apps = GetApps("App", "Restart", "RestartLimit");
            bool.TryParse(Get("LogEnabled", "false"), out IsLogEnabled);
            LogPath = GetPath("LogPath", defaultLogPath);
            Description = Get("Description", "Logon Service for running applications on logon screen");
            DisplayName = Get("DisplayName", "Logon Service");
            ServiceName = Get("ServiceName", "LogonService");
        }

        public static string Get(string prop, string defProp = null)
        {
            string value = ConfigurationManager.AppSettings[prop];
            return string.IsNullOrEmpty(value) ? defProp : value;
        }

        public static string GetPath(string prop, string defProp = null)
        {
            string value = ConfigurationManager.AppSettings[prop];

            if (string.IsNullOrEmpty(value)) { return defProp; }
            try
            {
                return IO.Path.GetFullPath(value);
            }
            catch (Exception e)
            {
                LogError($"Error while parsing path in property '{prop}' with value '{value}'. Error: {e.Message}");
                return defProp;
            }
        }

        public static Dictionary<string, string> GetArrayStartsWith(string prop)
        {
            // Get target property keys and values
            IEnumerable<string> list = ConfigurationManager.AppSettings.AllKeys
               .Where(key => key.StartsWith(prop));

            // Extract id from key and convert list to dictionary
            Dictionary<string, string> dict = list.ToDictionary(
                key =>
                {
                    // Key contains additional ID?
                    if (key.Length > prop.Length)
                    {
                        // Delimeter or id position
                        int idPos = prop.Length;
                        // If key contains delimeter - remove it from key too
                        if (delimiters.Contains(key[idPos])) { idPos++; }
                        // Get id from key
                        return key.Remove(0, idPos);
                    }
                    else
                    {
                        // Property doesn't contains any additional id and this is just one key-value pair
                        return string.Empty;
                    }
                },
                k => ConfigurationManager.AppSettings[k]);

            return dict;
        }

        public static App[] GetApps(string appKey, string watchKey, string limitKey)
        {
            Dictionary<string, string> paths = GetArrayStartsWith(appKey);
            Dictionary<string, string> watchFlags = GetArrayStartsWith(watchKey);
            Dictionary<string, string> limits = GetArrayStartsWith(limitKey);
            List<App> apps = new List<App>();
            int i = 0;

            foreach (KeyValuePair<string, string> p in paths)
            {
                // Get app id and path options values
                string id = p.Key == appKey ? IO.Path.GetFileNameWithoutExtension(p.Value) : p.Key // One app case - id is 
                    , path = p.Value
                    , watch = string.Empty
                    , limit = string.Empty;

                // Get app watch and limit options values
                if (string.IsNullOrEmpty(id)) // No app ID provided
                {
                    if (watchFlags.Count < i)
                    {
                        watch = watchFlags.ElementAt(i).Value;
                    }
                    if (limits.Count < i)
                    {
                        limit = limits.ElementAt(i).Value;
                    }
                }
                else
                {
                    watchFlags.TryGetValue(id, out watch);
                    limits.TryGetValue(id, out limit);
                }

                // Ignore empty path apps
                if (string.IsNullOrEmpty(path)) { continue; }

                // Check if app file exists
                if (!IO.File.Exists(path))
                {
                    LogError($"Error while parsing path in property '{appKey}' -> '{id}' with value '{path}'. Error: file not found.");
                    continue;
                }

                // One app case - id is missing in config
                if (id == appKey)
                {
                    id = IO.Path.GetFileNameWithoutExtension(path);
                }

                // Create app descriptor
                App app = new App(id, path, watch, limit);
                // Push app to array
                apps.Add(app);
            }

            return apps.ToArray();
        }

        public static void Log(bool isConsole, string str)
        {
            if (isConsole)
            {
                Console.WriteLine(str);
            }
            if (IsLogEnabled)
            {
                LogWrite(str);
            }
        }

        private static void LogWrite(string str) => IO.File.AppendAllText(LogPath, $"[{DateTime.Now}] {str}\r\n");

        private static void LogError(string str)
        {
            Console.WriteLine(str);
            LogWrite(str);
        }

        /// <summary>
        /// Application config and runtime descriptor
        /// </summary>
        public class App
        {
            /// <summary>
            /// Application ID or process name without extension
            /// </summary>
            public readonly string Id = string.Empty;
            /// <summary>
            /// File name with extension
            /// </summary>
            public readonly string File = string.Empty;
            /// <summary>
            /// Application full path
            /// </summary>
            public readonly string Path = string.Empty;
            /// <summary>
            /// Watch application stop and restart or not
            /// </summary>
            public readonly bool IsRestart = false;
            /// <summary>
            /// Limit for application restarts
            /// </summary>
            public readonly uint RestartLimit = 0;

            /// <summary>
            /// Restarts counter
            /// </summary>
            public uint RestartCounter = 0;
            /// <summary>
            /// Runtime process info
            /// </summary>
            public ApplicationLoader.PROCESS_INFORMATION ProcInfo;

            public App(string id, string path, string watch, string limit)
            {
                Path = IO.Path.GetFullPath(path);
                File = IO.Path.GetFileName(path);
                // Id or short file name
                Id = string.IsNullOrEmpty(id)
                    ? IO.Path.GetFileNameWithoutExtension(File)
                    : id;
                bool.TryParse(watch, out IsRestart);
                uint.TryParse(limit, out RestartLimit);
            }

            public override string ToString() => Id;
        }
    }
}
