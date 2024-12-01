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

        public static IEnumerable<T> Each<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (T item in list) { action(item); }
            return list;
        }

        public static IEnumerable<KeyValuePair<K, V>> Each<K, V>(
            this IEnumerable<KeyValuePair<K, V>> dict,
            Action<K, V> action
        )
        {
            foreach (KeyValuePair<K, V> kv in dict) { action(kv.Key, kv.Value); }
            return dict;
        }

        public static IDictionary<K, V> RemoveAll<K, V>(
            this IDictionary<K, V> dict,
            IEnumerable<K> keys
        )
        {
            foreach (K key in keys) { dict.Remove(key); }
            return dict;
        }

        public static void Deconstruct<T>(this T[] items, out T t0, out T t1)
        {
            t0 = items.Length > 0 ? items[0] : default;
            t1 = items.Length > 1 ? items[1] : default;
        }
    }
}
