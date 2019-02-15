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
            this.adsprServiceInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.adsprInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // adsprServiceInstaller
            // 
            this.adsprServiceInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.adsprServiceInstaller.Password = "adspr";
            this.adsprServiceInstaller.Username = "adspr";
            // 
            // adsprInstaller
            // 
            this.adsprInstaller.Description = "AD Self Password Reset Helper Logon Service";
            this.adsprInstaller.DisplayName = "ADSPR Logon Service";
            this.adsprInstaller.ServiceName = "logonService";
            this.adsprInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.adsprServiceInstaller,
            this.adsprInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller adsprServiceInstaller;
        private System.ServiceProcess.ServiceInstaller adsprInstaller;

    }
}