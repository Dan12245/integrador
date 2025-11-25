using Consumo_Reducido_de_Agua_ahora_si_definitivo.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.ViewModel;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para Registro.xaml
    /// </summary>
    public partial class Registro : UserControl
    {
        public static class GlobalData
        {
            public static string UserName { get; set; }
            public static string email { get; set; }
            public static int userid { get; set; }
        }
        public Registro()
        {
            InitializeComponent();

            Boton_Login.MouseEnter += (s, e) =>
            {
                Boton_Login.Background = new SolidColorBrush(Color.FromRgb(57, 61, 191)); // Azul brillante
                Boton_Login.Foreground = new SolidColorBrush(Colors.White);

            };

            // MouseLeave → volver a color original
            Boton_Login.MouseLeave += (s, e) =>
            {
                Boton_Login.Background = new SolidColorBrush(Color.FromArgb(150, 4, 2, 62)); // Fondo original
                Boton_Login.Foreground = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)); // Texto original
            };
        }

        private void Boton_Login_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow?.DataContext is MainViewModel shell)
            {
                shell.NavigateToLoginCommand.Execute(null);
            }
        }
        private void PlaceHolderTexto(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string name = Campo_Nombre.Text;
            string email = Campo_Email.Text;

            email_place_holder.Text = string.IsNullOrWhiteSpace(email) ? "Email" : string.Empty;
            nombre_place_holder.Text = string.IsNullOrWhiteSpace(name) ? "Nombre" : string.Empty;
        }

        private void PasswordPlaceHolder(object sender, RoutedEventArgs e)
        {
            string contraseña = Campo_Contraseña.Password;
            string repetir_contraseña = Campo_Repetir_Contraseña.Password;

            contraseña_place_holder.Text = string.IsNullOrWhiteSpace(contraseña) ? "Contraseña" : string.Empty;
            repetir_contraseña_place_holder.Text = string.IsNullOrWhiteSpace(repetir_contraseña) ? "Repetir Contraseña" : string.Empty;
        }
        private async void Boton_Registrarse(object sender, RoutedEventArgs e)
        {
            string name = Campo_Nombre.Text;
            string email = Campo_Email.Text;
            string contraseña = Campo_Contraseña.Password;
            string repetir_contraseña = Campo_Repetir_Contraseña.Password;

            if (Application.Current.MainWindow?.DataContext is not MainViewModel shell)
                return;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(contraseña) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(repetir_contraseña))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (repetir_contraseña != contraseña)
            {
                MessageBox.Show("Las contraseñas no coinciden.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (contraseña.Length < 6)
            {
                MessageBox.Show("La contraseña debe tener al menos 6 caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!(Application.Current.MainWindow is MainWindow main) || !main.escorreovalido(email))
            {
                MessageBox.Show("Por favor, ingrese un correo electrónico válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Registrar usuario
            conexion con = new conexion();
            con.registrar_usuario(name, email, contraseña);
            GlobalData.UserName = name;
            GlobalData.email = email;
            Login.userid =await con.id_usuario(email);

            // Navegar a Inicio tras registro exitoso
            shell.NavigateToInicioCommand.Execute(null);
        }
    }
}
