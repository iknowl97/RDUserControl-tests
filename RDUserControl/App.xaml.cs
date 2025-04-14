using Microsoft.Extensions.DependencyInjection;
using RDUserControl.Services;
using RDUserControl.ViewModels;
using RDUserControl.Views;
using System;
using System.Windows;

namespace RDUserControl
{
    public partial class App : Application
    {
        private readonly ServiceProvider? _serviceProvider;

        public App()
        {
            Services = ConfigureServices();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public static new App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services
            services.AddSingleton<IUserManagementService, UserManagementService>();
            services.AddSingleton<IRdpService, RdpService>();
            services.AddSingleton<IEmailService, EmailService>();

            // ViewModels
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<UserManagementViewModel>();
            services.AddSingleton<RdpManagementViewModel>();
            services.AddSingleton<SettingsViewModel>();

            // Views
            services.AddSingleton<MainWindow>();
            services.AddSingleton<DashboardPage>();
            services.AddSingleton<UserManagementPage>();
            services.AddSingleton<RdpManagementPage>();
            services.AddSingleton<SettingsPage>();

            return services.BuildServiceProvider();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = Services.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}