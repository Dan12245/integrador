using System;
using System.Windows;

namespace MiApp
{
    public partial class DialogoAgregarPersona : Window
    {
        public string Nombre { get; private set; }
        public string Descripcion { get; private set; }

        public DialogoAgregarPersona()
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