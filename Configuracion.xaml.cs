using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static C.R.A_Consumo_reducido_de_agua.MainWindow;
using C.R.A_Consumo_reducido_de_agua.Controls;


namespace C.R.A_Consumo_reducido_de_agua
{
    /// <summary>
    /// Lógica de interacción para Configuracion.xaml
    /// </summary>
    public partial class Configuracion : UserControl
    {
        public Configuracion()
        {
            InitializeComponent();
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

        private void Boton_Cerrar_Sesion(object sender, RoutedEventArgs e)
        {
            CloseAllCallouts();
            CambiarEscena(new Login());
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Dominio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #region ConfigCallouts
        private readonly List<Popup> _calloutPopups = new();
        private void Config_Help_Click(object sender, RoutedEventArgs e)
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
                "Presiona aquí para ir a la ventana principal.\nTambien puedes acceder a la ventana presionando '1'."
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
    }
}
