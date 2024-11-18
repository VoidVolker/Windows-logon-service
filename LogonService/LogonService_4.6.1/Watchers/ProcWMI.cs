using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

using LogonService.Watchers;

using static LogonService.ProcWMI;


namespace LogonService
{
    /// <summary>
    /// Prcess watcher using WMI interface, based on https://github.com/malcomvetter/WMIProcessWatcher
    /// </summary>
    public partial class ProcWMI : ProcWatcher<ProcEventArgs>
    {
        private ManagementEventWatcher StartWatcher;
        private ManagementEventWatcher StopWatcher;

        public new void Start()
        {
            StartWatcher = new ManagementEventWatcher(
                new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            StartWatcher.EventArrived += new EventArrivedEventHandler(OnProcessStarted);
            StartWatcher.Start();

            StopWatcher = new ManagementEventWatcher(
                new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            StopWatcher.EventArrived += new EventArrivedEventHandler(OnProcessStopped);
            StopWatcher.Start();
        }

        public new void Stop()
        {
            StartWatcher?.Stop();
            StopWatcher?.Stop();
            StartWatcher = null;
            StopWatcher = null;
        }

        private void OnProcessStarted(object sender, EventArrivedEventArgs e)
        {
            try
            {
                ManagementBaseObject ev = e.NewEvent;
                uint.TryParse(ev.Properties["ProcessID"].Value.ToString(), out uint id);
                string name = ev.Properties["ProcessName"].Value.ToString();

                StarTrigger(id, name, new ProcEventArgs(id, name, ev));
            }
            catch (Exception)
            {

            }
        }

        private void OnProcessStopped(object sender, EventArrivedEventArgs e)
        {
            try
            {
                ManagementBaseObject ev = e.NewEvent;
                uint.TryParse(ev.Properties["ProcessID"].Value.ToString(), out uint id);
                string name = ev.Properties["ProcessName"].Value.ToString();

                StopTrigger(id, name, new ProcEventArgs(id, name, ev));
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Process event arguments
        /// </summary>
        public class ProcEventArgs : EventArgs
        {
            public readonly uint Id;
            public readonly string Name;
            public readonly ManagementBaseObject Event;

            public ProcEventArgs(uint id, string name, ManagementBaseObject ev)
            {
                Id = id;
                Name = name;
                Event = ev;
            }

            public ProcInfo InfoGet()
            {
                ProcInfo info = new ProcInfo
                {
                    Id = Id,
                    Name = Name
                };

                try
                {
                    List<string> cmdLines = new List<string>();

                    using (ManagementObjectSearcher searcher =
                        new ManagementObjectSearcher($"SELECT * FROM Win32_Process WHERE ProcessId = {Id}"))
                    using (ManagementObjectCollection results = searcher.Get())
                    {
                        foreach (ManagementObject result in results.Cast<ManagementObject>())
                        {
                            try
                            {
                                cmdLines.Add(result["CommandLine"].ToString());
                            }
                            catch { }
                            try
                            {
                                ManagementBaseObject user = result.InvokeMethod("GetOwner", null, null);
                                info.UserDomain = user["Domain"].ToString();
                                info.UserName = user["User"].ToString();

                                info.User = string.IsNullOrEmpty(info.UserName)
                                    ? string.Empty
                                    : string.IsNullOrEmpty(info.UserDomain)
                                        ? info.UserName
                                        : $"{info.UserDomain}\\{info.UserName}";

                            }
                            catch { }
                        }
                    }

                    info.CommandLine = string.Join(" ", cmdLines);
                }
                catch (ManagementException) { }
                return info;
            }
        }

        public class ProcInfo
        {
            public string Name { get; internal set; }
            public uint Id { get; internal set; }
            public string CommandLine { get; internal set; }
            public string UserName { get; internal set; }
            public string UserDomain { get; internal set; }
            public string User { get; internal set; }
        }
    }
}
