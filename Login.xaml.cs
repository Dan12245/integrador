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
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
            // MouseEnter → cambio de color

            Boton_Register.MouseEnter += (s, e) =>
            {

                Boton_Register.Background = new SolidColorBrush(Color.FromRgb(57, 61, 191)); // Azul brillante
                Boton_Register.Foreground = new SolidColorBrush(Colors.White);

            };

            // MouseLeave → volver a color original
            Boton_Register.MouseLeave += (s, e) =>
            {
                Boton_Register.Background = new SolidColorBrush(Color.FromArgb(150, 4, 2, 62)); // Fondo original
                Boton_Register.Foreground = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)); // Texto original
            };
            Iniciar_Sesion.MouseEnter += (s, e) =>
            {
                Iniciar_Sesion.Height = 34;
                Iniciar_Sesion.Width = 297;
            };

            Iniciar_Sesion.MouseLeave += (s, e) =>
            {
                Iniciar_Sesion.Height = 32;
                Iniciar_Sesion.Width = 287;
            };
        }


        // --- PLACEHOLDERS ---
        private void txtEmail_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string email = txtEmail.Text;
            if (string.IsNullOrWhiteSpace(email))
            {
                txtPlaceholderEmail.Text = "Email";
            }
            else
            {
                txtPlaceholderEmail.Text = "";
            }
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = txtPassword.Password;
            if (string.IsNullOrWhiteSpace(password))
            {
                txtPlaceholderPass.Text = "Contraseña";
            }
            else
            {
                txtPlaceholderPass.Text = "";
            }
        }

        // --- LOGIN ---
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Password;

            // Credenciales correctas
            string emailCorrecto = "BrunoTilin69@hotmail.com";
            string passwordCorrecta = "perromojado";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Application.Current.MainWindow is MainWindow main)
            {
                if (email == emailCorrecto && password == passwordCorrecta)
                {
                    main.CambiarEscena(new Inicio());
                }
                else
                {
                    MessageBox.Show("Correo o contraseña incorrectos.", "Acceso denegado", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void Boton_Register_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow main)
            {
                // Cambiar a la escena Registro
                main.CambiarEscena(new Registro());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Boton_Login_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow main)
            {
                // Cambiar a la escena Registro (este mismo control)
                main.CambiarEscena(new Inicio());
            }
        }
    }
}
