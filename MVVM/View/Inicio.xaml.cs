using C.R.A_Consumo_reducido_de_agua.ViewModels;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.ViewModels;
using QuestPDF.Companion;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Windows.System;
using static C.R.A_Consumo_reducido_de_agua.MainWindow;
using static C.R.A_Consumo_reducido_de_agua.Registro;
using C.R.A_Consumo_reducido_de_agua.Controls;

namespace C.R.A_Consumo_reducido_de_agua
{
    public partial class Inicio : UserControl
    {
        string ruta = "UserData.json";
        private MainViewModel mainViewModel = new MainViewModel();
        public Inicio()
        {
            InitializeComponent();
            this.Loaded += Inicio_Loaded;
            UserData.Load();
            if (GlobalData.UserName == null)
                GlobalData.UserName = "Usuario";

            if  (UserData.Uso == false)
                texto_modo.Text = "Modo Empresarial";
            else
                texto_modo.Text = "Modo Doméstico";

            Texto_Bienvenida.Text = $"Hola, {GlobalData.UserName}!";
            Texto_Porcentaje.Text = $"¡Tu consumo de agua ha sido del {new Random().Next(10, 101)}% este mes!";

            // Mostrar primer consejo
            texto_consejo.Text = consejos[indiceActual];

            // Configurar el temporizador
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
            timer.Start();

            // Generar el documento PDF de ejemplo
            QuestPDF.Settings.License = LicenseType.Community; // Establecer el tipo de licencia



            //Be sure all the graphs are generated before creating the document
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
            // PRUEBA 1: Fant (mejor para la mayoría)
            ConfigurarChart_Fant();

            // Si no funciona, prueba:
            // ConfigurarChart_Linear();
            // ConfigurarChart_HighQuality();
        }

        // Configuración 1: Fant (Recomendado)
        private void ConfigurarChart_Fant()
        {
            RenderOptions.SetBitmapScalingMode(Chart, BitmapScalingMode.Fant);
            RenderOptions.SetEdgeMode(Chart, EdgeMode.Unspecified);
            Chart.SnapsToDevicePixels = true;
            Chart.UseLayoutRounding = true;
            TextOptions.SetTextFormattingMode(Chart, TextFormattingMode.Display);
            TextOptions.SetTextRenderingMode(Chart, TextRenderingMode.ClearType);
        }

        // Configuración 2: Linear (Más suave)
        private void ConfigurarChart_Linear()
        {
            RenderOptions.SetBitmapScalingMode(Chart, BitmapScalingMode.Linear);
            Chart.SnapsToDevicePixels = false;
            Chart.UseLayoutRounding = false;
            TextOptions.SetTextFormattingMode(Chart, TextFormattingMode.Ideal);
            TextOptions.SetTextRenderingMode(Chart, TextRenderingMode.ClearType);
        }

        // Configuración 3: HighQuality (Más suave, posible blur)
        private void ConfigurarChart_HighQuality()
        {
            RenderOptions.SetBitmapScalingMode(Chart, BitmapScalingMode.HighQuality);
            RenderOptions.SetClearTypeHint(Chart, ClearTypeHint.Enabled);
            Chart.SnapsToDevicePixels = false;
            Chart.UseLayoutRounding = false;
            TextOptions.SetTextFormattingMode(Chart, TextFormattingMode.Ideal);
        }

        // Configuración 4: Aumentar resolución base
        private void ConfigurarChart_AltaResolucion()
        {
            // Duplicar el tamaño para mejor calidad
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
            Storyboard sb = (Storyboard)this.Resources["SaltoStoryboard"];
            sb.Begin();
        }

        public void CambiarEscena(UserControl nuevoControl)
        {
            Login_Window.Children.Clear();
            Login_Window.Children.Add(nuevoControl);
        }
        //spam de sofi: “
        // Hola papus :D
        //  Gerardwayfan71_"
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #region InicioCallouts
        private readonly List<Popup> _calloutPopups = new();
        private void Inicio_Help_Click(object sender, RoutedEventArgs e)
        {
            // Toggle: if any callouts are open, close them all; otherwise show new callouts.
            if (_calloutPopups.Count > 0)
            {
                CloseAllCallouts();
                return;
            }

            // ShowCalloutFor(Boton_Menu,
            //     "Te encuentras aquí"
            //     );
            ShowCalloutFor(Boton_Configuracion,
                "Presiona aquí para ir a la ventana de configuración.\nTambien puedes acceder a la ventana presionando '2'."
                );
            ShowCalloutFor(Boton_Usuario,
                "Presiona aquí para ir a la ventana de usuario e invitados.\nTambien puedes acceder a la ventana presionando '3'."
                );
            ShowCalloutFor(Boton_Reportar,
                "Presiona aquí para ir a la ventana de reporte de errores.\nTambien puedes acceder a la ventana presionando '4'."
                );
            //ShowCalloutFor(Graph1,
            //    "Presiona aquí para ir a la ventana de reporte de errores.\nTambien puedes acceder a la ventana presionando '4'."
            //    );
            //ShowCalloutFor(Chart,
            //    "Presiona aquí para ir a la ventana de reporte de errores.\nTambien puedes acceder a la ventana presionando '4'."
            //    );
        }
        private void ShowCalloutFor(FrameworkElement target, string message)
        {
            var callout = new CalloutControl
            {
                Text = message,
                // Make the visual non-interactive so underlying controls (like the button)
                // can still receive clicks when a callout overlaps them.
                IsHitTestVisible = false
            };

            var popup = new Popup
            {
                Child = callout,
                PlacementTarget = target,
                Placement = PlacementMode.Right,   // try Top/Bottom/Left/Right or Custom
                HorizontalOffset = 10,
                VerticalOffset = 0,
                // Keep the popup open until we explicitly close it via the button.
                StaysOpen = true,
                AllowsTransparency = true,
                PopupAnimation = PopupAnimation.Fade
            };

            // Ensure popup repositions on layout changes (capture `popup` in the handler)
            EventHandler layoutHandler = (_, __) => popup.HorizontalOffset += 0;
            target.LayoutUpdated += layoutHandler;

            // Clean up when popup closes
            popup.Closed += (_, __) =>
            {
                target.LayoutUpdated -= layoutHandler;
                _calloutPopups.Remove(popup);
                popup.Child = null; // help GC
            };

            _calloutPopups.Add(popup);
            popup.IsOpen = true;
        }

        // Optional helper to close all callouts
        private void CloseAllCallouts()
        {
            foreach (var p in _calloutPopups.ToArray())
            {
                p.IsOpen = false;
            }
            _calloutPopups.Clear();
        }
        #endregion

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            var data = new InvoiceDocumentDataSource();
            var model = data.GetInvoiceDetails();

                var document = new InvoiceDocument(
            model, mainViewModel.ChartWeekData,
            mainViewModel.ChartMonthData,
            mainViewModel.ChartYearData);

           document.GeneratePdfAndShow();
            document.ShowInCompanionAsync();
        }
    }

}
