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

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    public partial class Inicio : UserControl
    {
        string ruta = "UserData.json";
        private MainViewModel mainViewModel = new MainViewModel();
        private readonly List<Popup> _calloutPopups = new();
        public Inicio()
        {
            InitializeComponent();
            Loaded += Inicio_Loaded;
            MainWindow.UserData.Load();
            if (Registro.GlobalData.UserName == null)
                Registro.GlobalData.UserName = "Usuario";

            if  (MainWindow.UserData.Uso == false)
                texto_modo.Text = "Modo Empresarial";
            else
                texto_modo.Text = "Modo Doméstico";

            Texto_Bienvenida.Text = $"Hola, {Registro.GlobalData.UserName}!";
            Texto_Porcentaje.Text = $"¡Tu consumo de agua ha sido del {new Random().Next(10, 101)}% este mes!";

            texto_consejo.Text = consejos[indiceActual];

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
            timer.Start();

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            mainViewModel.SelectedPeriod = "month";
            mainViewModel.SelectedPeriod = "year";
            mainViewModel.SelectedPeriod = "week";
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

        private void ConfigurarChart_Linear()
        {
            RenderOptions.SetBitmapScalingMode(Chart, BitmapScalingMode.Linear);
            Chart.SnapsToDevicePixels = false;
            Chart.UseLayoutRounding = false;
            TextOptions.SetTextFormattingMode(Chart, TextFormattingMode.Ideal);
            TextOptions.SetTextRenderingMode(Chart, TextRenderingMode.ClearType);
        }

        private void ConfigurarChart_HighQuality()
        {
            RenderOptions.SetBitmapScalingMode(Chart, BitmapScalingMode.HighQuality);
            RenderOptions.SetClearTypeHint(Chart, ClearTypeHint.Enabled);
            Chart.SnapsToDevicePixels = false;
            Chart.UseLayoutRounding = false;
            TextOptions.SetTextFormattingMode(Chart, TextFormattingMode.Ideal);
        }

        private void ConfigurarChart_AltaResolucion()
        {
            Chart.Width = 860;
            Chart.Height = 448;
            RenderOptions.SetBitmapScalingMode(Chart, BitmapScalingMode.HighQuality);
            Chart.SnapsToDevicePixels = false;
            Chart.UseLayoutRounding = false;
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

        public void CambiarEscena(UserControl nuevoControl)
        {
            Login_Window.Children.Clear();
            Login_Window.Children.Add(nuevoControl);
        }

        private void Boton_Usuario_Invitados(object sender, RoutedEventArgs e)
        {
            CloseAllCallouts();
            CambiarEscena(new Usuario());
        }

        private void Boton_Reporte(object sender, RoutedEventArgs e)
        {
            CloseAllCallouts();
            CambiarEscena(new Reporte());
        }

        private void Boton_ir_a_Configuracion(object sender, RoutedEventArgs e)
        {
            CloseAllCallouts();
            CambiarEscena(new Configuracion());
        }

        private void Inicio_Help_Click(object sender, RoutedEventArgs e)
        {
            if (_calloutPopups.Count > 0)
            {
                CloseAllCallouts();
                return;
            }

            ShowCalloutFor(Boton_Configuracion,
                "Presiona aquí para ir a la ventana de configuración.\nTambien puedes acceder a la ventana presionando '2'."
                );
            ShowCalloutFor(Boton_Usuario,
                "Presiona aquí para ir a la ventana de usuario e invitados.\nTambien puedes acceder a la ventana presionando '3'."
                );
            ShowCalloutFor(Boton_Reportar,
                "Presiona aquí para ir a la ventana de reporte de errores.\nTambien puedes acceder a la ventana presionando '4'."
                );
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
    }
}
