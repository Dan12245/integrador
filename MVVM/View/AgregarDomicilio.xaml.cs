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
using System.Windows.Shapes;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para AgregarDomicilio.xaml
    /// </summary>
    public partial class AgregarDomicilio : Window
    {
        public string Nombre { get; private set; }
        public string Descripcion { get; private set; }
        public AgregarDomicilio()
        {
            InitializeComponent();
            txtNombre.Focus();
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            // Validar nombre
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio", "Validación",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNombre.Focus();
                return;
            }

            // Guardar los valores
            Nombre = txtNombre.Text.Trim();
            Descripcion = Descripcion_Texto.Text.Trim();

            // Cerrar el diálogo con resultado positivo
            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            // Cerrar el diálogo con resultado negativo
            DialogResult = false;
            Close();
        }
    }
}
