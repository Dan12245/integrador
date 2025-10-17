using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail; 
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json.Serialization;

using IOPath = System.IO.Path;
using System.Xml.Linq;

namespace C.R.A_Consumo_reducido_de_agua
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static class UserData
        {
            private static string folder = IOPath.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "C.R.A"
            );

            private static string filePath = IOPath.Combine(folder, "UserData.json");

            // Datos básicos
            public static string UserName { get; set; } = "";
            public static string ProfileImagePath { get; set; } = "";

            // Configuraciones
            public static string Nota { get; private set; } =
    "ATENCION! en caso de modificarse este archivo, su cuenta sera automaticamente baneada de la aplicacion, proceda con precaucion";

            public static bool Grafica { get; set; } = false;
            public static bool Frecuencia { get; set; } = false;
            public static bool Uso { get; set; } = false;
            public static string Rendimiento { get; set; } = "";
            public static bool Notificaciones { get; set; } = true;
            public static bool Invitados { get; set; } = false;
            public static string Idioma { get; set; } = "";
            public static int Tamaño { get; set; } = 0;

            public static int Meta { get; set; } = 0;

            public static int Domicilios { get; set; } = 0;
            public static string Domicilio { get; set; } = "";
            public static bool Propio { get; set; } = false;

            public static void Save()
            {
                Directory.CreateDirectory(folder);

                var json = JsonSerializer.Serialize(new
                {
                    //si agregas una nueva variable, asegurate de agregarla aqui tambien
                    Nota,
                    UserName,
                    ProfileImagePath,
                    Domicilios,
                    Grafica,
                    Meta,
                    Frecuencia,
                    Uso,
                    Rendimiento,
                    Notificaciones,
                    Invitados,
                    Idioma,
                    Tamaño,
                    Domicilio,
                    Propio
                }, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(filePath, json);
            }

            public static void Load()
            {
                if (!File.Exists(filePath))
                    return; // Mantener valores por defecto si no existe archivo

                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<JsonElement>(json);

                UserName = data.GetProperty("UserName").GetString() ?? "";
                ProfileImagePath = data.GetProperty("ProfileImagePath").GetString() ?? "";

                Meta = data.GetProperty("Meta").GetInt32();

                Grafica = data.GetProperty("Grafica").GetBoolean();
                Frecuencia = data.GetProperty("Frecuencia").GetBoolean();
                Uso = data.GetProperty("Uso").GetBoolean();
                Rendimiento = data.GetProperty("Rendimiento").GetString() ?? "";
                Notificaciones = data.GetProperty("Notificaciones").GetBoolean();
                Invitados = data.GetProperty("Invitados").GetBoolean();
                Idioma = data.GetProperty("Idioma").GetString() ?? "";
                Tamaño = data.GetProperty("Tamaño").GetInt32();
                Domicilio = data.GetProperty("Domicilio").GetString() ?? "";
                Propio = data.GetProperty("Propio").GetBoolean();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            UserData.Save();
        }

        public bool escorreovalido(String email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public void CambiarEscena(UserControl nuevoControl)
        {
            Login_Window.Children.Clear();
            Login_Window.Children.Add(nuevoControl);
        }

        // Este evento se ejecuta cuando termina la animación
        private void LogoAnimacion_Completed(object sender, EventArgs e)
        {
            // Reemplaza "Inicio" con el UserControl que quieras mostrar después
            CambiarEscena(new Login());
        }
    }
}
