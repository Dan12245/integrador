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

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo
{
    /// <summary>
    /// Lógica de interacción para pruebas.xaml
    /// </summary>
    public partial class pruebas : UserControl
    {
        public pruebas()
        {
            InitializeComponent();
        }

        private const double BaseWidth = 800;
        private const double BaseHeight = 450;

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double scaleX = ActualWidth / BaseWidth;
            double scaleY = ActualHeight / BaseHeight;

            // Calculamos el factor de escala más pequeño para mantener proporción
            double uniformScale = Math.Min(scaleX, scaleY);

            // Si la ventana es más pequeña que el tamaño base → escala normal
            if (ActualWidth <= BaseWidth && ActualHeight <= BaseHeight)
            {
                RootScale.ScaleX = uniformScale;
                RootScale.ScaleY = uniformScale;
                OverlayBorde.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Cuando la ventana es más grande → no escalar más, mostrar bordes
                RootScale.ScaleX = 1;
                RootScale.ScaleY = 1;
                OverlayBorde.Visibility = Visibility.Visible;
            }
        }
    }
}
