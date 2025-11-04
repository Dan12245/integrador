using Consumo_Reducido_de_Agua_ahora_si_definitivo.Core;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.ViewModel
{
    class LoginViewModel : Core.ViewModel
    {
        private readonly INavigationService _navigationService;

        // Comando para navegar a Invitados (usado por Boton_Register en Usuario.xaml)
        public RelayCommand NavigateToInicioCommand { get; }
        public RelayCommand NavigateToRegistroCommand { get; }

        public LoginViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateToInicioCommand = new RelayCommand(_ => _navigationService.NavigateTo<InicioViewModel>(), _ => true);
            NavigateToRegistroCommand = new RelayCommand(_ => _navigationService.NavigateTo<RegistroViewModel>(), _ => true);
        }
    }
}
