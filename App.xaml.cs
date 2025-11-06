using Consumo_Reducido_de_Agua_ahora_si_definitivo.Core;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.ViewModel;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.Services;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.View;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Windows.Devices.I2c.Provider;
// Alias to distinguish between Shell and Charts MainViewModel
using ShellMainViewModel = Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.ViewModel.MainViewModel;
using ChartsMainViewModel = Consumo_Reducido_de_Agua_ahora_si_definitivo.ViewModels.MainViewModel;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            // Register main VMs
            services.AddSingleton<ShellMainViewModel>();
            services.AddSingleton<ChartsMainViewModel>();

            // Register navigable VMs
            services.AddSingleton<InicioViewModel>();
            services.AddSingleton<ConfiguracionViewModel>();
            services.AddSingleton<UsuarioViewModel>();
            services.AddSingleton<ReporteViewModel>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<RegistroViewModel>();
            services.AddSingleton<InvitadosViewModel>();

            services.AddSingleton<INavigationService, NavigationService>();

            // Register factory for ViewModels
            services.AddSingleton<Func<Type, ViewModel>>(serviceProvider =>
                viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));

            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                // DataContext on shell VM
                DataContext = provider.GetRequiredService<ShellMainViewModel>()
            });

            // Views can be resolved too if needed
            services.AddSingleton<Configuracion>();
            services.AddSingleton<Inicio>();
            services.AddSingleton<Invitados>();
            services.AddSingleton<Login>();
            services.AddSingleton<Registro>();
            services.AddSingleton<Reporte>();
            services.AddSingleton<Usuario>();

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