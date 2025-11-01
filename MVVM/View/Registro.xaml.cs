using Consumo_Reducido_de_Agua_ahora_si_definitivo.View;
using System.Windows;
using System.Windows.Controls;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    public partial class Registro : UserControl
    {
        public static class GlobalData
        {
            public static string UserName { get; set; }
        }
        public Registro()
        {
            InitializeComponent();

            Boton_Login.MouseEnter += (s, e) =>
            {
                Boton_Login.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(57, 61, 191));
                Boton_Login.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
            };

            Boton_Login.MouseLeave += (s, e) =>
            {
                Boton_Login.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(150, 4, 2, 62));
                Boton_Login.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 255, 255, 255));
            };
        }

        private void Boton_Login_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow main)
            {
                main.CambiarEscena(new Login());
            }
        }
        private void PlaceHolderTexto(object sender, TextChangedEventArgs e)
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
        private void Boton_Registrarse(object sender, RoutedEventArgs e)
        {
            string name = Campo_Nombre.Text;
            string email = Campo_Email.Text;
            string contraseña = Campo_Contraseña.Password;
            string repetir_contraseña = Campo_Repetir_Contraseña.Password;

            if (Application.Current.MainWindow is MainWindow main)
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(contraseña) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(repetir_contraseña))
                {
                    MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    if (repetir_contraseña != contraseña)
                    {
                        MessageBox.Show("Las contraseñas no coinciden.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else if (contraseña.Length < 3)
                    {
                        MessageBox.Show("La contraseña debe tener al menos 6 caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else if (!main.escorreovalido(email))
                    {
                        MessageBox.Show("Por favor, ingrese un correo electrónico válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        var con = new conexion();
                        con.registrar_usuario(name, email, contraseña);
                        main.CambiarEscena(new View.Inicio());
                        GlobalData.UserName = name;
                    }
                }
            }
        }
    }
}
