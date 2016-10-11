namespace WatchDogService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.WatchDogServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.WatchDogServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // WatchDogServiceProcessInstaller
            // 
            this.WatchDogServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.WatchDogServiceProcessInstaller.Password = null;
            this.WatchDogServiceProcessInstaller.Username = null;
            // 
            // WatchDogServiceInstaller
            // 
            this.WatchDogServiceInstaller.ServiceName = "WatchDogService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.WatchDogServiceProcessInstaller,
            this.WatchDogServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller WatchDogServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller WatchDogServiceInstaller;
    }
}