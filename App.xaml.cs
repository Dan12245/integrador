using Consumo_Reducido_de_Agua_ahora_si_definitivo.Core;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.ViewModel;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.Services;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.View;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Windows.Devices.I2c.Provider;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<Configuracion>();
            services.AddSingleton<Inicio>();
            services.AddSingleton<Invitados>();
            services.AddSingleton<Login>();
            services.AddSingleton<Registro>();
            services.AddSingleton<Reporte>();
            services.AddSingleton<Usuario>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}
