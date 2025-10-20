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
            ("Seleccione el idioma", 0),                                  // 0
            ("¿La aplicación se utilizará de manera Doméstica o Empresarial?", 1), // 1
            ("¿Su domicilio es propio o rentado?", 2),                    // 2
            ("¿Cuenta con más de un domicilio en renta?", 3),             // 3
            ("Con cuántos domicilios cuenta?", 4),                        // 4
            ("Ingrese el código de invitado que se le dio", 5),           // 5
            ("Elija con qué frecuencia desea recibir las actualizaciones", 6), // 6
            ("¿Con qué tipo de gráfica se siente más cómodo?", 7),        // 7
            ("¿Desea recibir notificaciones sobre consejos o tips para ahorrar agua?", 8), // 8
            ("¿Cuál es la gama de su PC? (rendimiento)", 9),              // 9
        };


        private void CambiarBoton(
            string imagenBoton1,
            RoutedEventHandler eventoBoton1,
            string imagenBoton2,
            RoutedEventHandler eventoBoton2)
        {
            // Ensure callers pass only filenames; we prepend the folder here
            string basePath = "Images/";

            var empresarialUri = new Uri($"pack://application:,,,/{basePath}{imagenBoton2}", UriKind.Absolute);
            var domesticoUri   = new Uri($"pack://application:,,,/{basePath}{imagenBoton1}", UriKind.Absolute);

            var brushEmpresarial = new ImageBrush
            {
                ImageSource = new BitmapImage(empresarialUri),
                Stretch = Stretch.UniformToFill
            };

            var brushDomestico = new ImageBrush
            {
                ImageSource = new BitmapImage(domesticoUri),
                Stretch = Stretch.UniformToFill
            };

            Boton_Empresarial.Background = brushEmpresarial;
            Boton_Domestico.Background   = brushDomestico;

            LimpiarEventos(Boton_Empresarial);
            LimpiarEventos(Boton_Domestico);

            Boton_Empresarial.Click += eventoBoton2;
            Boton_Domestico.Click   += eventoBoton1;
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
                Listado.Items.Clear();
                Listado.Visibility = Visibility.Collapsed;
                CambiarBoton("Casa.jpeg", Seleccion_Domestica, "Empresa.jpg", Seleccion_Empresarial);
                Listado.SelectionChanged -= Cambiar_Idioma;
                Listado.SelectionChanged += Gama_PC;

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
                Numero_Domicilios.Visibility = Visibility.Collapsed;
                CambiarBoton( "Si.png", Seleccion_Mas_Domicilios, "No.png", Seleccion_Sin_Domicilios);
            }

            else if (pregunta.tipo == 5)
            {
                Boton_Empresarial.Visibility = Visibility.Collapsed;
                Boton_Domestico.Visibility = Visibility.Collapsed;
                Boton_Continuar.Visibility = Visibility.Visible;
                Numero_Domicilios.Visibility = Visibility.Visible;
            }

            else if (pregunta.tipo == 6)
            {
                Boton_Empresarial.Visibility = Visibility.Visible;
                Boton_Domestico.Visibility = Visibility.Visible;
                Boton_Continuar.Visibility = Visibility.Collapsed;
                Numero_Domicilios.Visibility = Visibility.Collapsed;
                CambiarBoton("Mes.png", Seleccion_Mes, "Semana.png", Seleccion_Semana);
            }

            else if (pregunta.tipo == 7)
            {
                Boton_Empresarial.Visibility = Visibility.Visible;
                Boton_Domestico.Visibility = Visibility.Visible;
                Boton_Continuar.Visibility = Visibility.Collapsed;
                CambiarBoton("Grafica lineas y puntos.png", Seleccion_Mes, "Grafica barras.png", Seleccion_Semana);
                Listado.Items.Clear();
            }

            else if (pregunta.tipo == 8)
            {
                Boton_Empresarial.Visibility = Visibility.Visible;
                Boton_Domestico.Visibility = Visibility.Visible;
                Boton_Continuar.Visibility = Visibility.Collapsed;
            }

            else if (pregunta.tipo == 9)
            {
                Listado.Visibility = Visibility.Visible;
                Listado.Items.Add("Alta");
                Listado.Items.Add("Media");
                Listado.Items.Add("Baja");
                Boton_Continuar.Visibility = Visibility.Visible;
                Boton_Empresarial.Visibility = Visibility.Collapsed;
                Boton_Domestico.Visibility = Visibility.Collapsed;
            }

            else if (pregunta.tipo == 10)
            {
                Listado.Visibility = Visibility.Visible;
                Listado.Items.Add("Alta");
                Listado.Items.Add("Media");
                Listado.Items.Add("Baja");
                Boton_Continuar.Visibility = Visibility.Visible;
                Boton_Empresarial.Visibility = Visibility.Collapsed;
                Boton_Domestico.Visibility = Visibility.Collapsed;
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
            // Si aún hay preguntas restantes
            if (indice < preguntas.Length - 1)
            {
                indice++;
                MostrarPregunta();
            }
            // Si estamos justo en la última pregunta
            else if (indice == preguntas.Length - 1)
            {

            }
        }

        private void Cambiar_Idioma(object sender, SelectionChangedEventArgs e)
        {
            UserData.Idioma = Listado.SelectedItem?.ToString() ?? "";
            UserData.Save(); 
        }
        private void Gama_PC(object sender, SelectionChangedEventArgs e)
        {
            UserData.Rendimiento = Listado.SelectedItem?.ToString() ?? "";
            UserData.Save();
        }

        private void Seleccion_Mes(object sender, RoutedEventArgs e)
        {
            UserData.Frecuencia = true;
            UserData.Save();
            SiguientePregunta();
        }

        private void Seleccion_Semana(object sender, RoutedEventArgs e)
        {
            UserData.Frecuencia=false;
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
           IrAPregunta(6);
            MostrarPregunta();
        }

        private void Seleccion_Mas_Domicilios(object sender, RoutedEventArgs e)
        {
            UserData.Uso = true;
            UserData.Save();
            IrAPregunta(9);
            MostrarPregunta();

        }

        private void Seleccion_Rentado(object sender, RoutedEventArgs e)
        {
            UserData.Uso = false;
            UserData.Save();
            IrAPregunta(5);
            MostrarPregunta();
        }


        private void Seleccion_Propio(object sender, RoutedEventArgs e)
        {
            IrAPregunta(4);
            UserData.Uso= true;
            UserData.Save();
            MostrarPregunta();
        }

        private void IrAPregunta(int nuevoIndice)
        {
            if (nuevoIndice >= 0 && nuevoIndice < preguntas.Length)
            {
                indice = nuevoIndice;
                MostrarPregunta();
            }
            else
            {
                MessageBox.Show("Índice de pregunta fuera de rango.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Boton_Continuar_Click(object sender, RoutedEventArgs e)
        {
            switch (indice)
            {

                case 4: // Pregunta "Con cuántos domicilios cuenta?"
                    if (!int.TryParse(Numero_Domicilios.Text, out int domicilios) || domicilios <= 1)
                    {
                        MessageBox.Show(
                            "Por favor, ingresa un número válido mayor a 1.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                        );
                        return;
                    }

                    UserData.Domicilios = domicilios;
                    UserData.Save();
                    IrAPregunta(6);
                    break;

                default:
                    break;
            }
            if (indice == preguntas.Length - 1)
            {
                MessageBox.Show("¡Has completado todas las preguntas!", "Fin del cuestionario",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                CambiarEscena(new Inicio());
                return;
            }
            SiguientePregunta();
        }
    }
    }
