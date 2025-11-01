using Consumo_Reducido_de_Agua_ahora_si_definitivo.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Registro;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    public partial class Usuario : UserControl
    {
        public Usuario()
        {
            InitializeComponent();
            Texto_Nombre.Text = $"{GlobalData.UserName}";
            Barra_meta.Value = new Random().Next(10, 101);
        }

        public void CambiarEscena(UserControl nuevoControl)
        {
            Login_Window.Children.Clear();
            Login_Window.Children.Add(nuevoControl);
        }
        private void Boton_ir_a_Configuracion(object sender, RoutedEventArgs e)
        {
            CloseAllCallouts();
            CambiarEscena(new Configuracion());
        }

        private void Boton_ir_a_Invitados(object sender, RoutedEventArgs e)
        {
            CloseAllCallouts();
            CambiarEscena(new Invitados());
        }

        private void Boton_Reporte(object sender, RoutedEventArgs e)
        {
            CloseAllCallouts();
            CambiarEscena(new Reporte());
        }

        private void Boton_ir_a_Inicio(object sender, RoutedEventArgs e)
        {
            CloseAllCallouts();
            CambiarEscena(new Inicio());
        }

        private void Button_Click(object sender, RoutedEventArgs e) { }

        private readonly List<Popup> _calloutPopups = new();
        private void Report_Help_Click(object sender, RoutedEventArgs e)
        {
            if (_calloutPopups.Count > 0)
            {
                CloseAllCallouts();
                return;
            }

            ShowCalloutFor(Boton_Configuracion,
                "Presiona aquí para ir a la ventana principal.\nTambien puedes acceder a la ventana presionando '1'."
                );
            ShowCalloutFor(Boton_Menu,
                "Presiona aquí para ir a la ventana de configuración.\nTambien puedes acceder a la ventana presionando '2'."
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
    }
}
