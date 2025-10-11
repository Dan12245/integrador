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
    /// Lógica de interacción para Preguntas.xaml
    /// </summary>
    public partial class Preguntas : UserControl
    {
        public Preguntas()
        {
            InitializeComponent();
        }

        public void CambiarEscena(UserControl nuevoControl)
        {
            Login_Window.Children.Clear();
            Login_Window.Children.Add(nuevoControl);
        }

        private void Boton_Register_Click(object sender, RoutedEventArgs e)
        {
            CambiarEscena(new Inicio());
        }
        }
    }
