using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using IO = System.IO;

namespace LogonService
{
    /// <summary>
    /// Application configuration
    /// </summary>
    public static class AppConfig
    {
        #region Public Fields

        static AppConfig()
        {
            LogPath = defaultLogPath = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
            LastUpdated = DateTime.MinValue;
        }

        // Options
        public static App[] Apps = new App[0];

        public static string Description;
        public static string DisplayName;
        public static bool IsLogEnabled = false;

        // Config last write time - required for reloading when it changes
        public static DateTime LastUpdated;

        public static string LogPath;
        public static string ServiceName;

        #endregion Public Fields

        #region Private Fields

        private static readonly string defaultLogPath;

        //// Delimiters list for application id
        //private static readonly string delimiters = "\t !\\\"#$%&'()*+,-./|:;?[\\]^_`{|}~";
        private static readonly string idDelimiters = " ";

        #endregion Private Fields

        #region Public Methods

        public static string Get(string prop, string defProp = null)
        {
            string value = ConfigurationManager.AppSettings[prop];
            return string.IsNullOrWhiteSpace(value) ? defProp : value;
        }

        public static App[] GetApps(string appKey, string argsKey, string watchKey, string limitKey)
        {
            Dictionary<string, string> paths = GetArrayStartsWith(appKey);
            Dictionary<string, string> allArgs = GetArrayStartsWith($"{appKey}.{argsKey}");
            Dictionary<string, string> watchFlags = GetArrayStartsWith($"{appKey}.{watchKey}");
            Dictionary<string, string> limits = GetArrayStartsWith($"{appKey}.{limitKey}");
            List<App> apps = new List<App>();
            int i = 0;

            foreach (KeyValuePair<string, string> p in paths)
            {
                // Get app id and path options values
                string id = p.Key == appKey ? IO.Path.GetFileNameWithoutExtension(p.Value) : p.Key // One app case - id is missing
                    , path = p.Value
                    , args = string.Empty
                    , watch = string.Empty
                    , limit = string.Empty;

                // Get app watch and limit options values
                if (string.IsNullOrWhiteSpace(id)) // No app ID provided
                {
                    if (watchFlags.Count < i)
                    {
                        watch = watchFlags.ElementAt(i).Value;
                    }
                    if (limits.Count < i)
                    {
                        limit = limits.ElementAt(i).Value;
                    }
                    if (allArgs.Count < i)
                    {
                        args = allArgs.ElementAt(i).Value;
                    }
                }
                else
                {
                    allArgs.TryGetValue(id, out args);
                    watchFlags.TryGetValue(id, out watch);
                    limits.TryGetValue(id, out limit);
                }

                // Ignore empty path apps
                if (string.IsNullOrWhiteSpace(path)) { continue; }

                // Try to get full path
                try
                {
                    path = IO.Path.GetFullPath(path);
                }
                catch (Exception e)
                {
                    LogError($"Error while parsing path in property '{appKey}' -> '{id}' with value '{p.Value}'. Path try: {path}; Error: {e.Message}");
                    continue;
                }

                // Check if app file exists
                if (string.IsNullOrWhiteSpace(path) || !IO.File.Exists(path))
                {
                    LogError($"Error while parsing path in property '{appKey}' -> '{id}' with value '{p.Value}'. Path try: {path};  Error: file not found.");
                    continue;
                }

                //// One app case - id is missing in config
                //if (id == appKey)
                //{
                //    // Use file name as id
                //    id = IO.Path.GetFileNameWithoutExtension(path);
                //}

                // Create app descriptor
                App app = new App(id, path, args, watch, limit);
                // Push app to array
                apps.Add(app);
            }

            return apps.ToArray();
        }

        public static Dictionary<string, string> GetArrayStartsWith(string prop)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            int propLen = prop.Length;
            // Get target property keys and values
            ConfigurationManager.AppSettings.AllKeys
                .Where(key => key.StartsWith(prop))
                // Extract id from key and convert list to dictionary
                .Each(
                key =>
                {
                    // Key contains additional ID?
                    if (key.Length > propLen)
                    {
                        // Key contains delimeter after prop end?
                        if (idDelimiters.Contains(key[propLen]))
                        {
                            string k = key.Remove(0, propLen + 1);
                            result[k] = ConfigurationManager.AppSettings[key];
                        }
                    }
                    else // No Id added
                    {
                        result[string.Empty] = ConfigurationManager.AppSettings[key];
                    }
                });

            return result;
        }

        public static string GetPath(string prop, string defProp = null)
        {
            string value = ConfigurationManager.AppSettings[prop];

            if (string.IsNullOrWhiteSpace(value))
            { return defProp; }
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
            bool.TryParse(Get("LogEnabled", "false"), out IsLogEnabled);
            LogPath = GetPath("LogPath", defaultLogPath);
            Apps = GetApps("App", "Arguments", "Restart", "RestartLimit");
            Description = Get("Description", "Logon Service for running applications on logon screen");
            DisplayName = Get("DisplayName", "Logon Service");
            ServiceName = Get("ServiceName", "LogonService");
        }

        public static void Log(bool isConsole, string str)
        {
            if (isConsole)
            {
                Console.WriteLine(str);
            }
            if (IsLogEnabled)
            {
                Log(str);
            }
        }

        #endregion Public Methods

        #region Private Methods

        public static void Log(string str)
        {
            IO.File.AppendAllText(LogPath, $"[{DateTime.Now}] {str}\r\n");
        }

        private static void LogError(string str)
        {
            Console.WriteLine(str);
            Log(str);
        }

        #endregion Private Methods

        #region Public Classes

        /// <summary>
        /// Application config and runtime descriptor
        /// </summary>
        public class App
        {
            #region Public Constructors

            public App(string id, string path, string args, string watch, string limit)
            {
                Path = path;
                File = IO.Path.GetFileName(path);
                // Id or short file name
                Id = string.IsNullOrWhiteSpace(id)
                    ? IO.Path.GetFileNameWithoutExtension(File)
                    : id;
                bool.TryParse(watch, out IsRestart);
                uint.TryParse(limit, out RestartLimit);

                Command = string.IsNullOrWhiteSpace(args) ? Path : $"\"{Path}\" {args}";
            }

            #endregion Public Constructors

            #region Public Fields

            /// <summary>
            /// Command to execute
            /// </summary>
            public readonly string Command = string.Empty;

            /// <summary>
            /// File name with extension
            /// </summary>
            public readonly string File = string.Empty;

            /// <summary>
            /// Application ID or process name without extension
            /// </summary>
            public readonly string Id = string.Empty;

            /// <summary>
            /// Watch application stop and restart or not
            /// </summary>
            public readonly bool IsRestart = false;

            /// <summary>
            /// Application full path
            /// </summary>
            public readonly string Path = string.Empty;

            /// <summary>
            /// Limit for application restarts
            /// </summary>
            public readonly uint RestartLimit = 0;

            /// <summary>
            /// Runtime process info
            /// </summary>
            public ApplicationLoader.PROCESS_INFORMATION ProcInfo;

            /// <summary>
            /// Restarts counter
            /// </summary>
            public uint RestartCounter = 0;

            #endregion Public Fields

            #region Public Methods

            public override string ToString() => Id;

            #endregion Public Methods
        }

        #endregion Public Classes
    }
}