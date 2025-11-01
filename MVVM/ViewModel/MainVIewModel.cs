using Consumo_Reducido_de_Agua_ahora_si_definitivo.Core;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.Services;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.ViewModel
{
    internal class MainViewModel : Core.ViewModel
    {
        public INavigationService _navigation;
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
        public RelayCommand NavigateHommeCommand { get; set; }
        public MainViewModel(INavigationService navService)
        {
            Navigation = navService;
            NavigateHommeCommand = new RelayCommand(_ => Navigation.NavigateTo<InicioViewModel>(), _ => true);

            NavigateToConfiguracionCommand = new RelayCommand(_ => Navigation.NavigateTo<ConfiguracionViewModel>(), _ => true);
        }
    }
}
