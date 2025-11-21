using Consumo_Reducido_de_Agua_ahora_si_definitivo;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.Controls;
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

        private string FilePath => $"Domicilios_{Login.userid}.json";
        public bool Editor = false;

        public Usuario()
        {
            InitializeComponent();

            Loaded += async (s, e) => await CargarDatosIniciales();
            if (Registro.GlobalData.UserName == null)
                Registro.GlobalData.UserName = "Usuario";
            Domicilios = CargarDatosJSON();

            InicializarTimer();
            ActualizarTextBox();

            Texto_Nombre.Text = $"{Registro.GlobalData.UserName}";
            Barra_meta.Value = new Random().Next(10, 101);
            Datos_Usuario.Text = $"Invitados: " + new Random().Next(0, 101);

            ActualizarImagen();
        }
        private async Task CargarDatosIniciales()
        {
            Domicilios = CargarDatosJSON();

            if (Domicilios.Count == 0)
            {
                await Cargar_domicilios();
            }

            miDataGrid.ItemsSource = Domicilios;
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
                MessageBox.Show($"Error al cargar desde JSON: {ex.Message}");
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

        private void ActualizarTextBox()
        {
            DateTime hoy = DateTime.Now;
            int ultimoDia = DateTime.DaysInMonth(hoy.Year, hoy.Month);
            int diasRestantes = ultimoDia - hoy.Day + 1;

            Dias_restantes.Text = $"{diasRestantes} días restantes";
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
                MessageBox.Show($"Error al guardar datos: {ex.Message}");
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
                MessageBox.Show("Modo edición activado");
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
    }
}