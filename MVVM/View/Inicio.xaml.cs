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
            this.DataContext = mainViewModel;
            MainWindow.UserData.Load();

            CambiarFondoGradiente(
              MediaColor.FromRgb(26,34,53),
              MediaColor.FromRgb(26,34,53),
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
            Texto_Porcentaje.Text = $"¡Tu consumo de agua ha sido del {new Random().Next(10,101)}% este mes!";

            //Mostrar el primer consejo
            texto_consejo.Text = $"Consejo: " + consejos[indiceActual];

            //Temporizador para cambiar consejo cada5 segundos
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
            bienvenidaTimer.Interval = TimeSpan.FromSeconds(2);
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

            // Posicionar relativo al target disponible
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

            // Auto-cerrar después de8 segundos
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
        "Dúchate rápido: Intenta reducir el tiempo en la regadera a5 minutos o menos.",
         "Cierra la llave cuando no la uses: Al enjabonarte las manos, etc evita dejar correr el agua.",
         "Usa un vaso para cepillarte los dientes: En vez de usar la llave, llena un vaso y con eso se enjuaga",
         "Lava los trastes con método: Enjabona todo primero con la llave cerrada y luego enjuágalos de una.",
         "Carga la lavadora al máximo recomendado: No uses la lavadora con poca ropa.",
         "Revisa fugas en baños y llaves: Una fuga puede desperdiciar demasiada agua al mes sin que se note.",
         "Riega de noche o temprano: Así evitas la evaporación por el sol y las plantas no se secan.",
         "Reutiliza agua cuando se pueda: El agua de lavar frutas o verduras puede servir para regar plantas.",
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
            if (indiceActual <0)
                indiceActual = consejos.Count -1;

            texto_consejo.Text = $"Consejo: " + $"Consejo: " + consejos[indiceActual];
            ReiniciarTimer();
        }

        private void SiguienteConsejo()
        {
            indiceActual++;
            if (indiceActual >= consejos.Count)
                indiceActual =0;

            texto_consejo.Text = $"Consejo: " + consejos[indiceActual];
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
        public static void CambiarFondoGradiente(MediaColor colorInicio, MediaColor colorFin, double offset =0.5, string direccion = "Horizontal")
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.CambiarFondoGradiente(colorInicio, colorFin, offset, direccion: "Horizontal");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
