using Consumo_Reducido_de_Agua_ahora_si_definitivo.Controls;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.View;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.ViewModels;
using QuestPDF.Companion;
using QuestPDF.Fluent;
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
            "¿Sabías que usar una IA como ChatGPT consume bastante agua? durísimo hermano",
            "hola",
            "jaja",
            "prueba"
        };

        private Popup tetoPopup;
        private bool primerMensajeMostrado = false;
        private DispatcherTimer caminataTimer;
        private double tetoPosition = 586; // Posición inicial del gif
        private bool moviendoDerecha = true;
        private const double POSICION_INICIAL = 586;
        private const double RANGO_MOVIMIENTO = 70;

        public Inicio()
        {
            InitializeComponent();
            Loaded += Inicio_Loaded;
            this.DataContext = mainViewModel;
            MainWindow.UserData.Load();

            CambiarFondoGradiente(
              MediaColor.FromRgb(26, 34, 53),
              MediaColor.FromRgb(26, 34, 53),
               0.5,
               "Horizontal"
           );

            if (Registro.GlobalData.UserName == null)
                Registro.GlobalData.UserName = "Usuario";

            if (MainWindow.UserData.Uso == false)
                texto_modo.Text = "Modo Empresarial";
            else
                texto_modo.Text = "Modo Doméstico";

            Texto_Bienvenida.Text = $"Hola, {Registro.GlobalData.UserName}!";
            Texto_Porcentaje.Text = $"¡Tu consumo de agua ha sido del {new Random().Next(10, 101)}% este mes!";

            //Mostrar el primer consejo
            texto_consejo.Text = consejos[indiceActual];

            //Temporizador para cambiar consejo cada 5 segundos
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
            timer.Start();

            //Generar el documento PDF de ejemplo
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            // Obtener datos para el documento
            mainViewModel.SelectedPeriod = "month";
            mainViewModel.SelectedPeriod = "year";
            mainViewModel.SelectedPeriod = "week";

            // Mostrar mensaje de bienvenida de Teto después de un pequeño delay
            var bienvenidaTimer = new DispatcherTimer();
            bienvenidaTimer.Interval = TimeSpan.FromSeconds(1);
            bienvenidaTimer.Tick += (s, e) =>
            {
                MostrarMensajeTeto("Hola, soy Teto, tu asistente virtual");
                bienvenidaTimer.Stop();
                primerMensajeMostrado = true;
            };
            bienvenidaTimer.Start();
        }

        // Método para agregar mensajes personalizados
        public void AgregarMensajeTeto(string mensaje)
        {
            if (!string.IsNullOrWhiteSpace(mensaje))
            {
                mensajesTeto.Add(mensaje);
            }
        }

        private void MostrarMensajeTeto(string mensaje)
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
                Background = new SolidColorBrush(MediaColor.FromRgb(255, 255, 220)),
                Foreground = new SolidColorBrush(MediaColor.FromRgb(0, 0, 0)),
                FontSize = 14,
                FontFamily = new FontFamily("Cascadia Code SemiLight"),
                MaxWidth = 300,
                TextWrapping = TextWrapping.Wrap
            };

            var border = new Border
            {
                Child = textBlock,
                BorderBrush = new SolidColorBrush(MediaColor.FromRgb(100, 100, 100)),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(10),
                Background = new SolidColorBrush(MediaColor.FromRgb(255, 255, 220))
            };

            // Necesitamos encontrar el botón con el gif por su nombre en el XAML
            // El botón no tiene x:Name, así que lo buscamos por su posición o le agregamos uno
            // Por ahora, usamos el Grid padre para posicionarlo
            tetoPopup = new Popup
            {
                Child = border,
                PlacementTarget = Application.Current.MainWindow, // Colocar relativo a la ventana principal
                Placement = PlacementMode.Absolute,
                HorizontalOffset = 650, // Ajustar según la posición del gif (586 + ancho)
                VerticalOffset = 70, // Ajustar según la posición del gif
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
        private List<string> consejos = new List<string>
        {
        "Dúchate rápido: Intenta reducir el tiempo en la regadera a 5 minutos o menos.",
         "Cierra la llave cuando no la uses: Al enjabonarte las manos, etc evita dejar correr el agua.",
         "Usa un vaso para cepillarte los dientes: En vez de usar la llave, llena un vaso y con eso se enjuaga",
         "Lava los trastes con método: Enjabona todo primero con la llave cerrada y luego enjuágalos de una.",
         "Carga la lavadora al máximo recomendado: No uses la lavadora con poca ropa.",
         "Revisa fugas en baños y llaves: Una fuga puede desperdiciar demasiada agua al mes sin que se note.",
         "Riega de noche o temprano: Así evitas la evaporación por el sol y las plantas no se secan.",
         "Reutiliza agua cuando se pueda: El agua de lavar frutas o verduras puede servir para regar plantas.",
        };

        private int indiceActual = 0;
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

        private void Inicio_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigurarChart_Fant();
        }

        private void ConfigurarChart_Fant()
        {
            RenderOptions.SetBitmapScalingMode(Chart, BitmapScalingMode.Fant);
            RenderOptions.SetEdgeMode(Chart, EdgeMode.Unspecified);
            Chart.SnapsToDevicePixels = true;
            Chart.UseLayoutRounding = true;
            TextOptions.SetTextFormattingMode(Chart, TextFormattingMode.Display);
            TextOptions.SetTextRenderingMode(Chart, TextRenderingMode.ClearType);
        }
        private void btnAnterior_Click(object sender, RoutedEventArgs e)
        {
            indiceActual--;
            if (indiceActual < 0)
                indiceActual = consejos.Count - 1;

            texto_consejo.Text = consejos[indiceActual];
            ReiniciarTimer();
        }

        private void SiguienteConsejo()
        {
            indiceActual++;
            if (indiceActual >= consejos.Count)
                indiceActual = 0;

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
        }
        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            var data = new ViewModels.InvoiceDocumentDataSource();
            var model = data.GetInvoiceDetails();

            var document = new ViewModels.InvoiceDocument(
            model, mainViewModel.ChartWeekData,
            mainViewModel.ChartMonthData,
            mainViewModel.ChartYearData);

            document.GeneratePdfAndShow();
            document.ShowInCompanionAsync();
        }
        public static void CambiarFondoGradiente(MediaColor colorInicio, MediaColor colorFin, double offset = 0.5, string direccion = "Horizontal")
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.CambiarFondoGradiente(colorInicio, colorFin, offset, direccion: "Horizontal");
            }
        }

        #region InicioCallouts
        private readonly List<Popup> _calloutPopups = new();
        private void Inicio_Help_Click(object sender, RoutedEventArgs e)
        {
            if (_calloutPopups.Count > 0)
            {
                CloseAllCallouts();
                return;
            }
            /*
            ShowCalloutFor(Boton_Configuracion,
                "Presiona aquí para ir a la ventana de configuración.\nTambien puedes acceder a la ventana presionando '2'."
                );
            ShowCalloutFor(Boton_Usuario,
                "Presiona aquí para ir a la ventana de usuario e invitados.\nTambien puedes acceder a la ventana presionando '3'."
                );
            ShowCalloutFor(Boton_Reportar,
                "Presiona aquí para ir a la ventana de reporte de errores.\nTambien puedes acceder a la ventana presionando '4'."
                );
            ShowCalloutFor(Day,
                "Presiona los diferentes períodos para observar los diferentes consumos."
                ); */
        }

        private void ShowCalloutFor(FrameworkElement target, string message)
        {
            var callout = new CalloutControl
            {
                Text = message,
                IsHitTestVisible = false
            };

            var popup = new Popup
            {
                Child = callout,
                PlacementTarget = target,
                Placement = PlacementMode.Right,
                HorizontalOffset = 10,
                VerticalOffset = 0,
                StaysOpen = true,
                AllowsTransparency = true,
                PopupAnimation = PopupAnimation.Fade
            };

            EventHandler layoutHandler = (_, __) => popup.HorizontalOffset += 0;
            target.LayoutUpdated += layoutHandler;

            popup.Closed += (_, __) =>
            {
                target.LayoutUpdated -= layoutHandler;
                _calloutPopups.Remove(popup);
                popup.Child = null;
            };

            _calloutPopups.Add(popup);
            popup.IsOpen = true;
        }

        private void CloseAllCallouts()
        {
            foreach (var p in _calloutPopups.ToArray())
            {
                p.IsOpen = false;
            }
            _calloutPopups.Clear();
        }
        #endregion


    }
}
