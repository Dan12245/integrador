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
using Npgsql;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Invitados;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Registro;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    public partial class Usuario : UserControl
    {
        conexion con = new conexion();
        public class Domicilio
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string NombreOriginal { get; set; }
            public string Descripcion { get; set; }
        }

        public class ConfiguracionMeta
        {
            public double MetaConsumo { get; set; } = 100.0;
            public double PromedioActual { get; set; } = 0.0;
        }

        public class EstadoMes
        private ObservableCollection<Domicilio> Domicilios { get; set; } = new();
      

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tb = sender as TextBox;
                if (tb != null && !tb.IsReadOnly)
                {
                    tb.IsReadOnly = true;
                    miDataGrid.Focus();
                }
            }
        }
            public int MesActual { get; set; }
            public int AnioActual { get; set; }
            public bool MensajeMostrado { get; set; } = false;
        }

        private string FilePath => $"Domicilios_{Login.userid}.json";
        public bool Editor = false;
        private ConfiguracionMeta configuracionMeta;
        private EstadoMes estadoMes;

        public Usuario()
        {
            InitializeComponent();

            Loaded += async (s, e) => await CargarDatosIniciales();
            if (Registro.GlobalData.UserName == null)
                Registro.GlobalData.UserName = "Usuario";
            Domicilios = CargarDatosJSON();

            configuracionMeta = CargarConfiguracionMeta();
            estadoMes = CargarEstadoMes();

            InicializarTimer();
            ActualizarTextBox();
            ActualizarBarraMeta();

            Texto_Nombre.Text = $"{Registro.GlobalData.UserName}";
            Barra_meta.Value = new Random().Next(10, 101);
            Datos_Usuario.Text = $"Invitados: " + new Random().Next(0, 101);

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
        private async Task CargarDatosIniciales()
        {
            Domicilios = CargarDatosJSON();

            if (Domicilios.Count == 0)
            {
                await Cargar_domicilios();
            }
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


      
        private async Task Cargar_domicilios()
        {
            try
            {
                await using var conexion = new NpgsqlConnection(con.cadenaconexion());
                await conexion.OpenAsync();

                string query = "SELECT building_id, alias, description FROM cra.buildings WHERE user_id = @user_id";

                await using var command = new NpgsqlCommand(query, conexion);
                command.Parameters.AddWithValue("@user_id", Login.userid);

                await using var reader = await command.ExecuteReaderAsync();

                Domicilios.Clear();

                while (await reader.ReadAsync())
                {
                    var domicilio = new Domicilio
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        NombreOriginal = reader.GetString(1),
                        Descripcion = reader.IsDBNull(2) ? "" : reader.GetString(2)
                    };

                    Domicilios.Add(domicilio);
                }
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromHours(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

                GuardarJSON();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar domicilios: {ex.Message}");
            }
        }

        // Cargar desde JSON 
        private ObservableCollection<Domicilio> CargarDatosJSON()
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
       

        // ⭐ Guardar solo en JSON (rápido)
        private void GuardarJSON()
        {
            try
            {
                string json = JsonSerializer.Serialize(Domicilios, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar JSON: {ex.Message}");
            }
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

        private void Timer_Tick(object sender, EventArgs e)
        {
            ActualizarTextBox();
        }

        // Eliminar: BD + JSON
        private async void EliminarDomicilio()
        {
            if (miDataGrid.SelectedItem != null)
            {
                var domicilio = (Domicilio)miDataGrid.SelectedItem;
                string alias = domicilio.NombreOriginal; // Usar el original

                var resultado = MessageBox.Show(
                    $"¿Estás seguro de eliminar '{domicilio.Nombre}'?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (resultado == MessageBoxResult.Yes)
                {
                    // Eliminar de la BD
                    con.eliminar_domicilio(alias, Login.userid);

                    // Eliminar de la colección
                    Domicilios.Remove(domicilio);

                    // ⭐ Actualizar JSON
                    GuardarJSON();

                    MessageBox.Show("Domicilio eliminado correctamente");
                }
            }
            else
            {
                MessageBox.Show("Selecciona un domicilio primero");
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

        // ⭐ Guardar: BD + JSON
        private async void GuardarDatos()
        {
            try
            {
                if (miDataGrid.SelectedItem == null)
                {
                    MessageBox.Show("Selecciona un domicilio primero");
                    return;
                }

                var domicilio = (Domicilio)miDataGrid.SelectedItem;
                string aliasNuevo = domicilio.Nombre;
                string aliasOriginal = domicilio.NombreOriginal;
                string desc = domicilio.Descripcion;

                // Buscar con el alias ORIGINAL
                int building_id = await con.id_edificio(Login.userid, aliasOriginal);

                if (building_id != -1)
                {
                    // Actualizar en BD
                    bool exito = await con.editar_domicilio(aliasNuevo, desc, building_id);

                    if (exito)
                    {
                        // Actualizar el NombreOriginal
                        domicilio.NombreOriginal = aliasNuevo;

                        // ⭐ Guardar en JSON
                        GuardarJSON();

                        MessageBox.Show("Datos guardados correctamente.");
                    }
                }
                else
                {
                    MessageBox.Show($"No se encontró el edificio '{aliasOriginal}' en la base de datos");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}");
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        // ⭐ Agregar: BD + JSON
        private async void Boton_Agregar2_Click(object sender, RoutedEventArgs e)
        {
            var dialogo = new AgregarDomicilio();

            if (dialogo.ShowDialog() == true)
            {
                // 1. Agregar SOLO a la BD
                int nuevoId = await con.agregar_domicilio(dialogo.Nombre, dialogo.Descripcion, Login.userid);

                if (nuevoId > 0)
                {
                    // 2. Crear el objeto local
                    var nuevoDomicilio = new Domicilio
                    {
                        Id = nuevoId,
                        Nombre = dialogo.Nombre,
                        NombreOriginal = dialogo.Nombre,
                        Descripcion = dialogo.Descripcion
                    };

                    // 3. Agregar a la colección (esto actualiza el DataGrid automáticamente)
                    Domicilios.Add(nuevoDomicilio);

                    // 4. Guardar en JSON
                    GuardarJSON();
                    await RecargarDomicilios();

                    MessageBox.Show("Domicilio agregado exitosamente!", "Éxito",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Error al agregar el domicilio", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
        // ⭐ Método para recargar el DataGrid de domicilios
        public async Task RecargarDomicilios()
        {
            try
            {
                await Cargar_domicilios();
                miDataGrid.Items.Refresh(); // Forzar actualización visual
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al recargar domicilios: {ex.Message}");
            }
        }
        private void Boton_Eliminar2_Click(object sender, RoutedEventArgs e)
        {
            EliminarDomicilio();
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