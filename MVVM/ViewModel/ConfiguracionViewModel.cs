namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.ViewModel
{
    using Consumo_Reducido_de_Agua_ahora_si_definitivo.Core;
    using Consumo_Reducido_de_Agua_ahora_si_definitivo.Services;
    using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Registro; // For GlobalData

    internal class ConfiguracionViewModel : Core.ViewModel
    {
        private readonly INavigationService _navigationService;

        public RelayCommand NavigateToLoginCommand { get; }

        public ConfiguracionViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateToLoginCommand = new RelayCommand(_ =>
            {
                // Clear basic session data
                GlobalData.UserName = string.Empty;
                GlobalData.email = string.Empty;
                GlobalData.userid = 0;

                // Navigate back to Login screen
                _navigationService.NavigateTo<LoginViewModel>();
            }, _ => true);
        }
    }
}
