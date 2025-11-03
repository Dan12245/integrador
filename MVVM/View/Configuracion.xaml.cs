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
        public void CambiarEscena(UserControl nuevoControl)
        {
            Login_Window.Children.Clear();
            Login_Window.Children.Add(nuevoControl);
        }
        private void Boton_Usuario_Invitados(object sender, RoutedEventArgs e)
        {
            CambiarEscena(new Usuario());
        }

        private void Boton_Cerrar_Sesion(object sender, RoutedEventArgs e)
        {
            CambiarEscena(new Login());
        }

        private void Boton_Eliminar_Usuario(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            conexion con = new conexion();
            string user_email = GlobalData.email;
            con.Eliminar_usuario(user_email);
            CambiarEscena(new Login());
        }

        private void Boton_Reporte(object sender, RoutedEventArgs e)
        {
            CambiarEscena(new Reporte());
        }

        private void Boton_ir_a_Inicio(object sender, RoutedEventArgs e)
        {
            CambiarEscena(new Inicio());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Dominio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
