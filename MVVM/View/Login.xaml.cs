using Consumo_Reducido_de_Agua_ahora_si_definitivo.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.ViewModel;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Registro;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        private bool mostrandoContraseña = false; // Estado actual
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
        public static string UsuarioActualEmail { get; private set; }

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

            conexion con = new conexion();
            if (con.iniciar_sesion(email, password))
            {
                // Navegar a Inicio tras validar credenciales
                if (Application.Current.MainWindow?.DataContext is MainViewModel shell)
                {
                    shell.NavigateToInicioCommand.Execute(null);
                }
            }
            else
            {
                MessageBox.Show("Correo o contraseña incorrectos.", "Acceso denegado", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
