using Consumo_Reducido_de_Agua_ahora_si_definitivo;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.Controls;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.View;
using MiApp;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Invitados;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Registro;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    public partial class Usuario : UserControl
    {
        public class Domicilio
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
        }

        public class ConfiguracionMeta
        {
            public double MetaConsumo { get; set; } = 100.0;
            public double PromedioActual { get; set; } = 0.0;
        }

        public class EstadoMes
        {
            public int MesActual { get; set; }
            public int AnioActual { get; set; }
            public bool MensajeMostrado { get; set; } = false;
        }

        public ObservableCollection<Domicilio> Domicilios { get; set; }

        private const string FilePath = "Domicilios.json";
        private const string MetaFilePath = "MetaConsumo.json";
        private const string EstadoMesFilePath = "EstadoMes.json";
        public bool Editor = false;
        private ConfiguracionMeta configuracionMeta;
        private EstadoMes estadoMes;

        public Usuario()
        {
            InitializeComponent();

            Domicilios = CargarDatos();
            miDataGrid.ItemsSource = Domicilios;

            if (Registro.GlobalData.UserName == null)
                Registro.GlobalData.UserName = "User";

            configuracionMeta = CargarConfiguracionMeta();
            estadoMes = CargarEstadoMes();

            InicializarTimer();
            ActualizarTextBox();
            ActualizarBarraMeta();

            Texto_Nombre.Text = $"{GlobalData.UserName}";
            Datos_Usuario.Text = $"Guests: " + new Random().Next(0, 101);

            ActualizarImagen();
        }

        private ConfiguracionMeta CargarConfiguracionMeta()
        {
            try
            {
                if (File.Exists(MetaFilePath))
                {
                    string json = File.ReadAllText(MetaFilePath);
                    var config = JsonSerializer.Deserialize<ConfiguracionMeta>(json);
                    return config ?? new ConfiguracionMeta();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading consumption goal configuration: {ex.Message}");
            }
            return new ConfiguracionMeta();
        }

        private EstadoMes CargarEstadoMes()
        {
            try
            {
                if (File.Exists(EstadoMesFilePath))
                {
                    string json = File.ReadAllText(EstadoMesFilePath);
                    var estado = JsonSerializer.Deserialize<EstadoMes>(json);
                    return estado ?? new EstadoMes();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading month status: {ex.Message}");
            }
            return new EstadoMes();
        }

        private void GuardarConfiguracionMeta()
        {
            try
            {
                string json = JsonSerializer.Serialize(configuracionMeta, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(MetaFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving consumption goal configuration: {ex.Message}");
            }
        }

        private void GuardarEstadoMes()
        {
            try
            {
                string json = JsonSerializer.Serialize(estadoMes, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(EstadoMesFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving month status: {ex.Message}");
            }
        }

        private void VerificarNuevoMes()
        {
            DateTime hoy = DateTime.Now;

            if (estadoMes.MesActual != hoy.Month || estadoMes.AnioActual != hoy.Year)
            {
                estadoMes.MesActual = hoy.Month;
                estadoMes.AnioActual = hoy.Year;
                estadoMes.MensajeMostrado = false;
                GuardarEstadoMes();
            }
        }

        private void ActualizarBarraMeta()
        {
            if (configuracionMeta.MetaConsumo > 0)
            {
                double porcentaje = (configuracionMeta.PromedioActual / configuracionMeta.MetaConsumo) * 100;
                porcentaje = Math.Max(0, Math.Min(100, porcentaje));

                Barra_meta.Value = porcentaje;

                Meta_de_consumo_texto.Text =
                    $"Consumption: {configuracionMeta.PromedioActual:F1} m³ / Goal: {configuracionMeta.MetaConsumo:F1} m³ ({porcentaje:F1}%)";

                if (porcentaje <= 70)
                {
                    Barra_meta.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                }
                else if (porcentaje <= 90)
                {
                    Barra_meta.Foreground = new SolidColorBrush(Color.FromRgb(255, 193, 7));
                }
                else
                {
                    Barra_meta.Foreground = new SolidColorBrush(Color.FromRgb(244, 67, 54));
                }
            }
        }

        public void Boton_EditarMeta_Click(object sender, RoutedEventArgs e)
        {
            var dialogo = new Window
            {
                Title = "Edit Consumption Goal",
                Width = 400,
                Height = 250,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var txtMeta = new TextBlock
            {
                Text = "Monthly Consumption Goal (Cubic Meters):",
                FontSize = 16,
                Margin = new Thickness(20, 0, 20, 2),
                VerticalAlignment = VerticalAlignment.Top
            };

            var inputMeta = new TextBox
            {
                Text = configuracionMeta.MetaConsumo.ToString("F1"),
                FontSize = 16,
                Margin = new Thickness(20, 15, 20, 19),
                VerticalAlignment = VerticalAlignment.Bottom
            };

            var txtPromedio = new TextBlock
            {
                Text = "Current Average (Cubic Meters):",
                FontSize = 16,
                Margin = new Thickness(20, 0, 20, 5),
                VerticalAlignment = VerticalAlignment.Top
            };
            Grid.SetRow(txtPromedio, 1);

            var inputPromedio = new TextBox
            {
                Text = configuracionMeta.PromedioActual.ToString("F1"),
                FontSize = 16,
                Margin = new Thickness(20, 10, 20, 20),
                VerticalAlignment = VerticalAlignment.Bottom
            };
            Grid.SetRow(inputPromedio, 1);

            var panelBotones = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 10)
            };
            Grid.SetRow(panelBotones, 2);

            var btnGuardar = new Button
            {
                Content = "Save",
                Width = 100,
                Height = 35,
                Margin = new Thickness(5),
                Background = new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                Foreground = Brushes.White,
                FontSize = 14
            };

            var btnCancelar = new Button
            {
                Content = "Cancel",
                Width = 100,
                Height = 35,
                Margin = new Thickness(5),
                Background = new SolidColorBrush(Color.FromRgb(244, 67, 54)),
                Foreground = Brushes.White,
                FontSize = 14
            };

            btnGuardar.Click += (s, args) =>
            {
                if (double.TryParse(inputMeta.Text, out double meta) &&
                    double.TryParse(inputPromedio.Text, out double promedio))
                {
                    if (meta > 0 && promedio >= 0)
                    {
                        configuracionMeta.MetaConsumo = meta;
                        configuracionMeta.PromedioActual = promedio;
                        GuardarConfiguracionMeta();
                        ActualizarBarraMeta();
                        dialogo.DialogResult = true;
                        dialogo.Close();

                        if (promedio > meta)
                        {
                            MessageBox.Show(
                                "You have exceeded your consumption limit!\n\n" +
                                $"Current consumption: {promedio:F1} m³\n" +
                                $"Set goal: {meta:F1} m³\n" +
                                $"Excess: {(promedio - meta):F1} m³",
                                "Limit Exceeded",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                        }
                        else
                        {
                            MessageBox.Show("Goal successfully updated", "Success",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("The values must be greater than 0", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Please enter valid numeric values", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };

            btnCancelar.Click += (s, args) =>
            {
                dialogo.DialogResult = false;
                dialogo.Close();
            };

            panelBotones.Children.Add(btnGuardar);
            panelBotones.Children.Add(btnCancelar);

            grid.Children.Add(txtMeta);
            grid.Children.Add(inputMeta);
            grid.Children.Add(txtPromedio);
            grid.Children.Add(inputPromedio);
            grid.Children.Add(panelBotones);

            dialogo.Content = grid;
            dialogo.ShowDialog();
        }

        private void ActualizarImagen()
        {
            string ruta = Editor ? "/Images/IconoGuardado.png" : "/Images/Editar.png";
            BotonImagen.Source = new BitmapImage(new Uri(ruta, UriKind.Relative));
        }

        private DispatcherTimer timer;
        private void InicializarTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromHours(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private ObservableCollection<Domicilio> CargarDatos()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    string json = File.ReadAllText(FilePath);
                    var lista = JsonSerializer.Deserialize<ObservableCollection<Domicilio>>(json);
                    return lista ?? new ObservableCollection<Domicilio>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
            return new ObservableCollection<Domicilio>();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ActualizarTextBox();
        }

        private void EliminarDomicilio()
        {
            if (miDataGrid.SelectedItem != null)
            {
                var domicilio = (Domicilio)miDataGrid.SelectedItem;
                Domicilios.Remove(domicilio);
            }
        }

        public void ActualizarTextBox()
        {
            VerificarNuevoMes();

            DateTime hoy = DateTime.Now;
            int ultimoDia = DateTime.DaysInMonth(hoy.Year, hoy.Month);
            int diasRestantes = ultimoDia - hoy.Day;

            if (diasRestantes > 0)
            {
                Dias_restantes.Text = $"{diasRestantes} days remaining";
            }
            else
            {
                Dias_restantes.Text = "Last day of the month";
            }

            if (diasRestantes == 0 && !estadoMes.MensajeMostrado)
            {
                MostrarResumenMensual();
                estadoMes.MensajeMostrado = true;
                GuardarEstadoMes();
            }
        }

        private void MostrarResumenMensual()
        {
            if (configuracionMeta.MetaConsumo > 0)
            {
                double porcentajeConsumo = (configuracionMeta.PromedioActual / configuracionMeta.MetaConsumo) * 100;
                double porcentajeReduccion = 100 - porcentajeConsumo;

                string mensaje;
                string titulo;
                MessageBoxImage icono;

                if (configuracionMeta.PromedioActual <= configuracionMeta.MetaConsumo)
                {
                    mensaje =
                        "🎉 Congratulations! You have reached your consumption goal.\n\n" +
                        "📊 Monthly Summary:\n" +
                        $"• Set goal: {configuracionMeta.MetaConsumo:F1} m³\n" +
                        $"• Actual consumption: {configuracionMeta.PromedioActual:F1} m³\n" +
                        $"• You reduced your consumption by {Math.Abs(porcentajeReduccion):F1}%\n" +
                        $"• Savings: {(configuracionMeta.MetaConsumo - configuracionMeta.PromedioActual):F1} m³\n\n" +
                        "Keep it up! 💧";

                    titulo = "✅ Goal Achieved";
                    icono = MessageBoxImage.Information;
                }
                else
                {
                    double exceso = configuracionMeta.PromedioActual - configuracionMeta.MetaConsumo;

                    mensaje =
                        "⚠️ You did not reach your goal this month.\n\n" +
                        "📊 Monthly Summary:\n" +
                        $"• Set goal: {configuracionMeta.MetaConsumo:F1} m³\n" +
                        $"• Actual consumption: {configuracionMeta.PromedioActual:F1} m³\n" +
                        $"• You exceeded the goal by {(porcentajeConsumo - 100):F1}%\n" +
                        $"• Excess consumption: {exceso:F1} m³\n\n" +
                        "💡 Tip: Try reducing your usage next month.";

                    titulo = "❌ Goal Not Reached";
                    icono = MessageBoxImage.Warning;
                }

                MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, icono);
            }
            else
            {
                MessageBox.Show(
                    "No consumption goal has been set to evaluate this month's usage.",
                    "No Goal",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void GuardarDatos()
        {
            try
            {
                string json = JsonSerializer.Serialize(Domicilios, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
                MessageBox.Show("Data saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}");
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Boton_Agregar2_Click(object sender, RoutedEventArgs e)
        {
            var dialogo = new AgregarDomicilio();

            if (dialogo.ShowDialog() == true)
            {
                int nuevoId = Domicilios.Count > 0 ? Domicilios.Max(p => p.Id) + 1 : 1;

                var NuevoDomicilio = new Domicilio
                {
                    Id = Domicilios.Count + 1,
                    Nombre = dialogo.Nombre,
                    Descripcion = dialogo.Descripcion
                };

                Domicilios.Add(NuevoDomicilio);

                MessageBox.Show("Person added successfully!", "Success",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
            }
            GuardarDatos();
        }

        private void Boton_Editar2_Click(object sender, RoutedEventArgs e)
        {
            Editor = !Editor;
            miDataGrid.IsReadOnly = !Editor;

            if (!Editor)
            {
                GuardarDatos();
            }

            if (Editor)
            {
                MessageBox.Show("Edit mode activated");
            }

            ActualizarImagen();
        }

        private void Boton_Eliminar2_Click(object sender, RoutedEventArgs e)
        {
            EliminarDomicilio();
            GuardarDatos();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow main)
            {
                main.CambiarEscena(new Login());
            }
        }
    }
}
