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
using static C.R.A_Consumo_reducido_de_agua.MainWindow;
using static C.R.A_Consumo_reducido_de_agua.Registro;

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
            Texto_Nombre.Text = $"{GlobalData.UserName}";
            Barra_meta.Value = new Random().Next(10, 101);
        }

        public void CambiarEscena(UserControl nuevoControl)
        {
            Login_Window.Children.Clear();
            Login_Window.Children.Add(nuevoControl);
        }
        private void Boton_ir_a_Configuracion(object sender, RoutedEventArgs e)
        {
            CambiarEscena(new Configuracion());
        }

        private void Boton_ir_a_Invitados(object sender, RoutedEventArgs e)
        {
            CambiarEscena(new Invitados());
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

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
