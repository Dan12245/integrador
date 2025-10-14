using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace C.R.A_Consumo_reducido_de_agua
{
    /// <summary>
    /// Lógica de interacción para Usuario.xaml
    /// </summary>
    public partial class Usuario : UserControl
    {
        public Usuario()
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

        private void Boton_Reporte(object sender, RoutedEventArgs e)
        {
            CambiarEscena(new Usuario());
        }

        private void Boton_ir_a_Configuracion(object sender, RoutedEventArgs e)
        {
            CambiarEscena(new Usuario());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
