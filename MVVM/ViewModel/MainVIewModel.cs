using System;
using System.Collections.Generic;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.Core;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.Services;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.ViewModel
{
    public class MainViewModel : Core.ViewModel
    {
        private INavigationService _navigation;
        public INavigationService Navigation
        {
            get => _navigation;
            set
            {
                _navigation = value;
                OnPropertyChanged();
            }
        }
        
        // Single parameterized navigation command (Type or string alias)
        public RelayCommand NavigateToCommand { get; }

        // Keep existing commands for now to avoid touching existing XAML
        public RelayCommand NavigateToConfiguracionCommand { get; set; }
        public RelayCommand NavigateToInicioCommand { get; set; }
        public RelayCommand NavigateToUsuarioCommand { get; set; }
        public RelayCommand NavigateToReporteCommand { get; set; }
        public RelayCommand NavigateToInvitadosCommand { get; set; }
        public RelayCommand NavigateToLoginCommand { get; set; }
        public RelayCommand NavigateToRegistroCommand { get; set; }

        private readonly Dictionary<string, Type> _routes = new()
        {
            ["Inicio"] = typeof(InicioViewModel),
            ["Configuracion"] = typeof(ConfiguracionViewModel),
            ["Usuario"] = typeof(UsuarioViewModel),
            ["Reporte"] = typeof(ReporteViewModel),
            ["Invitados"] = typeof(InvitadosViewModel),
            ["Login"] = typeof(LoginViewModel),
            ["Registro"] = typeof(RegistroViewModel),
        };

        public MainViewModel(INavigationService navService)
        {
            Navigation = navService;

            NavigateToCommand = new RelayCommand(p =>
            {
                if (p is Type t && typeof(Core.ViewModel).IsAssignableFrom(t))
                {
                    Navigation.NavigateTo(t);
                }
                else if (p is string key && _routes.TryGetValue(key, out var vmType))
                {
                    Navigation.NavigateTo(vmType);
                }
            }, _ => true);

            // Existing commands (optional to keep)
            NavigateToInicioCommand = new RelayCommand(_ => Navigation.NavigateTo<InicioViewModel>(), _ => true);
            NavigateToConfiguracionCommand = new RelayCommand(_ => Navigation.NavigateTo<ConfiguracionViewModel>(), _ => true);
            NavigateToUsuarioCommand = new RelayCommand(_ => Navigation.NavigateTo<UsuarioViewModel>(), _ => true);
            NavigateToReporteCommand = new RelayCommand(_ => Navigation.NavigateTo<ReporteViewModel>(), _ => true);
            NavigateToInvitadosCommand = new RelayCommand(_ => Navigation.NavigateTo<InvitadosViewModel>(), _ => true);
            NavigateToLoginCommand = new RelayCommand(_ => Navigation.NavigateTo<LoginViewModel>(), _ => true);
            NavigateToRegistroCommand = new RelayCommand(_ => Navigation.NavigateTo<RegistroViewModel>(), _ => true);

            // Do not navigate by default; we will navigate after startup animation completes.
        }
    }
}
