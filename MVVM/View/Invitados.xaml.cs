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
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Registro;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Usuario;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para Invitados.xaml
    /// </summary>
    public partial class Invitados : UserControl
    {
        public class Invitado
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
        }

        public ObservableCollection<Invitado> Usuario_Invitado { get; set; }
        private const string FilePath = "Invitados.json";
        public bool Editor = false;
        public Invitados()
        {
            InitializeComponent();
            Usuario_Invitado = CargarDatos();
            Grid_Invitados.ItemsSource = Usuario_Invitado;
            ActualizarImagen();
        }

        private ObservableCollection<Invitado> CargarDatos()
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
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
            return new ObservableCollection<Invitado>();
        }

        private void ActualizarImagen()
        {
            string ruta = Editor ? "/Images/IconoGuardado.png" : "/Images/Editar usuario.png";
            BotonImagen.Source = new BitmapImage(new Uri(ruta, UriKind.Relative));
        }

        private void EliminarInvitado()
        {
            if (Grid_Invitados.SelectedItem != null)
            {
                var invitado = (Invitado)Grid_Invitados.SelectedItem;
                Usuario_Invitado.Remove(invitado);
            }
        }

        private void GuardarDatos()
        {
            try
            {
                string json = JsonSerializer.Serialize(Usuario_Invitado, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
                MessageBox.Show("Data saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}");
            }
        }

        private void Boton_Agregar_Click(object sender, RoutedEventArgs e)
        {
            var dialogo = new DialogoAgregarPersona();

            if (dialogo.ShowDialog() == true)
            {
                int nuevoId = Usuario_Invitado.Count > 0 ? Usuario_Invitado.Max(p => p.Id) + 1 : 1;

                var NuevoInvitado = new Invitado
                {
                    Id = Usuario_Invitado.Count + 1,
                    Nombre = dialogo.Nombre,
                    Descripcion = dialogo.Descripcion
                };

                Usuario_Invitado.Add(NuevoInvitado);

                MessageBox.Show("Guest added successfully!", "Success",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
            }
            GuardarDatos();
        }

        private void Boton_Editar_Click(object sender, RoutedEventArgs e)
        {
            Editor = !Editor;
            Grid_Invitados.IsReadOnly = !Editor;

            if (!Editor)
            {
                GuardarDatos();
            }

            if (Editor)
            {
                MessageBox.Show("Edit mode enabled");
            }

            ActualizarImagen();
        }

        private void Boton_Eliminar_Click(object sender, RoutedEventArgs e)
        {
            EliminarInvitado();
            GuardarDatos();
        }
    }
}
