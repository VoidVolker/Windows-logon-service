using System.Configuration.Install;
using System.Reflection;

namespace LogonService
{
    class ServiceInstaller
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        private static readonly string exePath = Assembly.GetExecutingAssembly().Location;

        public static bool Install()
        {
            try { ManagedInstallerClass.InstallHelper(new[] { exePath }); }
            catch { return false; }
            return true;
        }

        public static bool Uninstall()
        {
            //if( EventLog.Exists(adsrpService.evLogSource) )
            //{
            //    EventLog.Delete(adsrpService.evLogSource);
            //}
            //if (EventLog.SourceExists(adsrpService.evLogSource))
            //{
            //    EventLog.DeleteEventSource(adsrpService.evLogSource);                
            //}            
            try { ManagedInstallerClass.InstallHelper(new[] { "/u", exePath }); }
            catch { return false; }
            return true;
        }
    }
}
