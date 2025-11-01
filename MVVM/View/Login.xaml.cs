using Consumo_Reducido_de_Agua_ahora_si_definitivo.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Registro;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    public partial class Login : UserControl
    {
        private bool mostrandoContraseña = false;
        public Login()
        {
            InitializeComponent();

            Boton_Register.MouseEnter += (s, e) =>
            {
                Boton_Register.Background = new SolidColorBrush(Color.FromRgb(57, 61, 191));
                Boton_Register.Foreground = new SolidColorBrush(Colors.White);
            };

            Boton_Register.MouseLeave += (s, e) =>
            {
                Boton_Register.Background = new SolidColorBrush(Color.FromArgb(150, 4, 2, 62));
                Boton_Register.Foreground = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
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
        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            string email = txtEmail.Text;
            txtPlaceholderEmail.Text = string.IsNullOrWhiteSpace(email) ? "Email" : string.Empty;
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = txtPassword.Password;
            txtPlaceholderPass.Text = string.IsNullOrWhiteSpace(password) ? "Contraseña" : string.Empty;
        }

        // --- LOGIN ---
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
          string email = txtEmail.Text;   
          string password = txtPassword.Password;

             if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
             {
                 MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                 return;
             }

            if (Application.Current.MainWindow is MainWindow main)
            {
                var con = new conexion();
                if (con.iniciar_sesion(email, password))
                {
                    var nombreUsuario = GlobalData.UserName;
                    mail.email = email;
                    main.CambiarEscena(new Inicio());
                }
                else
                {
                    MessageBox.Show("Correo o contraseña incorrectos.", "Acceso denegado", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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

        private void Icono_Contraseña_Click(object sender, RoutedEventArgs e)
        {
            // No hacer nada si no hay contraseña escrita
            if (string.IsNullOrEmpty(txtPassword.Password))
                return;

            if (!mostrandoContraseña)
            {
                // Mostrar texto
                txtPlaceholderPass.Text = txtPassword.Password;

                // Mostrar el TextBox y desactivar el PasswordBox
                txtPlaceholderPass.Opacity = 1;
                txtPlaceholderPass.IsHitTestVisible = false;

                txtPassword.Opacity = 0;
                txtPassword.IsHitTestVisible = true;

                mostrandoContraseña = true;
            }
            else
            {
                // Volver al modo oculto
                txtPassword.Password = txtPlaceholderPass.Text;

                txtPlaceholderPass.Opacity = 0;
                txtPlaceholderPass.IsHitTestVisible = false;

                txtPassword.Opacity = 1;
                txtPassword.IsHitTestVisible = true;

                mostrandoContraseña = false;
            }
        }
    }
}

