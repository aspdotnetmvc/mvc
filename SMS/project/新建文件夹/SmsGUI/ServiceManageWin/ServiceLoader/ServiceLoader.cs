using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ServiceLoader
{
    /// <summary>
    /// 服务加载器
    /// </summary>
    public class ServiceLoader
    {
        private AppDomain AppDomain { get; set; }
        private IService.ServiceProp serviceProp;
        private RemoteLoader remoteLoader;

        public ServiceLoader(IService.Host host)
        {
            this.serviceProp = host.serviceProp;
            AppDomainSetup setup = new AppDomainSetup();
            setup.LoaderOptimization = LoaderOptimization.SingleDomain;
            setup.ApplicationName = serviceProp.Code;
            setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            setup.PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, serviceProp.Directory);
        
            setup.CachePath = setup.ApplicationBase; //Path.Combine(setup.ApplicationBase, "temp");
            setup.ShadowCopyFiles = "true";
            setup.ShadowCopyDirectories = setup.ApplicationBase;
          
            AppDomain.CurrentDomain.SetShadowCopyFiles();

            this.AppDomain = AppDomain.CreateDomain(serviceProp.Code, null, setup);
            this.AppDomain.SetData("APP_CONFIG_FILE", serviceProp.DLLFullPath + ".config");
              
            String name = Assembly.GetExecutingAssembly().GetName().FullName;
            this.remoteLoader = (RemoteLoader)this.AppDomain.CreateInstanceAndUnwrap(name, typeof(RemoteLoader).FullName);
        }
        public void LoadAssembly(IService.ServiceProp serviceProp)
        {
            remoteLoader.LoadAssembly(serviceProp.DLLFullPath);
        }

        public object GetInstance(string typeName)
        {
            if (remoteLoader == null) return null;
            return remoteLoader.GetInstance(typeName);
        }

        public void ExecuteMothod(string typeName, string methodName)
        {
            remoteLoader.ExecuteMothod(typeName, methodName);
        }

        public void Unload()
        {
            try
            {
                if (AppDomain == null) return;
                AppDomain.Unload(this.AppDomain);
                this.AppDomain = null;
            }
            catch (CannotUnloadAppDomainException ex)
            {
                // throw ex;
            }
        }

    }
}
