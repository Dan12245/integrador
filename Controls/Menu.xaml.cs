using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.Controls
{
    /// <summary>
    /// Logique d'interaction pour Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        public Menu()
        {
            InitializeComponent();
        }

        #region ConfigCallouts
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
            ShowCalloutFor(Boton_Menu,
                "Home.\nShortcut: ctrl + 1."
                );
            ShowCalloutFor(Boton_Usuario,
                "User window.\nShortcut: ctrl + 3."
                );
            ShowCalloutFor(Boton_Reportar,
                "Reports window \nShortcut: ctrl + 4."
                );
            ShowCalloutFor(Boton_Configuracion,
                "Configuration\nShortcut: ctrl + 2."
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

