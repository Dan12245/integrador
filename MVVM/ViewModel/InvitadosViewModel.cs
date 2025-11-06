using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.Core;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.Services;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.ViewModel
{
    public class InvitadosViewModel : Core.ViewModel
    {
        private readonly INavigationService _navigationService;

        // Comando para volver a Usuario (usado por Boton_Usuarios en Invitados.xaml)
        public RelayCommand NavigateToUsuariosCommand { get; }

        public InvitadosViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateToUsuariosCommand = new RelayCommand(_ => _navigationService.NavigateTo<UsuarioViewModel>(), _ => true);
        }
    }
}
