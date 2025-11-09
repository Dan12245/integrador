using Consumo_Reducido_de_Agua_ahora_si_definitivo.Controls;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.View;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Companion; //Necesario para abrir en Companion
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using MediaColor = System.Windows.Media.Color;


namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    public partial class Inicio : UserControl
    {
        string ruta = "UserData.json";
        private MainViewModel mainViewModel = new MainViewModel();

        // Lista de mensajes de Teto
        private List<string> mensajesTeto = new List<string>
        {
            "Did you know that using an AI like ChatGPT consumes a lot of water? That's hard, bro.",
            "Hello",
            "Bonjour",
            "Hola",
            "Hahaha",
            "Proof text",
            "For demaciaaaa"
        };

        // Índice actual para recorrer mensajes de Teto
        private int indiceMensajeTeto =0;

        private Popup tetoPopup;
        private bool primerMensajeMostrado = false;
        private DispatcherTimer caminataTimer;
        private double tetoPosition =586; // Posición inicial del gif
        private bool moviendoDerecha = true;
        private const double POSICION_INICIAL =586;
        private const double RANGO_MOVIMIENTO =70;

        public Inicio()
        {
            InitializeComponent();
            Loaded += Inicio_Loaded;
            DataContext = mainViewModel; // una sola asignación
            MainWindow.UserData.Load();
            InicializarUI();

            CambiarFondoGradiente(
              MediaColor.FromRgb(26,34,53),
              MediaColor.FromRgb(26,34,53),
               0.5,
               "Horizontal"
           );
        }

        private async void Inicio_Loaded(object sender, RoutedEventArgs e)
        {
            // Optimizar: configurar el chart primero (render hints)
            //ConfigurarChart_Fant();

            // cargar edificios y consumos antes de poblar UI
            await mainViewModel.LoadBuildingsAsync();
            if (mainViewModel.SelectedBuilding != null)
            {
                await mainViewModel.LoadConsumptionForSelectedBuildingAsync();
            }

            // Mostrar mensaje de bienvenida de Teto después de un pequeño delay
            var bienvenidaTimer = new DispatcherTimer { Interval = System.TimeSpan.FromSeconds(3) };
            bienvenidaTimer.Tick += (s, ev) =>
            {
                MostrarMensajeTeto("Hello, i'm Teto, your virtual assistant");
                bienvenidaTimer.Stop();
                primerMensajeMostrado = true;
            };
            bienvenidaTimer.Start();
        }

        private void InicializarUI()
        {
            if (Registro.GlobalData.UserName == null)
                Registro.GlobalData.UserName = "Usuario";

            texto_modo.Text = MainWindow.UserData.Uso ? "Domestic mode" : "Business mode";

            Texto_Bienvenida.Text = $"Hello, {Registro.GlobalData.UserName}!";
            Texto_Porcentaje.Text = $"¡Your water consumption has been of {new Random().Next(10,101)}% this month!";

            //Mostrar el primer consejo
            texto_consejo.Text = consejos[indiceActual];

            //Temporizador para cambiar consejo cada5 segundos
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            timer.Tick += Timer_Tick;
            timer.Start();

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community; // Configurar licencia QuestPDF


        }

        private void Boton_Editar_Click(object sender, RoutedEventArgs e)
        {
            var win = new Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Editors.ConsumoEditorWindow(mainViewModel)
            {
                Owner = Application.Current.MainWindow
            };
            win.ShowDialog();
        }

        // Método para agregar mensajes personalizados
        public void AgregarMensajeTeto(string mensaje)
        {
            if (!string.IsNullOrWhiteSpace(mensaje))
            {
                mensajesTeto.Add(mensaje);
            }
        }

        // Mostrar mensaje en un popup anclado al botón de Teto (o al ancla pasada)
        private void MostrarMensajeTeto(string mensaje, FrameworkElement? anchor = null)
        {
            // Cerrar popup anterior si existe
            if (tetoPopup != null && tetoPopup.IsOpen)
            {
                tetoPopup.IsOpen = false;
            }

            // Crear el cuadro de texto
            var textBlock = new TextBlock
            {
                Text = mensaje,
                Padding = new Thickness(10),
                Background = new SolidColorBrush(MediaColor.FromRgb(255,255,220)),
                Foreground = new SolidColorBrush(MediaColor.FromRgb(0,0,0)),
                FontSize =14,
                FontFamily = new FontFamily("Cascadia Code SemiLight"),
                MaxWidth =300,
                TextWrapping = TextWrapping.Wrap
            };

            var border = new Border
            {
                Child = textBlock,
                BorderBrush = new SolidColorBrush(MediaColor.FromRgb(100,100,100)),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(10),
                Background = new SolidColorBrush(MediaColor.FromRgb(255,255,220))
            };

            // Resolver ancla: intentar por parámetro, luego por nombre en el árbol, si no, usar ventana principal
            var target = anchor ?? (FindName("btnSalto") as FrameworkElement) ?? Application.Current.MainWindow as FrameworkElement;

            // Posicionar relativo al target disponible (Teto image/button)
            tetoPopup = new Popup
            {
                Child = border,
                PlacementTarget = target,
                Placement = PlacementMode.Top,
                HorizontalOffset =0,
                VerticalOffset = -10,
                StaysOpen = false,
                AllowsTransparency = true,
                PopupAnimation = PopupAnimation.Fade
            };

            tetoPopup.IsOpen = true;

            // Auto-cerrar después de 8 segundos
            var closeTimer = new DispatcherTimer();
            closeTimer.Interval = TimeSpan.FromSeconds(8);
            closeTimer.Tick += (s, e) =>
            {
                if (tetoPopup != null)
                {
                    tetoPopup.IsOpen = false;
                }
                closeTimer.Stop();
            };
            closeTimer.Start();
        }

        // Avanza al siguiente mensaje del listado y lo muestra
        private void MostrarSiguienteMensajeTeto(FrameworkElement? anchor = null)
        {
            if (mensajesTeto.Count ==0)
                return;

            var mensaje = mensajesTeto[indiceMensajeTeto];
            MostrarMensajeTeto(mensaje, anchor);

            indiceMensajeTeto = (indiceMensajeTeto +1) % mensajesTeto.Count;
        }

        private List<string> consejos = new List<string>
        {
            "Take quick showers: Try to reduce your shower time to 5 minutes or less.",
            "Turn off the tap when not in use: When soaping your hands, etc., avoid letting the water run.",
            "Use a glass to brush your teeth: Instead of using the tap, fill a glass and use that to rinse.",
            "Wash dishes efficiently: Soap everything first with the tap closed, and then rinse them all at once.",
            "Run the washing machine at the recommended maximum load: Don't use the washing machine with only a few clothes.",
            "Water plants at night or early in the morning: This way, you avoid evaporation from the sun and the plants don't dry out.",
            "Reuse water when possible: The water from washing fruits or vegetables can be used to water plants."
        };

        private int indiceActual =0;
        private DispatcherTimer timer;

        private void Timer_Tick(object sender, EventArgs e)
        {
            SiguienteConsejo();
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            SiguienteConsejo();
            ReiniciarTimer();
        }

        private void btnAnterior_Click(object sender, RoutedEventArgs e)
        {
            indiceActual--;
            if (indiceActual <0)
                indiceActual = consejos.Count -1;

            texto_consejo.Text = consejos[indiceActual];
            ReiniciarTimer();
        }

        private void SiguienteConsejo()
        {
            indiceActual++;
            if (indiceActual >= consejos.Count)
                indiceActual =0;

            texto_consejo.Text = consejos[indiceActual];
        }

        private void ReiniciarTimer()
        {
            timer.Stop();
            timer.Start();
        }

        private void btnSalto_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (Storyboard)Resources["SaltoStoryboard"];
            sb.Begin();

            // Mostrar siguiente mensaje del listado cada vez que se hace clic, anclado al botón presionado
            MostrarSiguienteMensajeTeto(sender as FrameworkElement);
        }
        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // restaurar lógica original sin Task.Run: generar gráficos y mostrar PDF directamente
                var original = mainViewModel.SelectedPeriod;
                mainViewModel.SelectedPeriod = "week";
                mainViewModel.SelectedPeriod = "month";
                mainViewModel.SelectedPeriod = "year";
                mainViewModel.SelectedPeriod = original;

                var data = new ViewModels.InvoiceDocumentDataSource();
                var model = data.GetInvoiceDetails();
                var document = new ViewModels.InvoiceDocument(
                model,
                mainViewModel.ChartWeekData,
                mainViewModel.ChartMonthData,
                mainViewModel.ChartYearData);

                // Mostrar directamente (GeneratePdfAndShow crea y abre el PDF temporal)
                document.GeneratePdfAndShow();
                // Abrir en Companion para vista adicional (async fire & forget)
                await document.ShowInCompanionAsync();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error generated: " + ex.Message);
            }
        }
        public static void CambiarFondoGradiente(MediaColor colorInicio, MediaColor colorFin, double offset =0.5, string direccion = "Horizontal")
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.CambiarFondoGradiente(colorInicio, colorFin, offset, direccion: "Horizontal");
            }
        }


    }
}
