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

        public RelayCommand NavigateToConfiguracionCommand { get; set; }
        public RelayCommand NavigateToInicioCommand { get; set; }
        public RelayCommand NavigateToUsuarioCommand { get; set; }
        public RelayCommand NavigateToReporteCommand { get; set; }
        public RelayCommand NavigateToInvitadosCommand { get; set; }
        public MainViewModel(INavigationService navService)
        {
            Navigation = navService;
            NavigateToInicioCommand = new RelayCommand(_ => Navigation.NavigateTo<InicioViewModel>(), _ => true);
            NavigateToConfiguracionCommand = new RelayCommand(_ => Navigation.NavigateTo<ConfiguracionViewModel>(), _ => true);
            NavigateToUsuarioCommand = new RelayCommand(_ => Navigation.NavigateTo<UsuarioViewModel>(), _ => true);
            NavigateToReporteCommand = new RelayCommand(_ => Navigation.NavigateTo<ReporteViewModel>(), _ => true);
            NavigateToInvitadosCommand = new RelayCommand(_ => Navigation.NavigateTo<InvitadosViewModel>(), _ => true);

            // Do not navigate by default; we will navigate after startup animation completes.
        }
    }
}
