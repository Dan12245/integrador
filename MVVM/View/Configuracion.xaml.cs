using Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.ViewModel;
using System.Windows;
using System.Windows.Controls;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Registro;


namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para Configuracion.xaml
    /// </summary>
    public partial class Configuracion : UserControl
    {
        public Configuracion()
        {
            InitializeComponent();
        }
        private void Boton_Eliminar_Usuario(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            conexion con = new conexion();
            string user_email = GlobalData.email;
            con.Eliminar_usuario(user_email);

            if (Application.Current.MainWindow?.DataContext is not MainViewModel shell)
                return;
            shell.NavigateToCommand.Execute(typeof(LoginViewModel));
        }
    }
}
