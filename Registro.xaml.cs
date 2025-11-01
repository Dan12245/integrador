using Consumo_Reducido_de_Agua_ahora_si_definitivo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
using System.Xml.Linq;

namespace C.R.A_Consumo_reducido_de_agua
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
            // MouseEnter → cambio de color



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
            if (Application.Current.MainWindow is MainWindow main)
            {
                // Cambiar a la escena Registro
                main.CambiarEscena(new Login());
            }
        }
        private void PlaceHolderTexto(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string name = Campo_Nombre.Text;
            string email = Campo_Email.Text;

            if (string.IsNullOrWhiteSpace(email))
            {
                email_place_holder.Text = "Email";
            }
            else
            {
                email_place_holder.Text = "";
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                nombre_place_holder.Text = "Nombre";
            }
            else
            {
                nombre_place_holder.Text = "";
            }
        }

        private void PasswordPlaceHolder(object sender, RoutedEventArgs e)
        {
            string contraseña = Campo_Contraseña.Password;
            string repetir_contraseña = Campo_Repetir_Contraseña.Password;

            if (string.IsNullOrWhiteSpace(contraseña))
            {
                contraseña_place_holder.Text = "Contraseña";
            }
            else
            {
                contraseña_place_holder.Text = "";
            }

            if (string.IsNullOrWhiteSpace(repetir_contraseña))
            {
                repetir_contraseña_place_holder.Text = "Repetir Contraseña";
            }
            else
            {
                repetir_contraseña_place_holder.Text = "";
            }
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
                        conexion con = new conexion();
                        con.registrar_usuario(name, email, contraseña);
                        // Cambiar a la escena Registro
                        main.CambiarEscena(new Preguntas());
                        GlobalData.UserName = name;
                    }

                }
            }
        }
    }
}