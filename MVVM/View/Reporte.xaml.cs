using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.Controls;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.Services;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para Reporte.xaml
    /// </summary>
    public partial class Reporte : UserControl
    {
        public Reporte()
        {
            InitializeComponent();
        }

        public void CambiarEscena(UserControl nuevoControl)
        {
            Login_Window.Children.Clear();
            Login_Window.Children.Add(nuevoControl);
        }

        private void Boton_Invitados(object sender, RoutedEventArgs e)
        {
            CloseAllCallouts();
            CambiarEscena(new Usuario());
        }

        private void Boton_ir_a_configuracion(object sender, RoutedEventArgs e)
        {
            CloseAllCallouts();
            CambiarEscena(new Configuracion());
        }

        private void Boton_ir_a_Inicio(object sender, RoutedEventArgs e)
        {
            CloseAllCallouts();
            CambiarEscena(new Inicio());
        }

        #region Bug Reporting Methods

        /// <summary>
        /// Envía el reporte de bug
        /// </summary>
        private void SubmitReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar que haya descripción
                if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                {
                    MessageBox.Show(
                        "Por favor describe el problema que encontraste.",
                        "Campo Requerido",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    DescriptionTextBox.Focus();
                    return;
                }

                // Obtener valores
                string errorType = (ErrorTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "No especificado";
                string description = DescriptionTextBox.Text.Trim();

                // Crear título automático basado en el tipo de error
                string title = $"{errorType} - {DateTime.Now:dd/MM/yyyy HH:mm}";

                // Información adicional con el tipo de error
                string additionalInfo = $"Tipo de Error: {errorType}\nReportado desde: Ventana de Reporte";

                // Reportar el bug
                BugReporter.Instance.ReportBug(
                    title: title,
                    description: description,
                    steps: "",
                    additionalInfo: additionalInfo
                );

                // Mostrar mensaje de éxito
                MessageBox.Show(
                    "¡Bug reportado exitosamente!\n\n" +
                    "Gracias por ayudarnos a mejorar la aplicación.\n" +
                    $"El reporte se guardó en:\n{BugReporter.Instance.GetLogFilePath()}",
                    "Reporte Enviado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                // Limpiar formulario
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al enviar el reporte:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                // Reportar el error del mismo sistema de reportes
                BugReporter.Instance.ReportException(ex, "Error en SubmitReport_Click");
            }
        }

        /// <summary>
        /// Limpia el formulario
        /// </summary>
        private void ClearForm()
        {
            ErrorTypeComboBox.SelectedIndex = 0;
            DescriptionTextBox.Clear();
            DescriptionTextBox.Focus();
        }

        /// <summary>
        /// Abre el archivo de reportes
        /// </summary>
        private void ViewReports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!BugReporter.Instance.HasReports())
                {
                    MessageBox.Show(
                        "No hay reportes guardados todavía.\n\n" +
                        "Cuando reportes un bug, aparecerá aquí.",
                        "Sin Reportes",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    return;
                }

                BugReporter.Instance.OpenLogFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al abrir los reportes:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        #endregion

        #region ReportCallouts
        private readonly List<Popup> _calloutPopups = new();

        private void Report_Help_Click(object sender, RoutedEventArgs e)
        {
            // Toggle: if any callouts are open, close them all; otherwise show new callouts.
           
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