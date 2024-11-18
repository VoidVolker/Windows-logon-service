using System;
using System.Collections.Generic;

namespace LogonService.Watchers
{
    /// <summary>
    /// Base logic for process watching 
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public abstract class ProcWatcher<TEvent>
    {
        /// <summary>
        /// Process start event watchers list
        /// </summary>
        public List<NameWatcher> StartWatchers { get; } = new List<NameWatcher>();
        /// <summary>
        /// Process stop event watchers list
        /// </summary>
        public List<NameWatcher> StopWatchers { get; } = new List<NameWatcher>();
        /// <summary>
        /// Process stop event watchers list
        /// </summary>
        public List<IdWatcher> IdStopWatchers { get; } = new List<IdWatcher>();

        /// <summary>
        /// Start watcher, virtual
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// Stop watcher, virtual
        /// </summary>
        public virtual void Stop() { }

        /// <summary>
        /// Add callback to process started event
        /// </summary>
        /// <param name="name">Process full name with extension</param>
        /// <param name="action">Callback to execute</param>
        /// <param name="once">Run callback once, false by default</param>
        public void OnStart(string name, Action<uint, TEvent> action, bool once = false)
        {
            StartWatchers.Add(new NameWatcher(name, action, once));
        }

        /// <summary>
        /// Add callback to process stopped event
        /// </summary>
        /// <param name="name">Process full name with extension</param>
        /// <param name="action">Callback to execute</param>
        /// <param name="once">Run callback once, false by default</param>
        public void OnStop(string name, Action<uint, TEvent> action, bool once = false)
        {
            StopWatchers.Add(new NameWatcher(name, action, once));
        }

        /// <summary>
        /// Add callback to process stopped event with exactly ID
        /// </summary>
        /// <param name="id">Process ID</param>
        /// <param name="action">Callback, runs once</param>
        public void OnIdStop(uint id, Action<TEvent> action)
        {
            IdStopWatchers.Add(new IdWatcher(id, action));
        }

        /// <summary>
        /// Trigger process started event
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <param name="name">Process full name with extension</param>
        /// <param name="ev">Event object</param>
        public void StarTrigger(uint pid, string name, TEvent ev)
        {
            Trigger(StartWatchers, pid, name, ev);
        }

        /// <summary>
        /// Trigger process stopped event
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <param name="name">Process full name with extension</param>
        /// <param name="ev">Event object</param>
        public void StopTrigger(uint pid, string name, TEvent ev)
        {
            Trigger(StopWatchers, pid, name, ev);
            // Find triggered ID watchers
            List<IdWatcher> idWatchers = IdStopWatchers.FindAll(w => w.Options.Id == pid);
            // Remove watcher from list first
            idWatchers.ForEach(w => IdStopWatchers.Remove(w));
            // Run callbacks in triggered watchers
            idWatchers.ForEach(w => w.Action(ev));
        }

        /// <summary>
        /// Trigger event for selected watchers list
        /// </summary>
        /// <param name="watchers">Watchers list</param>
        /// <param name="pid">Process ID</param>
        /// <param name="name">Process full name with extension</param>
        /// <param name="ev">Event object</param>
        private void Trigger(
            List<NameWatcher> watchers,
            uint pid,
            string name,
            TEvent ev
            )
        {
            // Find triggered watchers
            List<NameWatcher> triggeredWatchers = watchers
                .FindAll(w => w.Options.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            // Remove once callback call watchers
            triggeredWatchers.FindAll(w => w.Options.Once).ForEach(w => watchers.Remove(w));
            // Run callbacks in triggered watchers
            triggeredWatchers.ForEach(w => w.Action(pid, ev));
        }

        /// <summary>
        /// Watch descriptor
        /// </summary>
        /// <typeparam name="TOpt">Watcher options</typeparam>
        /// <typeparam name="TAction">Watcher action</typeparam>
        public class WatchDescriptor<TOpt, TAction>
        {
            public TOpt Options;
            public TAction Action;

            public WatchDescriptor(TOpt options, TAction action)
            {
                Options = options;
                Action = action;
            }
        }

        /// <summary>
        /// Name watcher options
        /// </summary>
        public readonly struct NameWatchOption
        {
            /// <summary>
            /// Process name with extension
            /// </summary>
            public readonly string Name;
            /// <summary>
            /// Run callback once and remove watcher from watchers list
            /// </summary>
            public readonly bool Once;

            public NameWatchOption(string name, bool once = false)
            {
                Name = name;
                Once = once;
            }
        }

        /// <summary>
        /// Process ID watcher options
        /// </summary>
        public readonly struct IdWatchOption
        {
            /// <summary>
            /// Process ID
            /// </summary>
            public readonly uint Id;

            public IdWatchOption(uint id)
            {
                Id = id;
            }
        }

        /// <summary>
        /// Process name watcher
        /// </summary>
        public class NameWatcher : WatchDescriptor<NameWatchOption, Action<uint, TEvent>>
        {
            public NameWatcher(string name, Action<uint, TEvent> action, bool once = false)
                : base(new NameWatchOption(name, once), action)
            {
            }
        }

        /// <summary>
        /// Process ID watcher
        /// </summary>
        public class IdWatcher : WatchDescriptor<IdWatchOption, Action<TEvent>>
        {
            public IdWatcher(uint id, Action<TEvent> action)
                : base(new IdWatchOption(id), action)
            {
            }
        }
    }
}
