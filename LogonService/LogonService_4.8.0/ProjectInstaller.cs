using System.ComponentModel;

namespace LogonService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            LogonServiceInstaller.Description = AppConfig.Description;
            LogonServiceInstaller.DisplayName = AppConfig.DisplayName;
            LogonServiceInstaller.ServiceName = AppConfig.ServiceName;
        }
    }
}
