namespace LogonService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.ServiceInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.LogonServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // ServiceInstaller
            // 
            this.ServiceInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.ServiceInstaller.Password = "lsrv";
            this.ServiceInstaller.Username = "lsrv";
            // 
            // LogonServiceInstaller
            // 
            this.LogonServiceInstaller.Description = "Logon Service for running applications on logon screen";
            this.LogonServiceInstaller.DisplayName = "Logon Service";
            this.LogonServiceInstaller.ServiceName = "LogonService";
            this.LogonServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ServiceInstaller,
            this.LogonServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller ServiceInstaller;
        private System.ServiceProcess.ServiceInstaller LogonServiceInstaller;

    }
}
