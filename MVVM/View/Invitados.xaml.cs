using Consumo_Reducido_de_Agua_ahora_si_definitivo;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.Controls;
using MiApp;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Npgsql;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Registro;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    public partial class Invitados : UserControl
    {
        conexion con = new conexion();

        public class Invitado
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string NombreOriginal { get; set; }
            public bool ReadOnly { get; set; }
        }

        public ObservableCollection<Invitado> Usuario_Invitado { get; set; } = new();

        private string FilePath => $"Invitados_{Login.userid}.json";

        public bool Editor = false;

        public Invitados()
        {
            InitializeComponent();
            Loaded += async (s, e) => await CargarDatosIniciales();
            ActualizarImagen();
        }

        private async Task CargarDatosIniciales()
        {
            Usuario_Invitado = CargarDatosJSON();

            if (Usuario_Invitado.Count == 0)
            {
                await CargarInvitadosDesdeDBAsync();
            }

            Grid_Invitados.ItemsSource = Usuario_Invitado;
        }

        private async Task CargarInvitadosDesdeDBAsync()
        {
            try
            {
                await using var conexion = new NpgsqlConnection(con.cadenaconexion());
                await conexion.OpenAsync();

                string query = "SELECT inv_user_id, name, read_only FROM cra.invited_users WHERE user_id = @user_id";

                await using var command = new NpgsqlCommand(query, conexion);
                command.Parameters.AddWithValue("@user_id", Login.userid);

                await using var reader = await command.ExecuteReaderAsync();

                Usuario_Invitado.Clear();

                while (await reader.ReadAsync())
                {
                    var invitado = new Invitado
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        NombreOriginal = reader.GetString(1),
                        ReadOnly = reader.GetBoolean(2)
                    };

                    Usuario_Invitado.Add(invitado);
                }

                GuardarJSON();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar invitados: {ex.Message}");
            }
        }

        private ObservableCollection<Invitado> CargarDatosJSON()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    string json = File.ReadAllText(FilePath);
                    var lista = JsonSerializer.Deserialize<ObservableCollection<Invitado>>(json);
                    return lista ?? new ObservableCollection<Invitado>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar desde JSON: {ex.Message}");
            }
            return new ObservableCollection<Invitado>();
        }

        private void GuardarJSON()
        {
            try
            {
                string json = JsonSerializer.Serialize(Usuario_Invitado, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar JSON: {ex.Message}");
            }
        }

        private void ActualizarImagen()
        {
            string ruta = Editor ? "/Images/IconoGuardado.png" : "/Images/Editar usuario.png";
            BotonImagen.Source = new BitmapImage(new Uri(ruta, UriKind.Relative));
        }

        // ⭐ Eliminar sin código de invitación
        private async void EliminarInvitado()
        {
            if (Grid_Invitados.SelectedItem != null)
            {
                var invitado = (Invitado)Grid_Invitados.SelectedItem;

                var resultado = MessageBox.Show(
                    $"¿Estás seguro de eliminar a '{invitado.Nombre}'?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (resultado == MessageBoxResult.Yes)
                {
                    // Eliminar directamente usando el ID
                    bool exito = await EliminarInvitadoDB(invitado.Id);

                    if (exito)
                    {
                        Usuario_Invitado.Remove(invitado);
                        GuardarJSON();
                        MessageBox.Show("Invitado eliminado correctamente");
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecciona un invitado primero");
            }
        }

        // ⭐ Método directo para eliminar
        private async Task<bool> EliminarInvitadoDB(int invUserId)
        {
            try
            {
                await using var conexion = new NpgsqlConnection(con.cadenaconexion());
                await conexion.OpenAsync();

                string query = "DELETE FROM cra.invited_users WHERE inv_user_id = @inv_user_id AND user_id = @user_id";

                await using var command = new NpgsqlCommand(query, conexion);
                command.Parameters.AddWithValue("@inv_user_id", invUserId);
                command.Parameters.AddWithValue("@user_id", Login.userid);

                int filasAfectadas = await command.ExecuteNonQueryAsync();
                return filasAfectadas > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar invitado: {ex.Message}");
                return false;
            }
        }

        // ⭐ Agregar sin código de invitación
        private async void Boton_Agregar_Click(object sender, RoutedEventArgs e)
        {
            var dialogo = new DialogoAgregarPersona();

            if (dialogo.ShowDialog() == true)
            {
                bool readOnly = false; // O puedes agregarlo al diálogo

                // Agregar directamente sin código
                int nuevoId = await AgregarInvitadoDB(dialogo.Nombre, readOnly);

                if (nuevoId > 0)
                {
                    var nuevoInvitado = new Invitado
                    {
                        Id = nuevoId,
                        Nombre = dialogo.Nombre,
                        NombreOriginal = dialogo.Nombre,
                        ReadOnly = readOnly
                    };

                    Usuario_Invitado.Add(nuevoInvitado);
                    GuardarJSON();

                    MessageBox.Show("Invitado agregado exitosamente!", "Éxito",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Error al agregar el invitado", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // ⭐ Método directo para agregar
        private async Task<int> AgregarInvitadoDB(string nombre, bool readOnly)
        {
            try
            {
                await using var conexion = new NpgsqlConnection(con.cadenaconexion());
                await conexion.OpenAsync();

                string query = @"INSERT INTO cra.invited_users (user_id, name, read_only) 
                                VALUES (@user_id, @name, @read_only) 
                                RETURNING inv_user_id";

                await using var command = new NpgsqlCommand(query, conexion);
                command.Parameters.AddWithValue("@user_id", Login.userid);
                command.Parameters.AddWithValue("@name", nombre);
                command.Parameters.AddWithValue("@read_only", readOnly);

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar invitado: {ex.Message}");
                return -1;
            }
        }

        // ⭐ Editar
        private async void GuardarDatos()
        {
            try
            {
                if (Grid_Invitados.SelectedItem == null)
                {
                    MessageBox.Show("Selecciona un invitado primero");
                    return;
                }

                var invitado = (Invitado)Grid_Invitados.SelectedItem;

                bool exito = await EditarInvitadoDB(invitado.Id, invitado.Nombre, invitado.ReadOnly);

                if (exito)
                {
                    invitado.NombreOriginal = invitado.Nombre;
                    GuardarJSON();
                    MessageBox.Show("Datos guardados correctamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar datos: {ex.Message}");
            }
        }

        private async Task<bool> EditarInvitadoDB(int invUserId, string nuevoNombre, bool readOnly)
        {
            try
            {
                await using var conexion = new NpgsqlConnection(con.cadenaconexion());
                await conexion.OpenAsync();

                string query = @"UPDATE cra.invited_users 
                                SET name = @name, read_only = @read_only 
                                WHERE inv_user_id = @inv_user_id AND user_id = @user_id";

                await using var command = new NpgsqlCommand(query, conexion);
                command.Parameters.AddWithValue("@name", nuevoNombre);
                command.Parameters.AddWithValue("@read_only", readOnly);
                command.Parameters.AddWithValue("@inv_user_id", invUserId);
                command.Parameters.AddWithValue("@user_id", Login.userid);

                int filasAfectadas = await command.ExecuteNonQueryAsync();
                return filasAfectadas > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al editar invitado: {ex.Message}");
                return false;
            }
        }

        private void Boton_Editar_Click(object sender, RoutedEventArgs e)
        {
            Editor = !Editor;
            Grid_Invitados.IsReadOnly = !Editor;

            if (!Editor)
            {
                GuardarDatos();
            }
            else
            {
                MessageBox.Show("Modo edición activado");
            }

            ActualizarImagen();
        }

        private void Boton_Eliminar_Click(object sender, RoutedEventArgs e)
        {
            EliminarInvitado();
        }

        private void Grid_Invitados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}