using System;
using System.Configuration;
using System.Windows.Forms;
using Microsoft.WindowsAzure.Storage;
using StructureMap;
using Trek.ProductMonitor.Common;
using Trek.ProductMonitor.Services;

namespace Trek.ProductMonitor
{
    public static class Program
    {
        /// <summary>
        /// Retain reference to IoC Container
        /// </summary>
        public static Container Container;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {   
            Initialize();
            ConfigureServices();
            Start();
        }

        static void Initialize()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            LoadConfiguration();
        }

        static void LoadConfiguration()
        {
            //Ensure we have an azure connection
            if (ConfigurationManager.ConnectionStrings[AppConstants.AzureConnectionStringName] == null)
            {
                throw new ConfigurationErrorsException($"{AppConstants.AzureConnectionStringName} could not be found in the application configuration file.");
            }    
        }

        static void ConfigureServices()
        {
            Container = new Container(_ =>
            {
                _.For<CloudStorageAccount>().Use(() => CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings[AppConstants.AzureConnectionStringName].ConnectionString));
                _.For<IVendorProductCache>().Use<VendorProductMemoryCache>().Singleton();
                _.For<IProductUpdateService>().Use<ProductUpdateService>().Singleton();
            });
        }

        static void Start()
        {
            Application.Run(Container.GetInstance<Splash>());
        }
    }
}
