using Consumo_Reducido_de_Agua_ahora_si_definitivo.Properties;
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
using static C.R.A_Consumo_reducido_de_agua.MainWindow;

namespace C.R.A_Consumo_reducido_de_agua
{
    /// <summary>
    /// Lógica de interacción para Preguntas.xaml
    /// </summary>
    public partial class Preguntas : UserControl
    {
        string ruta = "UserData.json";
        public Preguntas()
        {
            InitializeComponent();
            MostrarPregunta();
            UserData.Load();
            Numero_Domicilios.Visibility = Visibility.Collapsed;
            Numero_Domicilios.Text = "";

        }

        public void CambiarEscena(UserControl nuevoControl)
        {
            Login_Window.Children.Clear();
            Login_Window.Children.Add(nuevoControl);
        }

        private void Boton_Register_Click(object sender, RoutedEventArgs e)
        {
            CambiarEscena(new Inicio());
        }
        private int indice = 0;

        // Estructura básica: texto de pregunta + tipo (1 = Sí/No, 2 = Continuar)
        private (string texto, int tipo)[] preguntas =
        {
             ("Seleccione el idioma", 0),
            ("¿La aplicacion se utilizara de manera Domestica o Empresarial?", 1),
            ("¿Su domicilio es propio o rentado?", 2),
             ("¿Su domicilio es propio o rentado?", 3),
             ("¿Cuenta con mas de un domicilio en renta?", 4),
            ("Ingrese el codigo de Invitado que se le dio", 5),
            ("Eliga con que frecuencia desea recibir las actualizaciones", 6),
            ("¿Con que tipo de grafica se siente mas comodo?", 7),
            ("¿Desea recibir notificaciones sobre consejos o tips para ahorrar agua?", 8),
            ("¿Cual es la gama de su pc? (Rendimiento)", 10),
             ("Con cuantos domicilios cuenta?", 9),
        };


        private void CambiarBoton(
            string imagenBoton1,
            RoutedEventHandler eventoBoton1,
            string imagenBoton2,
            RoutedEventHandler eventoBoton2)
        {
            // Crear los ImageBrush individuales
            ImageBrush brushEmpresarial = new ImageBrush();
            brushEmpresarial.ImageSource = new BitmapImage(
                new Uri($"pack://application:,,,/{imagenBoton2}", UriKind.Absolute)
            );
            brushEmpresarial.Stretch = Stretch.UniformToFill;

            ImageBrush brushDomestico = new ImageBrush();
            brushDomestico.ImageSource = new BitmapImage(
                new Uri($"pack://application:,,,/{imagenBoton1}", UriKind.Absolute)
            );
            brushDomestico.Stretch = Stretch.UniformToFill;

            // Aplicar texturas a cada botón
            Boton_Empresarial.Background = brushEmpresarial;
            Boton_Domestico.Background = brushDomestico;

            // Quitar todos los eventos anteriores para evitar duplicaciones
            LimpiarEventos(Boton_Empresarial);
            LimpiarEventos(Boton_Domestico);

            // Asignar los nuevos eventos
            Boton_Empresarial.Click += eventoBoton2;
            Boton_Domestico.Click += eventoBoton1;
        }

        private void LimpiarEventos(Button boton)
        {
            // Obtiene el campo interno donde WPF guarda los eventos Click
            var eventField = typeof(UIElement).GetField("EventHandlersStore",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var eventStore = eventField?.GetValue(boton);
            if (eventStore != null)
            {
                // Esto elimina todos los eventos del botón sin causar excepción
                var removeMethod = eventStore.GetType().GetMethod("RemoveRoutedEventHandlers",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                removeMethod?.Invoke(eventStore, new object[] { Button.ClickEvent });
            }
        }

        private void MostrarPregunta()
        {
            var pregunta = preguntas[indice];
            Texto_Pregunta.Text = pregunta.texto;

            // Mostrar u ocultar botones según el tipo

            if (pregunta.tipo == 1)
            {
                Boton_Empresarial.Visibility = Visibility.Visible;
                Boton_Domestico.Visibility = Visibility.Visible;
                Boton_Continuar.Visibility = Visibility.Collapsed;
                Listado.Visibility = Visibility.Collapsed;
                CambiarBoton("Casa.jpeg", Seleccion_Domestica, "Empresa.jpg", Seleccion_Empresarial);

            }

            else if (pregunta.tipo == 0)
            {
                Listado.Visibility = Visibility.Visible;
                Listado.Items.Add("Español");
                Listado.Items.Add("Ingles");
                Listado.Items.Add("Frances");
                Boton_Continuar.Visibility = Visibility.Visible;
                Boton_Empresarial.Visibility = Visibility.Collapsed;
                Boton_Domestico.Visibility = Visibility.Collapsed;


            }


            else if (pregunta.tipo == 2)
            {
                CambiarBoton ("Propio.png", Seleccion_Propio, "Renta.png",Seleccion_Rentado);
            }

            else if (pregunta.tipo == 3)
            {
                CambiarBoton("Propio.png", Seleccion_Propio, "Renta.png", Seleccion_Rentado);
            }

            else if (pregunta.tipo == 4)
            {
                Boton_Empresarial.Visibility = Visibility.Visible;
                Boton_Domestico.Visibility = Visibility.Visible;
                Boton_Continuar.Visibility = Visibility.Collapsed;
                CambiarBoton("Casa2.png", Seleccion_Sin_Domicilios, "Vecindad.png", Seleccion_Mas_Domicilios);
            }

            else if (pregunta.tipo == 5)
            {
                Boton_Empresarial.Visibility = Visibility.Visible;
                Boton_Domestico.Visibility = Visibility.Visible;
                Boton_Continuar.Visibility = Visibility.Collapsed;
            }

            else if (pregunta.tipo == 6)
            {
                Boton_Empresarial.Visibility = Visibility.Visible;
                Boton_Domestico.Visibility = Visibility.Visible;
                Boton_Continuar.Visibility = Visibility.Collapsed;
                CambiarBoton("Casa2.png", Seleccion_Sin_Domicilios, "Vecindad.png", Seleccion_Mas_Domicilios);
            }

            else if (pregunta.tipo == 7)
            {
                Boton_Empresarial.Visibility = Visibility.Visible;
                Boton_Domestico.Visibility = Visibility.Visible;
                Boton_Continuar.Visibility = Visibility.Collapsed;
            }

            else if (pregunta.tipo == 8)
            {
                Boton_Empresarial.Visibility = Visibility.Visible;
                Boton_Domestico.Visibility = Visibility.Visible;
                Boton_Continuar.Visibility = Visibility.Collapsed;
            }

            else if (pregunta.tipo == 9)
            {
                Boton_Empresarial.Visibility = Visibility.Collapsed;
                Boton_Domestico.Visibility = Visibility.Collapsed;
                Boton_Continuar.Visibility = Visibility.Visible;
            }

            else if (pregunta.tipo == 10)
            {
                Boton_Empresarial.Visibility = Visibility.Collapsed;
                Boton_Domestico.Visibility = Visibility.Collapsed;
                Boton_Continuar.Visibility = Visibility.Visible;
                Numero_Domicilios.Visibility = Visibility.Visible;
            }

            else if (pregunta.tipo == 100)
            {
                Boton_Empresarial.Visibility = Visibility.Collapsed;
                Boton_Domestico.Visibility = Visibility.Collapsed;
                Boton_Continuar.Visibility = Visibility.Visible;
            }

            else
            {

            }
        }

        private void SiguientePregunta()
        {
            if (indice < preguntas.Length - 1)
            {
                indice++;
                MostrarPregunta();
            }
            else
            {
                MessageBox.Show("¡Has completado todas las preguntas!", "Fin del cuestionario",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                CambiarEscena(new Inicio());
            }
        }

        private void Cambiar_Idioma(object sender, SelectionChangedEventArgs e)
        {
            UserData.Idioma = Listado.SelectedItem?.ToString() ?? "";
            UserData.Save(); 
        }
        private void Seleccion_Empresarial(object sender, RoutedEventArgs e)
        {
            UserData.Uso = false;
            UserData.Save();
            SiguientePregunta();
        }

        private void Seleccion_Domestica(object sender, RoutedEventArgs e)
        {
            UserData.Uso = true;
            UserData.Save();
            SiguientePregunta();
        }

        private void Seleccion_Sin_Domicilios(object sender, RoutedEventArgs e)
        {
            UserData.Domicilios = 2;
            UserData.Save();
            indice = 9;
        }

        private void Seleccion_Mas_Domicilios(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(Numero_Domicilios.Text, out int valor))
            {
                UserData.Domicilios = valor;
                UserData.Save();
                indice = 6;
            }
            else
            {
                MessageBox.Show("Por favor, ingresa un número válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void Seleccion_Rentado(object sender, RoutedEventArgs e)
        {
            UserData.Uso = false;
            UserData.Save();
            indice = 5;
        }


        private void Seleccion_Propio(object sender, RoutedEventArgs e)
        {
            indice = 4;
            UserData.Uso= true;
            UserData.Save();
            SiguientePregunta();
        }

        private void Boton_Continuar_Click(object sender, RoutedEventArgs e)
        {
            SiguientePregunta();
        }
    }
    }
